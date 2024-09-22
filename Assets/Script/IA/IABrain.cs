using System.Collections.Generic;
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

        nbGroup = _ListOfGroup.Count;

        foreach (GroupManager group in _ListOfGroup)
        {

            Debug.Log(group.getNumberOnGroup());
        }
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
        if (gameObject.CompareTag(building.Tag))
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
    }

    private void Creategroup()
    {
        GroupManager groupToCreate = new GroupManager();
        groupToCreate.SetAllieTag(tag);
        groupToCreate.SetEnnemieTag(_ennemieTag);
        _ListOfGroup.Add(groupToCreate);
    }
}
