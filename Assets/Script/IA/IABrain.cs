using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IABrain : MonoBehaviour
{
    [SerializeField] public GameObject groupOfEntity;
    [HideInInspector] public IAStockBuilding stockBuilding;

    [HideInInspector] public  UnityEvent<BuildingIA,Vector3> NeedToSendEntityToBuildingEvent;
    [HideInInspector] public  UnityEvent<BuildingIA, Vector3> NeedToSendGroupToBuildingEvent;
    [HideInInspector] public UnityEvent<BuildingIA, Vector3> ATowerIsDestroyEvent;
    public int nbMaxOfTower;

    [SerializeField] private List<GameObject> Objectif;

    private IAGroupManager groupManager = new IAGroupManager();

    public string ennemieTag;

    void Start()
    {
        stockBuilding = new IAStockBuilding();
        stockBuilding.IAbrain = this;
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
        ATowerIsDestroyEvent.RemoveAllListeners();
    }

    public void AddObjectif(GameObject newObject)
    {
        if (!Objectif.Contains(newObject)) 
        {
            Objectif.Insert(0, newObject);
            AddTowerToEveryBuilding(newObject);
        }
    }
    public void RemoveObjectif(GameObject oldObjectif)
    {
        Objectif.Remove( oldObjectif );
    }

    private void AddTowerToEveryBuilding(GameObject newObject)
    {
        stockBuilding.AddTowerToEveryBuilding(newObject);
    }

    public void TowerToBuilding(DefenseManager defense, BuildingIA building)
    {
        building.AddTower(defense);
    }

    private void AddTowerToBuilding(BuildingIA building,Vector3 position)
    {
        if (building._ListOfTower.Count < nbMaxOfTower && stockBuilding._AllieBuilding.Contains(building))
        {
            groupManager.SendBuilderToBuildTower(building, GetTheNerestPoint(position, building.building.SpawnTower(groupManager.DistanceOfSecurity)));
        }
    }

    public void AddTowerToBuilding(BuildingIA building, GameObject newObject)
    {
        if (building._ListOfTower.Count < nbMaxOfTower && stockBuilding._AllieBuilding.Contains(building))
        {
            groupManager.SendBuilderToBuildTower(building, GetTheNerestPoint(newObject.transform.position, building.building.SpawnTower(groupManager.DistanceOfSecurity)));
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
    public List<BuildingIA> GetAllieBuilding()
    {
        return stockBuilding.GetAllieBuilding();
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
            GroupManager group = groupManager.SendEntityToBuilding(building, entity);
            if(group != null)
            {

                building.AddSpawnGroup(group);
            }
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

    public void ActualiseBuilding()
    {
        stockBuilding.ActualiseBuilding(FindObjectsOfType<BuildingController>());
    }

    public void ActualisePatrol()
    {
        List<BuildingIA> listOfAllie = stockBuilding.GetAllieBuilding();
        groupManager.ClearListOfPatrol();
        for (int i = 0; i < listOfAllie.Count - 1 ; i++)
        {
            for (int w = 1; w < listOfAllie.Count; w++)
            {
                groupManager.VerifyForPatrol(listOfAllie[i], listOfAllie[w]);
            }
        }
    }
    public void ActualisePatrol(BuildingIA building)
    {
        List<BuildingIA> listOfAllie = stockBuilding.GetAllieBuilding();
        foreach(BuildingIA i  in   listOfAllie)
        {
            if(i != building) { groupManager.VerifyForPatrol(building, i); }
        }
    }
    public void SendRenfortToBuilding(BuildingIA building, Vector3 location)
    {
        if (gameObject.CompareTag(building.Tag) || building.Tag == "neutral")
        {
            groupManager.SendRenfortToBuilding(building, location);
        }
    }
    public void SpawnEveryEntityOfABuilding(BuildingController building)
    {
        building.SpawnEveryEntity(tag, groupOfEntity, GetComponent<RessourceController>());
    }

    public void SpawnEntityOfBuilding(BuildingController building, GameObject entity)
    {
        building.SpawnEntity(entity,tag, groupOfEntity.GetComponentInChildren<EntityController>().gameObject, GetComponent<RessourceController>());
    }
}
