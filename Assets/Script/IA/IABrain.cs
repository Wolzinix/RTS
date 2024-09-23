using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IABrain : MonoBehaviour
{
    [SerializeField] GameObject groupOfEntity;
    private  Dictionary<BuildingController, BuildingStats> DicoOfBuilding;

    public delegate void NeedToSendEntityToBuildingDelegate(BuildingStats building, Vector3 location);

    public static event NeedToSendEntityToBuildingDelegate NeedToSendEntityToBuildingEvent;

    private List<GroupManager> _ListOfGroup;

    public string _ennemieTag;

    private int TailleDuGroupe = 3;

    public int nbGroup;


    public class BuildingStats
    {
        public List<GameObject> EntityNextTo = new List<GameObject>();
        public string Tag;
        public bool CanSpawn;
        public string TagOfEntity;

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

        NeedToSendEntityToBuildingEvent += SendEntity;
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


    private void SendEntity(BuildingStats building, Vector3 point)
    {
        Debug.Log(building.Tag);
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
            DebugGroup();
        }
       
    }


    private void DebugGroup()
    {
        nbGroup = _ListOfGroup.Count;
        foreach (var group in _ListOfGroup)
        {
            Debug.Log(group.getNumberOnGroup());
        }
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

    private void ActualiseGroup()
    {
        bool InGroup = false;

        
        foreach (EntityController theCloset in groupOfEntity.GetComponentsInChildren<EntityController>())
        {
            if(theCloset.CompareTag(gameObject.tag))
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
                            if (group.getNumberOnGroup() < TailleDuGroupe)
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
                            groupeARejoindre.AddSelect(theCloset.gameObject.GetComponent<EntityManager>());
                            theCloset.AddPath(groupeARejoindre.getCenterofGroup());
                        }
                        else
                        {
                            Creategroup();
                            _ListOfGroup.Reverse();
                            _ListOfGroup[0].AddSelect(theCloset.gameObject.GetComponent<EntityManager>());
                            _ListOfGroup.Reverse();
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
        DebugGroup();
    }

    private void Creategroup()
    {
        GroupManager groupToCreate = new GroupManager();
        groupToCreate.SetAllieTag(tag);
        groupToCreate.SetEnnemieTag(_ennemieTag);
        _ListOfGroup.Add(groupToCreate);
    }
}
