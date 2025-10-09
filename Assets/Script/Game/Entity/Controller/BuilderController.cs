using Assets.Script.Tools;
using System;
using System.Collections.Generic;
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
        _EntityStateManagement.AddBuildState(this, hit.point, _buildings[nb].GetComponent<SelectableManager>());
    }
    public List<GameObject> GetBuildings() { return _buildings; }

    public bool DoAbuild(int nb, Vector3 position, RessourceController ressourcesAvailable)
    {
        ressourceController = ressourcesAvailable;
        if (ressourceController.CompareWood(GetWoodCostOfBuilding(nb)) && ressourceController.CompareGold(GetGoldCostOfBuilding(nb)))
        {
            ResetHarvestOrder();

            _EntityStateManagement.AddBuildState(this, position, _buildings[nb].GetComponent<SelectableManager>());
            return true;
        }
        return false;
    }

    public int GetWoodCostOfBuilding(int index)
    {
        return _buildings[index].GetComponent<EntityManager>().WoodLoot;
    }

    public int GetGoldCostOfBuilding(int index)
    {
        return _buildings[index].GetComponent<EntityManager>().GoldLoot;
    }

    public int GetWoodCostOfBuilding(EntityManager index)
    {
        return _buildings.Find(i => i == index.gameObject).GetComponent<EntityManager>().WoodLoot;
    }

    public int GetGoldCostOfBuilding(EntityManager index)
    {
        return _buildings.Find(i => i == index.gameObject).GetComponent<EntityManager>().GoldLoot;
    }

    protected override void LateUpdate()
    {
        _EntityStateManagement.Update();
        if (_EntityStateManagement.GetLenghtOfState() <= 0) { NoMoreToHarvest.Invoke(this); }
    }

    private GameObject DoCircleRaycastForHarvest()
    {
        Collider[] hits = RayCast.DoASphereOverlap(gameObject.transform.position,_entityManager.SeeRange);
        
        GameObject closet = null;

        foreach (Collider hit in hits)
        {
            if (hit.transform && hit.transform.gameObject.GetComponent<RessourceManager>())
            {
                Debug.DrawLine(transform.position, hit.transform.position, Color.green, 1f);
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
        
        return closet;
    }
    public void SearchClosetHarvestTarget()
    {
        GameObject nextHarvest = DoCircleRaycastForHarvest();
        if (nextHarvest != null) { AddHarvestTarget(nextHarvest); }
        else { NoMoreToHarvest.Invoke(this); }
    }
    protected override void SearchTarget() { }

    private void ResetHarvestOrder()
    {
        _EntityStateManagement.ClearAllOrderOfType(typeof(HarvestState));
    }
    public Collider[] DoAOverlap(Vector3 spawnPosition)
    {
        return Physics.OverlapSphere(spawnPosition, 1, _IncludeLayer, QueryTriggerInteraction.Ignore);
    }

    public void AddHarvestTarget(GameObject hit)
    {
        _EntityStateManagement.AddHarvestTarget(hit,this);
    }

    public override void ClearAllOrder()
    {
        base.ClearAllOrder();
    }

    public void PayCostOfBuilding(SelectableManager defense)
    {
        ressourceController.AddGold(-defense.GetComponent<EntityManager>().GoldLoot);
        ressourceController.AddWood(-defense.GetComponent<EntityManager>().WoodLoot);
    }

    public void SetRessourceController(RessourceController ressourceController)
    {
        this.ressourceController = ressourceController;
    }
}
