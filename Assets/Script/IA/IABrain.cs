using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IABrain : MonoBehaviour
{
    [SerializeField] GameObject groupOfEntity;
    private  Dictionary<BuildingController, BuildingStats> DicoOfBuilding;


    public class BuildingStats
    {
        public List<GameObject> EntityNextTo = new List<GameObject>();
        public string Tag;
        public bool CanSpawn;
        public string TagOfEntity;

        public void changeHaveEntity(List<GameObject> Entity, BuildingController building) 
        {
            CanSpawn = building.GetCanSpawn();

            EntityNextTo.Clear();
            foreach (GameObject gameObject in Entity)
            {
                if(CanSpawn) { TagOfEntity = gameObject.tag; }
                EntityNextTo.Add(gameObject);
            }
        }
    }

    void Start()
    {

        DicoOfBuilding = new Dictionary<BuildingController, BuildingStats>();

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

            if(Vector3.Distance(point,theClosetEntity.transform.position) < Vector3.Distance(point, theCloset.transform.position))
            {
                theClosetEntity = theCloset.gameObject;
            }
        }

        return theClosetEntity;
    }

    void Update()
    {
    }
}
