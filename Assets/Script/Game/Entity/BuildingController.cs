using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.HID;

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
    List<GameObject> ListOfNearEntity;

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
        ListOfNearEntity = new List<GameObject>();
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

        List<RaycastHit> ListOfHit= DoCircleRaycast();
        List<GameObject> ListOfGO = new List<GameObject>();
        
        int nbAllies = 0;
        int nbEnnemie = 0;

        foreach (RaycastHit i in ListOfHit)
        {
            if (i.transform && !i.transform.gameObject.CompareTag("neutral") && i.transform.gameObject.GetComponent<TroupeManager>())
            {
                    
                Debug.DrawLine(transform.position, i.point, Color.red, 1f);

                GameObject gm = i.transform.gameObject;
                ListOfGO.Add(gm);
                tagOfNerestEntity = gm.tag;
                if (gm.CompareTag("Allie"))
                {
                    nbAllies += 1;
                }
                else if (gm.CompareTag("ennemie"))
                {
                    nbEnnemie += 1;
                }
            }
               
        }
        if (!ListOfNearEntity.SequenceEqual(ListOfGO))
        {
            EntityNextToEvent.Invoke(ListOfGO, this);
            if (nbAllies > 0) { _ally = true; }
            else { _ally = false; }

            if (nbEnnemie > 0) { _ennemie = true; }
            else { _ennemie = false; }
        }

        proximityGestion(ListOfGO);

        ListOfNearEntity = ListOfGO;


    }

    public bool GetCanSpawn() { return _canSpawn; }
    public Dictionary<GameObject, SpawnTime> GetEntityDictionary() { return entityDictionary; }
    public void AllySpawnEntity(GameObject entityToSpawn, RessourceController ressource)
    {
        if(_ally && !_ennemie) { SpawnEntity(entityToSpawn, "Allie", DoCircleRaycast()[0].transform.gameObject, ressource); }
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

                    Vector3 pos = calculPostion(spawnrayon,w);

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

    public List<Vector3> SpawnTower(float spawnRadius)
    {
        List<Vector3> ListOfPoint = new List<Vector3>();
        for (int w = 0; w < NbSpawnpoint; w++)
        {
            bool hasTower = false;
            Vector3 pos = calculPostion(spawnRadius, w);
            Collider[] colliders = DoAOverlap(pos,true);
            foreach(Collider collider in colliders)
            {
                if(collider.gameObject.GetComponent<DefenseManager>()) { hasTower = true; }
            }
            if(!hasTower) { ListOfPoint.Add(pos); }

        }
        return ListOfPoint;
    }
    private Vector3 calculPostion(float spawnRadius , int spawnPoint)
    {
        float Theta = 2f * (float)Mathf.PI * ((float)spawnPoint / NbSpawnpoint);


        float x = spawnRadius * Mathf.Cos(Theta);
        float y = spawnRadius * Mathf.Sin(Theta);

        Vector3 pos = new Vector3(x, 1, y);

        pos.x += transform.position.x;
        pos.z += transform.position.z;

        return pos;
    }

    private void SetPath(EntityController entity)
    {
        entity.AddAggressivePath(new Vector3(transform.position.x+transform.forward.x + 3, transform.position.y+transform.forward.y, transform.position.z+transform.forward.z));
    }
    private int DoAOverlap(Vector3 spawnPosition)
    {
        return Physics.OverlapSphere(spawnPosition, 1f).Length;
    }

    private Collider[] DoAOverlap(Vector3 spawnPosition, bool lol)
    {
        return Physics.OverlapSphere(spawnPosition, 1f);
    }

    private List<RaycastHit> DoCircleRaycast()
    {
        float numberOfRay = 40;
        float delta = 360 / numberOfRay;

        List<RaycastHit> listOfGameObejct = new List<RaycastHit>();

        for (int i = 0; i < numberOfRay; i++)
        {
            Vector3 dir = Quaternion.Euler(0, i * delta, 0) * transform.forward;

            Ray ray = new Ray(transform.position, dir);

            listOfGameObejct = listOfGameObejct.Union<RaycastHit>(Physics.RaycastAll(ray, _rangeDetection).ToList()).ToList();
        }

        return listOfGameObejct;
    }

}

