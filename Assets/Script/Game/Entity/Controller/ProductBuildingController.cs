using Assets.Script.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProductBuildingController : MonoBehaviour
{
    [Serializable]
    public class SpawnTime
    {
        public float actualTime;
        public float statsTime;
        public float actualStock;
        public float totalStock;
    }


    public string tagOfNerestEntity;
    public float spawnrayon = 2f;
    public LineRenderer lineRenderer;
    public List<SpawnTime> MySpawns = new ();

    [HideInInspector] public UnityEvent entitySpawnNow = new ();
    [HideInInspector] public UnityEvent<ProductBuildingController, GameObject> entityCanSpawnNow = new();
    [HideInInspector] public UnityEvent entityAsBeenBuy = new ();
    [HideInInspector] public UnityEvent<List<GameObject>, ProductBuildingController> EntityNextToEvent = new ();
    [HideInInspector] public List<GameObject> ListOfNearEntity = new();

    [SerializeField] private List<GameObject> prefabToSpawn;

    private TextChanger _textForInfo;
    private bool _ally;
    private bool _ennemie;
    private bool _canSpawn;
    private Dictionary<GameObject, SpawnTime> _entityDictionary = new();
    private int _nbSpawnpoint = 10;
    private SphereCollider _sphereCollider;
    private int _nbAllies = 0;
    private int _nbEnnemie = 0;

    void Start()
    {
        _sphereCollider = GetComponentInChildren<SphereCollider>();
        _sphereCollider.radius = gameObject.GetComponent<ProductionBuildingManager>().SeeRange;

        _textForInfo = FindAnyObjectByType<TextChanger>();

        if (prefabToSpawn.Count == MySpawns.Count)
        {
            for (int i = 0; i < prefabToSpawn.Count; i++)
            {
                MySpawns[i].actualTime = MySpawns[i].statsTime;
                _entityDictionary.Add(prefabToSpawn[i], MySpawns[i]);
            }
        }

        MeshRenderer meshrender = GetComponentInChildren<MeshRenderer>();

        if (meshrender == null)
        {
            SkinnedMeshRenderer render = GetComponentInChildren<SkinnedMeshRenderer>();
            spawnrayon += (render.bounds.size.x + render.bounds.size.z) / 2;
        }
        else
        {
            spawnrayon += (meshrender.bounds.size.x + meshrender.bounds.size.z) / 2;
        }
    }
    private void ReduceTimer()
    {
        foreach (GameObject i in _entityDictionary.Keys)
        {
            if (_entityDictionary[i].actualStock < _entityDictionary[i].totalStock)
            {
                _entityDictionary[i].actualTime -= Time.deltaTime;

                if (_entityDictionary[i].actualTime <= 0)
                {
                    _entityDictionary[i].actualStock += 1;
                    _entityDictionary[i].actualTime = _entityDictionary[i].statsTime;
                    entitySpawnNow.Invoke();
                    entityCanSpawnNow.Invoke(this, i);
                }
            }
        }
    }
    private void ProximityGestion()
    {
        if (!_ally && _ennemie) { tagOfNerestEntity = "ennemie"; _canSpawn = true; }
        else if (_ally && !_ennemie) { tagOfNerestEntity = "Allie"; _canSpawn = true; }
        else { tagOfNerestEntity = ""; _canSpawn = false; }
    }
    private void LateUpdate()
    {
        if(this)
        {
            ReduceTimer();
            ProximityGestion();
        }
    }

    public bool GetCanSpawn() { return _canSpawn; }
    public Dictionary<GameObject, SpawnTime> GetEntityDictionary() { return _entityDictionary; }
    private bool TextGestion(GameObject entityToSpawn, RessourceController ressource)
    {
        EntityManager entityManager = entityToSpawn.GetComponent<EntityManager>();
        if (ListOfNearEntity.Count == 0)
        {
            _textForInfo.SetText("Aucune entité a proximiter");
            return false;
        }
        else if (!_canSpawn)
        {
            _textForInfo.SetText("Zone Contester");
            return false;
        }
        else if (!ressource.CompareGold(entityManager.GoldLoot) ||
            !ressource.CompareWood(entityManager.WoodLoot)
            )
        {
            _textForInfo.SetText("Pas Assez De Ressource");
            return false;
        }
        else if (_entityDictionary[entityToSpawn].actualStock == 0)
        {
            _textForInfo.SetText("Pas Encore Disponible");
            return false;
        }
        else if(_canSpawn && !_ally) 
        {
            _textForInfo.SetText("Controler par l'ennemie");
            return false;
        }
        return true;
    }
    public void SpawnEntity(GameObject entityToSpawn, string tag, GameObject entity, RessourceController ressource)
    {
        EntityManager entityManager = entityToSpawn.GetComponent<EntityManager>();
        if (this &&
            ressource.CompareRessource(entityManager.WoodLoot, entityManager.GoldLoot) &&
            _canSpawn &&
            (transform.CompareTag(tag) || transform.CompareTag("neutral")) &&
            _entityDictionary[entityToSpawn].actualStock > 0)
        {
            for (int w = 0; w < _nbSpawnpoint; w++)
            {
                Vector3 pos = CalculPostion(spawnrayon, w);

                pos = RayCast.RaycastOnNavMeshGround(pos, spawnrayon);

                if (pos != transform.position)
                {
                    int colliders = RayCast.DoASphereOverlap(pos, LayerMask.GetMask("Default")).Length;
                    if (colliders <= 0)
                    {
                        GameObject newEntity = Instantiate(entityToSpawn, pos, transform.rotation, entity.transform.parent);

                        newEntity.name = NameIndex.GetARandomName();
                        newEntity.tag = tag;

                        newEntity.GetComponent<AggressifEntityManager>().ActualiseSprite();

                        _entityDictionary[entityToSpawn].actualStock -= 1;
                        entitySpawnNow.Invoke();
                        entityAsBeenBuy.Invoke();
                        ressource.AddGold(-entityManager.GoldLoot);
                        return;
                    }
                }
                if (w == _nbSpawnpoint - 1 && _ally)
                {
                    _textForInfo.SetText("Zone Obstrue");
                }
            }
        }
    }
    public void AllySpawnEntity(GameObject entityToSpawn, RessourceController ressource)
    {
        if (TextGestion(entityToSpawn, ressource)) { SpawnEntity(entityToSpawn, ListOfNearEntity[0].tag, ListOfNearEntity[0], ressource); }
    }
    public void SpawnEveryEntity(string tag, GameObject entity, RessourceController ressource)
    {
        foreach (GameObject i in _entityDictionary.Keys) { SpawnEntity(i, tag, entity, ressource); }
    }
    
    private Vector3 CalculPostion(float spawnRadius, int spawnPoint)
    {
        float Theta = 2f * Mathf.PI * ((float)spawnPoint / _nbSpawnpoint);

        float x = spawnRadius * Mathf.Cos(Theta);
        float y = spawnRadius * Mathf.Sin(Theta);

        Vector3 pos = new(x, transform.position.y, y);

        pos.x += transform.position.x;
        pos.z += transform.position.z;

        return pos;
    }
    public List<Vector3> SpawnTower(float spawnRadius)
    {
        List<Vector3> ListOfPoint = new List<Vector3>();
        for (int w = 0; w < _nbSpawnpoint; w++)
        {
            bool hasTower = false;
            Vector3 pos = CalculPostion(spawnRadius, w);
            Collider[] colliders = RayCast.DoASphereOverlap(pos,LayerMask.GetMask("Default"));
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.GetComponent<DefenseManager>()) { hasTower = true; }
            }
            if (!hasTower) { ListOfPoint.Add(pos); }
        }
        return ListOfPoint;
    }
    
    private void EntityProximityDeath(SelectableManager entity)
    {
        if (entity.CompareTag("Allie")) { _nbAllies -= 1; }
        else if (entity.CompareTag("ennemie")) { _nbEnnemie -= 1; }

        ListOfNearEntity.Remove(entity.gameObject);
        TagGestion();
    }

    private void TagGestion()
    {
        if (_nbAllies > 0) { _ally = true; }
        else { _ally = false; }

        if (_nbEnnemie > 0) { _ennemie = true; }
        else { _ennemie = false; }
    }
    private void AddCollider(GameObject go)
    {
        TroupeManager goManager = go.transform.gameObject.GetComponent<TroupeManager>();
        if (go.transform && !go.transform.gameObject.CompareTag("neutral") && goManager)
        {
            Debug.DrawLine(transform.position, go.transform.position, Color.red, 1f);
            if (!ListOfNearEntity.Contains(go))
            {
                if (go.CompareTag("Allie")) { _nbAllies += 1; }
                else if (go.CompareTag("ennemie")) { _nbEnnemie += 1; }

                ListOfNearEntity.Add(go);
                TagGestion();

                EntityNextToEvent.Invoke(ListOfNearEntity, this);
                goManager.deathEvent.AddListener(EntityProximityDeath);
            }
        }
    }

    private void RemoveCollider(GameObject go)
    {
        TroupeManager goManager = go.transform.gameObject.GetComponent<TroupeManager>();
        if (go.transform && !go.transform.gameObject.CompareTag("neutral") && goManager)
        {
            if (ListOfNearEntity.Contains(go))
            {
                if (go.CompareTag("Allie")) { _nbAllies -= 1; }
                else if (go.CompareTag("ennemie")) { _nbEnnemie -= 1; }

                ListOfNearEntity.Remove(go);
                TagGestion();

                EntityNextToEvent.Invoke(ListOfNearEntity, this);
                goManager.deathEvent.RemoveListener(EntityProximityDeath);

            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        AddCollider(collision.gameObject);
    }

    private void OnTriggerExit(Collider collision)
    {
        RemoveCollider(collision.gameObject);
    }
}

