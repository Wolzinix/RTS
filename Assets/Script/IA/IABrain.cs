using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class IABrain : MonoBehaviour
{
    [SerializeField] GameObject groupOfEntity;

    private  Dictionary<BuildingController, BuildingIA> DicoOfBuilding;

    public  UnityEvent<BuildingIA,Vector3> NeedToSendEntityToBuildingEvent;
    public  UnityEvent<BuildingIA, Vector3> NeedToSendGroupToBuildingEvent;

    [SerializeField] private List<GameObject> Objectif;

    private int TailleDuGroupe = 3;
   
    private List<GroupManager> _ListOfGroup;
    private List<GroupManager> _ListOfGroupToSpawnEntity;
    private List<GroupManager> _ListOfGroupToProtectBuilding;
    private Dictionary<GroupManager,List<BuildingController>> _ListOfGroupPatrol;
    private List<BuildingController> _AllieBuilding;
    private List<BuilderController> _ListOfBuilder;
   

    public string _ennemieTag;
    private float DistanceOfSecurity = 3f;

    public int nbGroup;

    void Start()
    {

        DicoOfBuilding = new Dictionary<BuildingController, BuildingIA>();

        _ListOfGroup = new List<GroupManager>();
        _ListOfGroupToSpawnEntity = new List<GroupManager>();
        _ListOfGroupToProtectBuilding = new List<GroupManager>();
        _ListOfBuilder = new List<BuilderController> ();
        _ListOfGroupPatrol = new Dictionary<GroupManager, List<BuildingController>>();
        _AllieBuilding = new List<BuildingController> ();

        NeedToSendEntityToBuildingEvent.AddListener( SendEntityToBuilding);

        NeedToSendGroupToBuildingEvent.AddListener(SendRenfortToBuilding);
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
        if(!Objectif.Contains(newObject)) 
        {
            ActualisePatrol();
            Objectif.Reverse();
            Objectif.Add(newObject);
            Objectif.Reverse();
        }
    }

    public void RemoveObjectif(GameObject oldObjectif)
    {
        ActualisePatrol();
        Objectif.Remove( oldObjectif );
    }

    private List<BuildingController> GetAllieBuilding()
    {
        List<BuildingController> list = new List<BuildingController>();

        foreach(BuildingController i in  DicoOfBuilding.Keys)
        {
            if(i)
            {
                i.entityAsBeenBuy.RemoveAllListeners();
                if (!i.CompareTag(_ennemieTag) || DicoOfBuilding[i].CanSpawn && i.tagOfNerestEntity == gameObject.tag)
                {
                    i.entityAsBeenBuy.AddListener(ActualiseGroup);
                    list.Add(i);
                    DicoOfBuilding[i].NeedAGroup();
                }
            }
          
        }
        return list;
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

    private GroupManager GetThenearsetGroupOfAPoint(Vector3 point)
    {
        GroupManager ThenearsetEntity = null;

        foreach (GroupManager Thenearset in _ListOfGroup)
        {
            if (!_ListOfGroupToSpawnEntity.Contains(Thenearset) && !_ListOfGroupToProtectBuilding.Contains(Thenearset) && !_ListOfGroupPatrol.Keys.Contains(Thenearset))
            {
                if (ThenearsetEntity == null) { ThenearsetEntity = Thenearset; }

                if (Vector3.Distance(point, ThenearsetEntity.getCenterofGroup()) > Vector3.Distance(point, Thenearset.getCenterofGroup()))
                {
                    ThenearsetEntity = Thenearset;
                }
            }
           
        }

        return ThenearsetEntity;
    }

    private RessourceManager GetThenearsetHarvestOfABuilder(BuilderController builder)
    {
       
        RessourceManager[] listOfRessources =  FindObjectsOfType<RessourceManager>();
        RessourceManager ThenearsetToReturn = listOfRessources[0];

        foreach(RessourceManager Thenearset in listOfRessources)
        {
            if(ThenearsetToReturn != Thenearset && Thenearset)
            {
                if(Vector3.Distance(Thenearset.gameObject.transform.position,builder.gameObject.transform.position) < Vector3.Distance(ThenearsetToReturn.gameObject.transform.position, builder.gameObject.transform.position))
                {
                    ThenearsetToReturn = Thenearset;
                }
            }
        }
        if(listOfRessources.Length > 0) {  return ThenearsetToReturn; }
        else { return null;}
    }
    private void DebugGroup()
    {
        nbGroup = _ListOfGroup.Count;
        foreach (var group in _ListOfGroup) { Debug.Log(group.getNumberOnGroup()); }
        Debug.Log("---------------");
    }


    private void ClearUselessGroup()
    {
        List<GroupManager> groupManagers = new List<GroupManager>(_ListOfGroup);
        foreach(GroupManager group in groupManagers)
        {
            if(group.getNumberOnGroup() == 0){ RemoveAGroup(group); }
        }
    }

    private void RemoveAGroup(GroupManager groupToRemove)
    {
        if(_ListOfGroupToProtectBuilding.Contains(groupToRemove)) { _ListOfGroupToProtectBuilding.Remove(groupToRemove);  }
        if(_ListOfGroupToSpawnEntity.Contains(groupToRemove)) { _ListOfGroupToSpawnEntity.Remove(groupToRemove); }
        if(_ListOfGroupPatrol.Keys.Contains(groupToRemove)) { _ListOfGroupPatrol.Remove(groupToRemove); }
        _ListOfGroup.Remove(groupToRemove);
    }

    private void SendToAttack(GroupManager group,GameObject objectif) { group.AttackingOnTravel(objectif.transform.position);}

    private void GroupToAttack(GroupManager group)
    {
        if (group.getNumberOnGroup() >= TailleDuGroupe)
        {
            group.ResetOrder();
            if(Objectif.Count>0)
            {
                if (Objectif[0]) { SendToAttack(group, Objectif[0]); }
                else
                {
                    Objectif.RemoveAt(0);
                    GroupToAttack(group);
                }
            }
        }
    }
    private void SendEverybodyToTheCenter(GroupManager group)
    {
        foreach (EntityController entity in group.getSelectList())
        {
            entity.ClearAllOrder();
            entity.AddPath(group.getCenterofGroup());
        }
    }


    public void SendRenfortToBuilding(BuildingIA building, Vector3 location)
    {
        if (gameObject.CompareTag(building.Tag) || building.Tag == "neutral")
        {
            GroupManager group = GetThenearsetGroupOfAPoint(location);
            if(group != null)
            {
                _ListOfGroupToProtectBuilding.Add(group);

                Vector3 centerOfGroup = group.getCenterofGroup();

                float angle = Vector3.Angle(centerOfGroup, location);
                angle = Vector3.Cross(centerOfGroup, location).y > 0 ? angle : 360 - angle;

                float x = DistanceOfSecurity * Mathf.Cos((float)angle);
                float y = DistanceOfSecurity * Mathf.Sin((float)angle);

                Vector3 pos = new Vector3(x, 1, y);

                pos.x += centerOfGroup.x;
                pos.z += centerOfGroup.z;

                group.ResetOrder();
                SendAGroup(group, pos);

                building.SetAProtectionGroup(group);
            }
            
        }
    }

    public void SendAGroup(GroupManager group, Vector3 point){ group.MooveSelected(point); }

    private void SendEntityToBuilding(BuildingIA building, Vector3 point)
    {
        if (gameObject.CompareTag(building.Tag) || building.Tag == "neutral")
        {
            EntityController entity = GetThenearsetEntityOfAPoint(point).GetComponent<EntityController>();
            entity.AddPath(point);
            foreach (GroupManager group in _ListOfGroup)
            {
                if (group.GroupContainUnity(entity))
                {
                    group.RemoveSelect(entity.gameObject.GetComponent<AggressifEntityManager>());
                }
            }
            Creategroup(entity.GetComponent<AggressifEntityManager>());
            ClearUselessGroup();
            _ListOfGroupToSpawnEntity.Add(_ListOfGroup.Last());


            //DebugGroup();
        }

    }

    private void SendBuilderToHarvest(BuilderController builder)
    {
        GameObject nextHarvest = GetThenearsetHarvestOfABuilder(builder).gameObject;
        if(nextHarvest)
        {
            builder.AddHarvestTarget(nextHarvest);
        }
   
    }

    private void AddEntityToGroup(GroupManager group, EntityController entity )
    {
        entity.ClearAllOrder();
        entity.AddPath(group.getCenterofGroup());
        group.AddSelect(entity.gameObject.GetComponent<AggressifEntityManager>());

        entity.GroupNumber = _ListOfGroup.IndexOf(group);
    }

   

    private bool EverybodyIsImmobile(GroupManager group)
    {
        bool Immobile = true;
        foreach(EntityController entity in group.getSelectList())
        {
            if(entity._listForOrder.Count>0)
            {
                Immobile = false;
            }
        }
        return Immobile;
    }

    private void AddBuilder(BuilderController builder)
    {
        if (!_ListOfBuilder.Contains(builder))
        {
            _ListOfBuilder.Add(builder);
            builder.NoMoreToHarvest.AddListener(SendBuilderToHarvest);
        }
        if (builder._listForOrder.Count == 0)
        {
            SendBuilderToHarvest(builder);
        }
    }
    private void ActualiseGroup()
    {
        foreach (EntityController Thenearset in groupOfEntity.GetComponentsInChildren<EntityController>())
        {
            if(Thenearset.GetComponent<BuilderController>())
            {
                AddBuilder(Thenearset.GetComponent<BuilderController>());
            }

            else if (Thenearset.CompareTag(gameObject.tag) && Thenearset.GetComponent<TroupeManager>())
            {
                bool InGroup = false;
                GroupManager groupeARejoindre = null;
                if (_ListOfGroup.Count > 0)
                {  
                    foreach (GroupManager group in _ListOfGroup)
                    {
                        if (group.GroupContainUnity(Thenearset)) { InGroup = true; }

                        if (!InGroup)
                        {
                            if (group.getNumberOnGroup() < TailleDuGroupe && !_ListOfGroupToSpawnEntity.Contains(group))
                            {
                                if (groupeARejoindre == null) { groupeARejoindre = group; }
                                else
                                {
                                    if (Vector3.Distance(groupeARejoindre.getCenterofGroup(), Thenearset.gameObject.transform.position) > Vector3.Distance(group.getCenterofGroup(), Thenearset.gameObject.transform.position))
                                    {
                                        groupeARejoindre = group;
                                    }
                                }
                            }
                        }
                    }
                    if (!InGroup)
                    {
                        if (groupeARejoindre != null) { AddEntityToGroup(groupeARejoindre, Thenearset); }
                        else { Creategroup(Thenearset.gameObject.GetComponent<AggressifEntityManager>()); }
                    }
                   
                }
                else { Creategroup(Thenearset.gameObject.GetComponent<AggressifEntityManager>()); }
            }
            
        }
        ClearUselessGroup();
        //DebugGroup();
    }

    private void ActualiseTheGroup(GroupManager group)
    {
        SendEverybodyToTheCenter(group);
        if(EverybodyIsImmobile(group)){
            GroupToAttack(group);
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
        }
        
        ActualisePatrol();
    }

    private void ActualisePatrol()
    {
        _AllieBuilding = GetAllieBuilding();
        ClearListOfPatrol();
        for (int i = 0; i < _AllieBuilding.Count - 1 ; i++)
        {
            for (int w = 1; w < _AllieBuilding.Count; w++)
            {
                if(Vector3.Distance(_AllieBuilding[i].transform.position, _AllieBuilding[w].transform.position)<30)
                {
                    bool exist = false;
                    GroupManager NearestGroup = GetThenearsetGroupOfAPoint(_AllieBuilding[i].transform.position);
                    if (NearestGroup != null)
                    {
                        List<BuildingController> buildings = new List<BuildingController>
                        {
                            _AllieBuilding[i],
                            _AllieBuilding[w]
                        };
                        foreach (GroupManager x in _ListOfGroupPatrol.Keys)
                        {
                            if (_ListOfGroupPatrol[x] == buildings || x == NearestGroup)
                            {
                                exist = true;
                            }
                        }
                        if(!exist)
                        {
                        
                            _ListOfGroupPatrol.Add(NearestGroup, buildings);
                            NearestGroup.SpecificPatrouilleOrder(_AllieBuilding[i].transform.position, _AllieBuilding[w].transform.position);
                        }
                      
                    }
                        
                }
           
            }
            
        }
    }

    private void ClearListOfPatrol()
    {
        foreach(GroupManager i in _ListOfGroupPatrol.Keys)
        {
            if (!_AllieBuilding.Contains(_ListOfGroupPatrol[i][0]) || !_AllieBuilding.Contains(_ListOfGroupPatrol[i][1]))
            {
                i.ResetOrder();
                _ListOfGroupPatrol.Remove(i);
            }
        }
    }

    private void Creategroup(AggressifEntityManager entity)
    {
        GroupManager groupToCreate = new GroupManager();
        groupToCreate.SetAllieTag(tag);
        groupToCreate.SetEnnemieTag(_ennemieTag);
        _ListOfGroup.Add(groupToCreate);

        groupToCreate.GroupIsDeadevent.AddListener(RemoveAGroup);
        groupToCreate.SomeoneIsImmobile.AddListener(ActualiseTheGroup);

        groupToCreate.AddSelect(entity);
        entity.gameObject.GetComponent<EntityController>().GroupNumber = _ListOfGroup.IndexOf(groupToCreate);
        
    }
}
