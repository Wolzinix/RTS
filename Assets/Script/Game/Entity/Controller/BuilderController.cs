using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Events;

public class BuilderController : EntityController
{
    [SerializeField] List<GameObject> _buildings;

    [HideInInspector] public UnityEvent<BuilderController> NoMoreToHarvest = new UnityEvent<BuilderController>();
    [HideInInspector] public UnityEvent<BuilderController, DefenseManager> TowerIsBuild = new UnityEvent<BuilderController, DefenseManager>();

    [SerializeField] LayerMask _IncludeLayer;
    private RessourceController ressourceController;

    [SerializeField] LayerMask _IncludeLayerToSpawn;
    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        resetEvent.AddListener(ResetHarvestOrder);
    }
    public void DoAbuildWithRaycast(int nb, RaycastHit hit)
    {
        _ListOfstate.Add(new BuildState(this, hit.point, _buildings[nb].GetComponent<SelectableManager>()));
    }
    public List<GameObject> getBuildings() { return _buildings; }

    public bool DoAbuild(int nb, Vector3 position, RessourceController ressourcesAvailable)
    {
        ressourceController = ressourcesAvailable;
        if (ressourceController.CompareWood(GetWoodCostOfBuilding(nb)) && ressourceController.CompareGold(GetGoldCostOfBuilding(nb)))
        {
            ResetHarvestOrder();

            _ListOfstate.Add(new BuildState(this, position, _buildings[nb].GetComponent<SelectableManager>()));
            return true;
        }
        return false;
    }

    public int GetWoodCostOfBuilding(int index)
    {
        return _buildings[index].GetComponent<EntityManager>().WoodCost;
    }

    public int GetGoldCostOfBuilding(int index)
    {
        return _buildings[index].GetComponent<EntityManager>().GoldCost;
    }

    public int GetWoodCostOfBuilding(EntityManager index)
    {
        return _buildings.Find(i => i == index.gameObject).GetComponent<EntityManager>().WoodCost;
    }

    public int GetGoldCostOfBuilding(EntityManager index)
    {
        return _buildings.Find(i => i == index.gameObject).GetComponent<EntityManager>().GoldCost;
    }

    protected override void LateUpdate()
    {
        if (_ListOfstate.Count > 0)
        {
            _ListOfstate[0].Update();
        }
        if (_ListOfstate.Count <= 0) { NoMoreToHarvest.Invoke(this); }
    }

    public void SearchClosetHarvestTarget()
    {
        GameObject nextHarvest = DoCircleRaycastForHarvest();
        if (nextHarvest != null) { AddHarvestTarget(nextHarvest); }
        else { NoMoreToHarvest.Invoke(this); }
    }

    private GameObject DoCircleRaycastForHarvest()
    {
        float numberOfRay = 40;
        float delta = 360 / numberOfRay;
        GameObject closet = null;

        for (int i = 0; i < numberOfRay; i++)
        {
            Vector3 dir = Quaternion.Euler(0, i * delta, 0) * transform.forward;

            Ray ray = new Ray(transform.position, dir);
            RaycastHit[] hits;

            hits = Physics.RaycastAll(ray, _entityManager.SeeRange);

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform && hit.transform.gameObject.GetComponent<RessourceManager>())
                {
                    Debug.DrawLine(transform.position, hit.point, Color.green, 1f);
                    if (closet == null)
                    {
                        closet = hit.transform.gameObject;
                    }
                    else if (Vector3.Distance(transform.position, closet.transform.position) > Vector3.Distance(transform.position, hit.transform.position))
                    {
                        closet = hit.transform.gameObject;
                    }
                }
            }
        }
        return closet;
    }
    protected override void SearchTarget() { }

    private void ResetHarvestOrder()
    {
        _ListOfstate.RemoveAll(x => x.GetType() == typeof(HarvestState));
    }
    public Collider[] DoAOverlap(Vector3 spawnPosition)
    {
        return Physics.OverlapSphere(spawnPosition, 1, _IncludeLayer, QueryTriggerInteraction.Ignore);
    }

    public void AddHarvestTarget(GameObject hit)
    {
        bool already = false;
        foreach (StateClassEntity i in _ListOfstate)
        {
            if (i.GetType() == typeof(HarvestState))
            {
                already = true;
            }
        }
        if (!already)
        {
            _ListOfstate.Add(new HarvestState(this, hit.GetComponent<RessourceManager>()));
        }
    }

    public override void ClearAllOrder()
    {
        base.ClearAllOrder();
    }

    public bool BuilderIsAlreadyBuilding()
    {
        foreach (StateClassEntity i in _ListOfstate)
        {
            if (i.GetType() == typeof(BuildState))
            {
                return true;
            }
        }
        return false;
    }

    public void PayCostOfBuilding(SelectableManager defense)
    {
        ressourceController.AddGold(-defense.GetComponent<EntityManager>().GoldCost);
        ressourceController.AddWood(-defense.GetComponent<EntityManager>().WoodCost);
    }

    public void SetRessourceController(RessourceController ressourceController)
        {
        this.ressourceController = ressourceController;
    }

    public Vector3 RayToTuchGround(Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, _IncludeLayerToSpawn))
        {
            Debug.DrawLine(pos, hit.point, Color.red, 10f);
            return new Vector3(pos.x, hit.point.y, pos.z);
        }

        return Vector3.zero;
    }
}
