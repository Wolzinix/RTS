using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class BuildingController : MonoBehaviour
{
    private Dictionary<GameObject, SpawnTime> entityDictionary;

    public UnityEvent entitySpawnNow = new UnityEvent();

    public UnityEvent entityAsBeenBuy = new UnityEvent();

    public UnityEvent<List<GameObject>,BuildingController> EntityNextToEvent = new UnityEvent<List<GameObject>, BuildingController>();

    private float _rangeDetection;

    private bool _ally;
    private bool _ennemie;
    private bool _canSpawn;

    public string tagOfNerestEntity;

    int NbSpawnpoint = 10;
    public float spawnrayon = 2f;
    public LineRenderer lineRenderer;

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
        _rangeDetection = gameObject.GetComponent<BuildingManager>().SeeRange;

        if (prefabToSpawn.Count == MySpawns.Count)
        {
            for (int i = 0; i < prefabToSpawn.Count; i++)
            {
                MySpawns[i].actualTime = MySpawns[i].statsTime;
                entityDictionary.Add(prefabToSpawn[i], MySpawns[i]);
            }
        }
    }

    private void FixedUpdate()
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
        EntityNextToEvent.Invoke(ListOfHit,this);
        int nbAllies = 0;
        int nbEnnemie = 0;

        foreach (GameObject i in ListOfHit)
        {
            tagOfNerestEntity = i.tag;
            if(i.CompareTag("Allie"))
            {
                nbAllies += 1;
            }
            else if(i.CompareTag("ennemie"))
            {
                nbEnnemie += 1;
            }
        }

        if(nbAllies > 0 ) { _ally = true; }
        else{ _ally = false; }

        if (nbEnnemie > 0) { _ennemie = true; }
        else { _ennemie = false; }


        proximityGestion(ListOfHit);
        
    }

    public bool GetCanSpawn() { return _canSpawn; }
    public Dictionary<GameObject, SpawnTime> GetEntityDictionary() { return entityDictionary; }
    public void AllySpawnEntity(GameObject entityToSpawn, RessourceController ressource)
    {
        if(_ally && !_ennemie) { SpawnEntity(entityToSpawn, "Allie", DoCircleRaycast()[0], ressource); }
    }

    private void proximityGestion(List<GameObject> list)
    {
        if (!_ally && _ennemie)
        {
            _canSpawn = true;
            foreach (GameObject i in entityDictionary.Keys) { SpawnEntity(i, "ennemie", list[0],FindAnyObjectByType<IABrain>().GetComponent<RessourceController>()); }
        }
        else if (_ally) { _canSpawn = true; }
        else { _canSpawn = false; }
    }

    private void SpawnEntity(GameObject entityToSpawn, string tag, GameObject entity, RessourceController ressource)
    {
        if(ressource.CompareGold(entityToSpawn.GetComponent<EntityManager>().GoldCost) && _canSpawn && (transform.CompareTag(tag) || transform.CompareTag("neutral")))
        {
            if (entityDictionary[entityToSpawn].actualStock > 0)
            {

                if (lineRenderer != null)
                { lineRenderer.positionCount = NbSpawnpoint; }

                for (int w = 0; w < NbSpawnpoint; w++)
                {
                    
                    float Theta = 2f * (float)Mathf.PI * ((float)w / NbSpawnpoint);


                    float x = spawnrayon * Mathf.Cos(Theta);
                    float y = spawnrayon * Mathf.Sin(Theta);

                    Vector3 pos = new Vector3(x, 1, y);

                    pos.x += transform.position.x;
                    pos.z += transform.position.z;

                    if (lineRenderer != null)
                    { lineRenderer.SetPosition(w, pos); }
                        

                    int colliders = DoAOverlap(pos);
                    if (colliders == 1)
                    {
                        GameObject newEntity = Instantiate(entityToSpawn, pos, transform.rotation, entity.transform.parent);

                        newEntity.tag = tag;

                        newEntity.GetComponent<AggressifEntityManager>().ActualiseSprite();

                        entityDictionary[entityToSpawn].actualStock -= 1;
                        entitySpawnNow.Invoke();
                        entityAsBeenBuy.Invoke();
                        ressource.AddGold(-entityToSpawn.GetComponent<EntityManager>().GoldCost);
                        break;
                    }
                }
            }
        }
        
    }

    private void SetPath(EntityController entity)
    {
        entity.AddAggressivePath(new Vector3(transform.position.x+transform.forward.x + 3, transform.position.y+transform.forward.y, transform.position.z+transform.forward.z));
    }
    private int DoAOverlap(Vector3 spawnPosition)
    {
        return Physics.OverlapSphere(spawnPosition, 1f).Length;
    }

    private List<GameObject> DoCircleRaycast()
    {
        float numberOfRay = 40;
        float delta = 360 / numberOfRay;

        List<GameObject> listOfGameObejct = new List<GameObject>();

        for (int i = 0; i < numberOfRay; i++)
        {
            Vector3 dir = Quaternion.Euler(0, i * delta, 0) * transform.forward;

            Ray ray = new Ray(transform.position, dir);
            RaycastHit[] hits;

            hits = Physics.RaycastAll(ray, _rangeDetection);

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform && !hit.transform.gameObject.CompareTag("neutral") && hit.transform.gameObject.GetComponent<AggressifEntityManager>())
                {
                    Debug.DrawLine(transform.position, hit.point, Color.red, 1f);
                    listOfGameObejct.Add(hit.transform.gameObject);
                }
            }
        }

        return listOfGameObejct;
    }
}
