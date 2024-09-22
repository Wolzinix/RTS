using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class IABrain : MonoBehaviour
{
    [SerializeField] GameObject groupOfEntity;
    private  Dictionary<BuildingController, BuildingStats> DicoOfBuilding;

    public delegate void NeedToSendEntityToBuildingDelegate(BuildingStats building, Vector3 location);

    public static event NeedToSendEntityToBuildingDelegate NeedToSendEntityToBuildingEvent;


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

        NeedToSendEntityToBuildingEvent += SendEntity;

        ActualiseBuilding();
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
        if(gameObject.CompareTag(building.Tag)) 
        { 
            GetTheClosetEntityOfAPoint(point).GetComponent<EntityController>().AddPath(point); 
        }
        
    }
    void Update()
    {
    }
}
