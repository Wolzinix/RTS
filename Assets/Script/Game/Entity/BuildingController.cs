using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BuildingController : MonoBehaviour
{
    [SerializeField] private Sprite sprite;
    private Dictionary<GameObject, SpawnTime> entityDictionary;

    public UnityEvent entitySpawnNow = new UnityEvent();

    private float _rangeDetection;

    [SerializeField]  private bool _ally;
    private bool _ennemie;
    private bool _canSpawn;

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
        _rangeDetection = gameObject.GetComponent<EntityManager>().SeeRange;

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

        List<GameObject> ListOfHit= DoCircleRaycast();

        foreach (GameObject i in ListOfHit)
        {
            if(i.CompareTag("Allie"))
            {
                _ally = true;
            }
            else if(i.CompareTag("ennemie"))
            {
                _ennemie = true;
            }
            else
            {
                _ennemie = false;
                _ally = false;
            }
        }
        proximityGestion();
        
    }

    public void AllySpawnEntity(GameObject entityToSpawn)
    {
        if(_ally && !_ennemie)
        {
            SpawnEntity(entityToSpawn);
        }
    }

    private void SpawnEntity(GameObject entityToSpawn)
    {
        if(_canSpawn)
        {
            if (entityDictionary[entityToSpawn].actualStock > 0)
            {
                GameObject newEntity = Instantiate(entityToSpawn,
                    new Vector3(transform.position.x + transform.forward.x + 2, transform.position.y + transform.forward.y, transform.position.z + transform.forward.z),
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
    }

    private void proximityGestion()
    {
        if (!_ally && _ennemie)
        {
            _canSpawn = true;
            foreach (GameObject i in entityDictionary.Keys) { SpawnEntity(i);}
        }
        else if(_ally) { _canSpawn = true; }
        else { _canSpawn = false; }
    }
    
    private void SetPath(EntityController entity)
    {
        entity.AddAggressivePath(new Vector3(transform.position.x+transform.forward.x + 3, transform.position.y+transform.forward.y, transform.position.z+transform.forward.z));
    }

    public Dictionary<GameObject, SpawnTime> GetEntityDictionary() { return entityDictionary; }

    private List<GameObject> DoCircleRaycast()
    {
        float numberOfRay = 30;
        float delta = 360 / numberOfRay;

        List<GameObject> listOfGameObejct = new List<GameObject>();

        for (int i = 0; i < numberOfRay; i++)
        {
            Vector3 dir = Quaternion.Euler(0, i * delta, 0) * transform.forward;

            Ray ray = new Ray(transform.position, dir);
            RaycastHit hit;

            Physics.Raycast(ray, out hit, _rangeDetection);

            if (hit.transform && hit.transform.gameObject.GetComponent<EntityManager>())
            {
                Debug.DrawLine(transform.position, hit.point, Color.green, 1f);
                listOfGameObejct.Add(hit.transform.gameObject);
            }
        }

        return listOfGameObejct;
    }
}
