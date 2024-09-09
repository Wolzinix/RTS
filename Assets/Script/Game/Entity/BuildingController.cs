using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BuildingController : MonoBehaviour
{
    [SerializeField] private Sprite sprite;
    private Dictionary<GameObject, SpawnTime> entityDictionary;

    public UnityEvent entitySpawnNow = new UnityEvent();

    [SerializeField] private List<GameObject> prefabToSpawn;
    [Serializable] public class SpawnTime {
        public float actualTime;
        public float statsTime;
        public float actualStock;
        public float totalStock;
    }
    public List<SpawnTime> MySpawns = new List<SpawnTime>();
    
    void Start()
    {
        entityDictionary = new Dictionary<GameObject, SpawnTime>();

        if (prefabToSpawn.Count == MySpawns.Count)
        {
            for (int i = 0; i < prefabToSpawn.Count; i++)
            {
                MySpawns[i].actualTime = MySpawns[i].statsTime;
                entityDictionary.Add(prefabToSpawn[i],MySpawns[i]);
            }
        }
    }

    private void Update()
    {
        Physics.SyncTransforms();
        foreach (GameObject i in entityDictionary.Keys)
        {
            if (entityDictionary[i].actualStock < entityDictionary[i].totalStock)
            {
                entityDictionary[i].actualTime -= Time.deltaTime;
            
                if (entityDictionary[i].actualTime <= 0)
                {
                    entityDictionary[i].actualStock += 1; 
                    entityDictionary[i].actualTime = entityDictionary[i].statsTime;
                    entitySpawnNow.Invoke();
                }
            }
        }
    }

    public void SpawnEntity(GameObject entityToSpawn)
    {
        if (entityDictionary[entityToSpawn].actualStock > 0)
        {      
            GameObject newEntity = Instantiate(entityToSpawn,
                new Vector3(transform.position.x+transform.forward.x + 2, transform.position.y+transform.forward.y, transform.position.z+transform.forward.z), 
                transform.rotation);
        
            if (newEntity.GetComponent<EntityController>())
            {
                SetPath(newEntity.GetComponent<EntityController>());
            } 
        
            newEntity.tag = transform.tag;

            entityDictionary[entityToSpawn].actualStock -= 1;
            entitySpawnNow.Invoke();
        }

    }
    
    private void SetPath(EntityController entity)
    {
        entity.AddAggressivePath(new Vector3(transform.position.x+transform.forward.x + 3, transform.position.y+transform.forward.y, transform.position.z+transform.forward.z));
    }

    public Dictionary<GameObject, SpawnTime> GetEntityDictionary() { return entityDictionary; }
}
