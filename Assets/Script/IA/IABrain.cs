using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IABrain : MonoBehaviour
{
    public  List<BuildingController> _AllieBuilding = new List<BuildingController>();
    [SerializeField] GameObject groupOfEntity;

    private  Dictionary<BuildingController, BuildingIA> DicoOfBuilding;

    public  UnityEvent<BuildingIA,Vector3> NeedToSendEntityToBuildingEvent;
    public  UnityEvent<BuildingIA, Vector3> NeedToSendGroupToBuildingEvent;


    public UnityEvent<BuildingIA, Vector3> ATowerIsDestroyEvent;
    public int nbMaxOfTower;

    [SerializeField] private List<GameObject> Objectif;

    IAGroupManager groupManager = new IAGroupManager();

    public string ennemieTag;

    void Start()
    {
        DicoOfBuilding = new Dictionary<BuildingController, BuildingIA>();
        groupManager.ia = this;

        NeedToSendEntityToBuildingEvent.AddListener(SendEntityToBuilding);
        NeedToSendGroupToBuildingEvent.AddListener(SendRenfortToBuilding);
        ATowerIsDestroyEvent.AddListener(AddTowerToBuilding);

        ActualiseGroup();
        ActualiseBuilding();
    }

    private void OnDestroy()
    {
        NeedToSendEntityToBuildingEvent.RemoveAllListeners();
        NeedToSendGroupToBuildingEvent.RemoveAllListeners();
    }

    public void AddObjectif(GameObject newObject)
    {
        if (!Objectif.Contains(newObject)) 
        {
            ActualisePatrol();
            Objectif.Reverse();
            Objectif.Add(newObject);
            Objectif.Reverse();
            AddTowerToBuilding(newObject);
        }
    }
    public void RemoveObjectif(GameObject oldObjectif)
    {
        ActualisePatrol();
        Objectif.Remove( oldObjectif );
    }

    private void AddTowerToBuilding(GameObject newObject)
    {
        foreach (BuildingController building in _AllieBuilding)
        {
            if (DicoOfBuilding[building]._ListOfTower.Count < nbMaxOfTower)
            {
                groupManager.SendBuilderToBuildTower(DicoOfBuilding[building], GetTheNerestPoint(newObject.transform.position,building.SpawnTower(groupManager.DistanceOfSecurity)));
            }
        }
    }

    public void TowerToBuilding(DefenseManager defense, BuildingIA building)
    {
        building.AddTower(defense);
    }

    private void AddTowerToBuilding(BuildingIA building,Vector3 position)
    {
        if (building._ListOfTower.Count < nbMaxOfTower && _AllieBuilding.Contains(building.building))
        {
            groupManager.SendBuilderToBuildTower(building, GetTheNerestPoint(position, building.building.SpawnTower(groupManager.DistanceOfSecurity)));
        }
    }

    private Vector3 GetTheNerestPoint(Vector3 objectif, List<Vector3> listOfPosition)
    {
        Vector3 position = listOfPosition[0];
        foreach(Vector3 i in listOfPosition)
        {
            if(Vector3.Distance(i,objectif) > Vector3.Distance(position, objectif))
            {
                position = i;
            }
        }
        return position;
    }
    private List<BuildingController> GetAllieBuilding()
    {
        _AllieBuilding = new List<BuildingController>();

        foreach(BuildingController i in  DicoOfBuilding.Keys)
        {
            if (i && groupManager.BuildingIsProtected(DicoOfBuilding[i]))
            {
                _AllieBuilding.Add(i);
            }
            else if (i)
            {
                i.entityAsBeenBuy.RemoveAllListeners();
                if (!i.CompareTag(ennemieTag) || DicoOfBuilding[i].CanSpawn && i.tagOfNerestEntity == gameObject.tag)
                {
                    i.entityAsBeenBuy.AddListener(ActualiseGroup);
                    _AllieBuilding.Add(i);
                    DicoOfBuilding[i].NeedAGroup();
                }
            }
        }
        return _AllieBuilding;
    }
    private GameObject GetThenearsetEntityOfAPoint(Vector3 point)
    {
        GameObject ThenearsetEntity = null;

        foreach(EntityController Thenearset in groupOfEntity.GetComponentsInChildren<EntityController>())
        {
            if (ThenearsetEntity == null) { ThenearsetEntity = Thenearset.gameObject; }

            if(Vector3.Distance(point,ThenearsetEntity.transform.position) > Vector3.Distance(point, Thenearset.transform.position))
            {
                ThenearsetEntity = Thenearset.gameObject;
            }
        }

        return ThenearsetEntity;
    }
    public RessourceManager GetThenearsetHarvestOfABuilder(BuilderController builder)
    {
        RessourceManager[] listOfRessources =  FindObjectsOfType<RessourceManager>();
        if(listOfRessources.Length > 0)
        {
            RessourceManager ThenearsetToReturn = listOfRessources[0];

            foreach (RessourceManager Thenearset in listOfRessources)
            {
                if (ThenearsetToReturn != Thenearset && Thenearset)
                {
                    if (Vector3.Distance(Thenearset.gameObject.transform.position, builder.gameObject.transform.position) < Vector3.Distance(ThenearsetToReturn.gameObject.transform.position, builder.gameObject.transform.position))
                    {
                        ThenearsetToReturn = Thenearset;
                    }
                }
            }
            if (listOfRessources.Length > 0) { return ThenearsetToReturn; }
            else { return null; }
        }
        else  {  return null;}
    }
    private void SendEntityToBuilding(BuildingIA building, Vector3 point)
    {
        if (gameObject.CompareTag(building.Tag) || building.Tag == "neutral")
        {
            EntityController entity = GetThenearsetEntityOfAPoint(point).GetComponent<EntityController>();
            groupManager.SendEntityToBuilding(building, point, entity);
        }

    }
    public void ActualiseGroup()
    {
        foreach (EntityController currentEntity in groupOfEntity.GetComponentsInChildren<EntityController>())
        {
            bool InGroup = groupManager.EntityIsInAGroup(currentEntity);
            if (!InGroup)
            {
                if (currentEntity.GetComponent<BuilderController>()) { groupManager.AddBuilder(currentEntity.GetComponent<BuilderController>()); }

                else if (currentEntity.CompareTag(gameObject.tag) && currentEntity.GetComponent<TroupeManager>())
                {
                    groupManager.EntityJoinAGroup(currentEntity); 
                }
            }
        }
        ActualisePatrol();
        //DebugGroup();
    }
   
    public void ActualiseTheGroup(GroupManager group)
    {
        if (Objectif.Count > 0)
        {
            if (Objectif[0]) { groupManager.GroupToAttack(group, Objectif[0]); }
            else
            {
                Objectif.RemoveAt(0);
                ActualiseTheGroup(group);
            }
        }
    }

    private void ActualiseBuilding()
    {
        BuildingController[] buildings = FindObjectsOfType<BuildingController>();
        foreach (BuildingController building in buildings)
        {
            BuildingIA stats = new BuildingIA();
            stats.Tag = building.tag;
            stats.building = building;
            stats.IAbrain = this;
            building.EntityNextToEvent.AddListener(stats.changeHaveEntity);
            DicoOfBuilding[building] = stats;
            if(!building.CompareTag(ennemieTag))
            {
                stats.NeedToSendEntity();
            }
        }
        
        ActualisePatrol();
    }

    private void ActualisePatrol()
    {
        GetAllieBuilding();
        groupManager.ClearListOfPatrol();
        for (int i = 0; i < _AllieBuilding.Count - 1 ; i++)
        {
            for (int w = 1; w < _AllieBuilding.Count; w++)
            {
                if(Vector3.Distance(_AllieBuilding[i].transform.position, _AllieBuilding[w].transform.position)<30)
                {
                    List<BuildingController> buildings = new List<BuildingController>
                    {
                        _AllieBuilding[i],
                        _AllieBuilding[w]
                    };
                    groupManager.SendAGroupToPatrol(_AllieBuilding[i].transform.position, buildings);
                }
            }
        }
    }
    public void SendRenfortToBuilding(BuildingIA building, Vector3 location)
    {
        if (gameObject.CompareTag(building.Tag) || building.Tag == "neutral")
        {
            groupManager.SendRenfortToBuilding(building, location);
        }
    }

}
