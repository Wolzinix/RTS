using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IABrain : MonoBehaviour
{
    [SerializeField] GameObject groupOfEntity;
    private  Dictionary<BuildingController, BuildingStats> DicoOfBuilding;

    public delegate void NeedToSendToBuildingDelegate(BuildingStats building, Vector3 location);

    public static event NeedToSendToBuildingDelegate NeedToSendEntityToBuildingEvent;

    public static event NeedToSendToBuildingDelegate NeedToSendGroupToBuildingEvent;

    [SerializeField] private GameObject Objectif;

    private List<GroupManager> _ListOfGroup;

    public string _ennemieTag;

    private int TailleDuGroupe = 3;

    public int nbGroup;

    private List<GroupManager> _ListOfGroupToSpawnEntity;

    private List<GroupManager> _ListOfGroupToProtectBuilding;

    private int DistanceOfSecurity = 3;

    public class BuildingStats
    {
        public List<GameObject> EntityNextTo = new List<GameObject>();
        public string Tag;
        public bool CanSpawn;
        public string TagOfEntity;

        public BuildingController building;

        public bool IsProtected;

        public List<GroupManager> _ListOfProtector = new List<GroupManager>();

        public void SetAProtectionGroup(GroupManager group)
        {
            _ListOfProtector.Add(group);
            IsProtected = true;
            group.GroupIsDeadevent.AddListener(RemoveAGroup);
        }

        public void RemoveAGroup(GroupManager group)
        {
            _ListOfProtector.Remove(group);
            if(_ListOfProtector.Count == 0)
            {
                IsProtected = false;
                IABrain.NeedToSendGroupToBuildingEvent(this, building.transform.position);
            }
        }

        public void NeedAGroup()
        {
            IABrain.NeedToSendGroupToBuildingEvent(this, building.transform.position);
        }

        public void changeHaveEntity(List<GameObject> Entity, BuildingController building) 
        {
            CanSpawn = building.GetCanSpawn();

            if(CanSpawn)
            {
                EntityNextTo.Clear();
                foreach (GameObject gameObject in Entity)
                {
                    if (CanSpawn) { TagOfEntity = gameObject.tag; }
                    EntityNextTo.Add(gameObject);
                }
            }
            else
            {
                IABrain.NeedToSendEntityToBuildingEvent(this, building.gameObject.transform.position);
            }
           
        }
    }

    void Start()
    {

        DicoOfBuilding = new Dictionary<BuildingController, BuildingStats>();

        _ListOfGroup = new List<GroupManager>();
        _ListOfGroupToSpawnEntity = new List<GroupManager>();
        _ListOfGroupToProtectBuilding = new List<GroupManager>();

        NeedToSendEntityToBuildingEvent += SendEntityToBuilding;

        NeedToSendGroupToBuildingEvent += SendRenfortToBuilding;
        ActualiseGroup();

        ActualiseBuilding();

    }

    private List<BuildingController> GetAllieBuilding()
    {
        List<BuildingController> list = new List<BuildingController>();

        foreach(BuildingController i in  DicoOfBuilding.Keys)
        {
            i.entityAsBeenBuy.RemoveAllListeners();
            if(!i.CompareTag(_ennemieTag)|| DicoOfBuilding[i].CanSpawn) 
            {
                i.entityAsBeenBuy.AddListener(ActualiseGroup);
                list.Add(i);
                DicoOfBuilding[i].NeedAGroup();
            }
        }
        return list;
    }

    private void ActualiseBuilding()
    {
        BuildingController[] buildings = Resources.FindObjectsOfTypeAll(typeof(BuildingController)) as BuildingController[];
        foreach (BuildingController building in buildings)
        {
            BuildingStats stats = new BuildingStats();
            stats.Tag = building.tag;
            stats.building = building;
            building.EntityNextToEvent.AddListener(stats.changeHaveEntity);
            DicoOfBuilding[building] = stats;
        }
        GetAllieBuilding();
    }

    private GameObject GetTheClosetEntityOfAPoint(Vector3 point)
    {
        GameObject theClosetEntity = null;

        foreach(EntityController theCloset in groupOfEntity.GetComponentsInChildren<EntityController>())
        {
            if (theClosetEntity == null) { theClosetEntity = theCloset.gameObject; }

            if(Vector3.Distance(point,theClosetEntity.transform.position) > Vector3.Distance(point, theCloset.transform.position))
            {
                theClosetEntity = theCloset.gameObject;
            }
        }

        return theClosetEntity;
    }

    private GroupManager GetTheClosetGroupOfAPoint(Vector3 point)
    {
        GroupManager theClosetEntity = null;

        foreach (GroupManager theCloset in _ListOfGroup)
        {
            if (theClosetEntity == null) { theClosetEntity = theCloset; }

            if (Vector3.Distance(point, theClosetEntity.getCenterofGroup()) > Vector3.Distance(point, theCloset.getCenterofGroup()))
            {
                theClosetEntity = theCloset;
            }
        }

        return theClosetEntity;
    }



    public void SendRenfortToBuilding(BuildingStats building, Vector3 location)
    { 
        if (gameObject.CompareTag(building.Tag) || building.Tag == "neutral")
        {
            GroupManager group = GetTheClosetGroupOfAPoint(location);
            _ListOfGroupToProtectBuilding.Add(group);

            Vector3 pos = new Vector3();

            pos.x = location.x + DistanceOfSecurity * Mathf.Cos(Vector3.Angle( location,group.getCenterofGroup()));
            pos.y = location.y;
            pos.z = location.z + DistanceOfSecurity * Mathf.Sin(Vector3.Angle(location, group.getCenterofGroup()));

            SendAGroup(group, pos);

            building.SetAProtectionGroup(group);
        }
    }

    public void SendAGroup(GroupManager group,Vector3 point)
    {
        group.MooveSelected(point);
    }

    private void SendEntityToBuilding(BuildingStats building, Vector3 point)
    {
        if (gameObject.CompareTag(building.Tag) || building.Tag == "neutral")
        {
            EntityController entity = GetTheClosetEntityOfAPoint(point).GetComponent<EntityController>();
            entity.AddPath(point);
            foreach (GroupManager group in _ListOfGroup)
            {
                if (group.GroupContainUnity(entity))
                {
                    group.RemoveSelect(entity.gameObject.GetComponent<EntityManager>());
                }
            }
            Creategroup();
            _ListOfGroup.Last().AddSelect(entity.GetComponent<EntityManager>());
            ClearUselessGroup();
            _ListOfGroupToSpawnEntity.Add(_ListOfGroup.Last());
            //DebugGroup();
        }
       
    }


    private void DebugGroup()
    {
        nbGroup = _ListOfGroup.Count;
        foreach (var group in _ListOfGroup)
        {
            Debug.Log(group.getNumberOnGroup());
        }
        Debug.Log("---------------");
    }


    private void ClearUselessGroup()
    {
        List<GroupManager> groupManagers = new List<GroupManager>(_ListOfGroup);
        foreach(GroupManager group in groupManagers)
        {
            if(group.getNumberOnGroup() == 0)
            {
                _ListOfGroup.Remove(group);
            }
        }
    }

    private void SendToAttack(GroupManager group,GameObject objectif)
    {
        group.AttackingOnTravel(objectif.transform.position);
    }

    private void ActualiseGroup()
    {
        foreach (EntityController theCloset in groupOfEntity.GetComponentsInChildren<EntityController>())
        {
            bool InGroup = false;
            if (theCloset.CompareTag(gameObject.tag))
            {
                GroupManager groupeARejoindre = null;
                if (_ListOfGroup.Count > 0)
                {
                    foreach (GroupManager group in _ListOfGroup)
                    {
                        if (group.GroupContainUnity(theCloset))
                        {
                            InGroup = true;
                        }
                    }
                    if (!InGroup)
                    {
                        foreach (GroupManager group in _ListOfGroup)
                        {
                            if(group.getNumberOnGroup() >= TailleDuGroupe && !_ListOfGroupToSpawnEntity.Contains(group) && !_ListOfGroupToProtectBuilding.Contains(group))
                            {
                                SendToAttack(group,Objectif);
                            }
                            if (group.getNumberOnGroup() < TailleDuGroupe && !_ListOfGroupToSpawnEntity.Contains(group))
                            {
                                if (groupeARejoindre == null)
                                {
                                    groupeARejoindre = group;
                                }
                                else
                                {
                                    if (Vector3.Distance(groupeARejoindre.getCenterofGroup(), theCloset.gameObject.transform.position) > Vector3.Distance(group.getCenterofGroup(), theCloset.gameObject.transform.position))
                                    {
                                        groupeARejoindre = group;
                                    }
                                }
                            }
                        }
                        if (groupeARejoindre != null)
                        {
                            theCloset.ClearAllOrder();
                            theCloset.AddPath(groupeARejoindre.getCenterofGroup());
                           
                            groupeARejoindre.AddSelect(theCloset.gameObject.GetComponent<EntityManager>());
                            
                        }
                        else
                        {
                            Creategroup();
                            _ListOfGroup.Last().AddSelect(theCloset.gameObject.GetComponent<EntityManager>());
                        }
                    }
                }
                else
                {
                    Creategroup();
                    _ListOfGroup[0].AddSelect(theCloset.gameObject.GetComponent<EntityManager>());
                }
            }
            
        }
        ClearUselessGroup();
      //  DebugGroup();
    }

    private void Creategroup()
    {
        GroupManager groupToCreate = new GroupManager();
        groupToCreate.SetAllieTag(tag);
        groupToCreate.SetEnnemieTag(_ennemieTag);
        _ListOfGroup.Add(groupToCreate);
    }

   
}
