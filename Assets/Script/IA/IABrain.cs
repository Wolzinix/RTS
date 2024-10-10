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
    private List<GroupManager> _ListOfGroupToSpawnEntity;
    private List<GroupManager> _ListOfGroupToProtectBuilding;
    private List<BuilderController> _ListOfBuilder;
    private int TailleDuGroupe = 3;

    public string _ennemieTag;
    private float DistanceOfSecurity = 3f;

    public int nbGroup;
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

        public void NeedAGroup() { IABrain.NeedToSendGroupToBuildingEvent(this, building.transform.position);}

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
            else { IABrain.NeedToSendEntityToBuildingEvent(this, building.gameObject.transform.position);  }
           
        }
    }

    void Start()
    {

        DicoOfBuilding = new Dictionary<BuildingController, BuildingStats>();

        _ListOfGroup = new List<GroupManager>();
        _ListOfGroupToSpawnEntity = new List<GroupManager>();
        _ListOfGroupToProtectBuilding = new List<GroupManager>();
        _ListOfBuilder = new List<BuilderController> ();

        NeedToSendEntityToBuildingEvent += SendEntityToBuilding;

        NeedToSendGroupToBuildingEvent += SendRenfortToBuilding;
        ActualiseGroup();

        ActualiseBuilding();

    }

    private void OnDestroy()
    {
        NeedToSendEntityToBuildingEvent -= SendEntityToBuilding;

        NeedToSendGroupToBuildingEvent -= SendRenfortToBuilding;
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
        _ListOfGroup.Remove(groupToRemove);
    }

    private void SendToAttack(GroupManager group,GameObject objectif)
    {
        group.AttackingOnTravel(objectif.transform.position);
    }

    private void GroupToAttack(GroupManager group)
    {
        if (group.getNumberOnGroup() >= TailleDuGroupe && !_ListOfGroupToSpawnEntity.Contains(group) && !_ListOfGroupToProtectBuilding.Contains(group))
        {
            group.ResetOrder();
            SendToAttack(group, Objectif);
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


    public void SendRenfortToBuilding(BuildingStats building, Vector3 location)
    {
        if (gameObject.CompareTag(building.Tag) || building.Tag == "neutral")
        {
            GroupManager group = GetTheClosetGroupOfAPoint(location);
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

    public void SendAGroup(GroupManager group, Vector3 point)
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
                    group.RemoveSelect(entity.gameObject.GetComponent<TroupeManager>());
                }
            }
            Creategroup(entity.GetComponent<TroupeManager>());
            ClearUselessGroup();
            _ListOfGroupToSpawnEntity.Add(_ListOfGroup.Last());
            //DebugGroup();
        }

    }

    private void SendBuilderToHarvest()
    {

    }

    private void AddEntityToGroup(GroupManager group, EntityController entity )
    {
        entity.ClearAllOrder();
        entity.AddPath(group.getCenterofGroup());

        //group.MooveSelected(group.getCenterofGroup());
        group.AddSelect(entity.gameObject.GetComponent<TroupeManager>());

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
    private void ActualiseGroup()
    {
        foreach (EntityController theCloset in groupOfEntity.GetComponentsInChildren<EntityController>())
        {
            if(theCloset.GetComponent<BuilderController>())
            {
                _ListOfBuilder.Add(theCloset.GetComponent<BuilderController>());
            }
            else
            {
                bool InGroup = false;
                if (theCloset.CompareTag(gameObject.tag))
                {
                    GroupManager groupeARejoindre = null;
                    if (_ListOfGroup.Count > 0)
                    {
                        foreach (GroupManager group in _ListOfGroup)
                        {
                            if (group.GroupContainUnity(theCloset)) { InGroup = true; }
                        }

                        if (!InGroup)
                        {
                            foreach (GroupManager group in _ListOfGroup)
                            {
                                if (group.getNumberOnGroup() < TailleDuGroupe && !_ListOfGroupToSpawnEntity.Contains(group))
                                {
                                    if (groupeARejoindre == null) { groupeARejoindre = group; }
                                    else
                                    {
                                        if (Vector3.Distance(groupeARejoindre.getCenterofGroup(), theCloset.gameObject.transform.position) > Vector3.Distance(group.getCenterofGroup(), theCloset.gameObject.transform.position))
                                        {
                                            groupeARejoindre = group;
                                        }
                                    }
                                }
                            }
                            if (groupeARejoindre != null) { AddEntityToGroup(groupeARejoindre, theCloset); }
                            else { Creategroup(theCloset.gameObject.GetComponent<TroupeManager>()); }
                        }
                    }
                    else { Creategroup(theCloset.gameObject.GetComponent<TroupeManager>()); }
                }
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

    private void Creategroup(TroupeManager entity)
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
