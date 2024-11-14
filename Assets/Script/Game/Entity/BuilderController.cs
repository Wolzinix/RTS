using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;

public class BuilderController : EntityController
{
    [SerializeField] List<GameObject> _buildings;

    private List<int> ListOfBuildsIndex = new List<int>();
    private List<Vector3> ListOfBuildPosition = new List<Vector3>();
    private List<RessourceManager> _listOfRessource = new List<RessourceManager>();

    [HideInInspector] public UnityEvent<BuilderController> NoMoreToHarvest = new UnityEvent<BuilderController>();
    [HideInInspector] public UnityEvent<BuilderController,DefenseManager> TowerIsBuild = new UnityEvent<BuilderController,DefenseManager> ();

    private RessourceController ressourceController;

    private void Start()
    {
        resetEvent.AddListener(ResetBuildingOrder);
    }
    public void DoAbuildWithRaycast(int nb, RaycastHit hit)
    {
        _ListForOrder.Add(Order.Build);
        _ListOfstate.Add(new BuildState(this, hit.point, _buildings[nb].GetComponent<DefenseManager>()));
    }
    public List<GameObject> getBuildings() { return _buildings; }

    public bool DoAbuild(int nb, Vector3 position, RessourceController ressourcesAvailable)
    {
        ressourceController = ressourcesAvailable;
        if (ressourceController.CompareWood(GetWoodCostOfBuilding(nb)) && ressourceController.CompareGold(GetGoldCostOfBuilding(nb)))
        {
            ResetHarvestOrder();

            _ListForOrder.Add(Order.Build);
            _ListOfstate.Add(new BuildState(this, position, _buildings[nb].GetComponent<DefenseManager>()));
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
        base.LateUpdate();

    }

    void DoAnAttackOnRessource(RessourceManager target)
    {
        _entityManager.DoAttack(target);
    }

    private void SearchClosetHarvestTarget()
    {
        GameObject nextHarvest = DoCircleRaycastForHarvest();
        if (nextHarvest != null)
        {
            AddHarvestTarget(nextHarvest);
        }
        else
        {
            NoMoreToHarvest.Invoke(this);
        }
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
                if (hit.transform &&  hit.transform.gameObject.GetComponent<RessourceManager>())
                {
                    Debug.DrawLine(transform.position, hit.point, Color.green, 1f);
                    if(closet == null)
                    {
                        closet = hit.transform.gameObject;
                    }
                    else if(Vector3.Distance(transform.position,closet.transform.position)> Vector3.Distance(transform.position, hit.transform.position))
                    {
                        closet = hit.transform.gameObject;
                    }
                }
            }
        }
        return closet;
    }
    private void DoHarvest()
    {
        if (!_listOfRessource[0])
        {
            _listOfRessource.RemoveAt(0);   
            _ListForOrder.RemoveAt(0);

            SearchClosetHarvestTarget();
        }
        else
        {
            RessourceManager target = _listOfRessource[0];

            if (Vector3.Distance(gameObject.transform.position, target.transform.position) <= _entityManager.Range + target.size)
            {
                if (_navMesh)
                {
                    _navMesh.StopPath();
                }
                _animator.SetBool(Moving, false);


                if (!_animator.IsInTransition(0) &&
                    _animator.GetInteger(Attacking) == 1 &&
                    _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5 &&
                    _attacking)
                {
                    DoAnAttackOnRessource(target);
                    _attacking = false;
                }

                if (!_animator.IsInTransition(0) &&
                    _animator.GetInteger(Attacking) == 1 &&
                    _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)

                { GetComponentInChildren<Animator>().SetInteger(Attacking, 0); }

                else
                {
                    transform.LookAt(target.transform);
                    GetComponentInChildren<Animator>().SetInteger(Attacking, 1);
                }
                int i = _animator.GetInteger(Attacking);
                if (_animator.IsInTransition(0) && _animator.GetInteger(Attacking) == 1) { _attacking = true; }
            }
            else
            {
                if ( _navMesh) { _navMesh.ActualisePath(target); }
            }
        }
    }

    private bool DoAnHarvest()
    {
        bool etat = false;
        if (_ListForOrder[0] == Order.Harvest)
        {
            etat = true;

            DoHarvest();
        }

        return etat;
    }
    protected override void ExecuteOrder()
    {
        int i = _animator.GetInteger(Attacking);
        if (_ListForOrder.Count > 0)
        {
            DoAnHarvest();
        }
        base.ExecuteOrder();

        i = _animator.GetInteger(Attacking);
        i = 0;
    }
    protected override void SearchTarget(){}

    private void ResetBuildingOrder()
    {
        ListOfBuildPosition.Clear();
        ListOfBuildsIndex.Clear();
    }
    private void ResetHarvestOrder()
    {
        _listOfRessource.Clear();
        _ListForOrder.RemoveAll(x => x == Order.Harvest); 
    }
    public Collider[] DoAOverlap(Vector3 spawnPosition)
    {
        return Physics.OverlapSphere(spawnPosition, 1, ~0, QueryTriggerInteraction.Ignore);
    }

    public void AddHarvestTarget(GameObject hit)
    {
        if (!_listOfRessource.Contains(hit.GetComponent<RessourceManager>()))
        {
            _listOfRessource.Add(hit.GetComponent<RessourceManager>());
            _ListForOrder.Add(Order.Harvest);
        }
    }

    public override void ClearAllOrder()
    {
        base.ClearAllOrder();
        _listOfRessource.Clear();
    }

    public bool BuilderIsAlradyBuilding()
    {
        return ListOfBuildPosition.Count > 0;
    }

    public void PayCostOfBuilding(DefenseManager defense)
    {
        ressourceController.AddGold(-GetGoldCostOfBuilding(defense));
        ressourceController.AddWood(-GetWoodCostOfBuilding(defense));
    }
}
