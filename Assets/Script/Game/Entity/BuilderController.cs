using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BuilderController : EntityController
{
    [SerializeField] List<GameObject> _buildings;

    private List<int> ListOfBuildsIndex = new List<int>();
    private List<Vector3> ListOfBuildPosition = new List<Vector3>();
    private List<RessourceManager> _listOfRessource = new List<RessourceManager>();
    public List<GameObject> getBuildings() { return _buildings; }

    public UnityEvent<BuilderController> NoMoreToHarvest = new UnityEvent<BuilderController>();
    public UnityEvent<BuilderController,DefenseManager> TowerIsBuild = new UnityEvent<BuilderController,DefenseManager> ();

    private void Start()
    {
        resetEvent.AddListener(ResetBuildingOrder);
    }
    public void DoAbuildWithRaycast(int nb, RaycastHit hit)
    {
        ListOfBuildsIndex.Add(nb);
        ListOfBuildPosition.Add(hit.point);
    }

    public bool DoAbuild(int nb, Vector3 position, RessourceController ressourcesAvailable)
    {
        if(ressourcesAvailable.CompareWood(GetWoodCostOfBuilding(nb)) && ressourcesAvailable.CompareGold(GetGoldCostOfBuilding(nb)))
        {
            ResetHarvestOrder();
            ListOfBuildsIndex.Add(nb);
            ListOfBuildPosition.Add(position);
            ressourcesAvailable.AddGold(GetGoldCostOfBuilding(nb));
            ressourcesAvailable.AddWood(GetWoodCostOfBuilding(nb));
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

    private void Update()
    {
        if (ListOfBuildsIndex.Count != 0)
        {

            bool location = gameObject.GetComponent<NavMeshController>().notOnTraject();
            bool distance =
                Vector3.Distance(gameObject.transform.position, ListOfBuildPosition[0]) 
                <=
                gameObject.GetComponent<NavMeshController>().HaveStoppingDistance() + _buildings[ListOfBuildsIndex[0]].GetComponentInChildren<Renderer>().bounds.size.x + _buildings[ListOfBuildsIndex[0]].GetComponentInChildren<Renderer>().bounds.size.y;

            if (location && distance) { Build(); }
            else if (location && !distance) { AddPath(ListOfBuildPosition[0]); }

        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

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
            _listForOrder.RemoveAt(0);

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
                if (!_stayPosition && _navMesh) { _navMesh.ActualisePath(target); }
            }
        }
    }

    private bool DoAnHarvest()
    {
        bool etat = false;
        if (_listForOrder[0] == Order.Harvest)
        {
            etat = true;

            DoHarvest();
        }

        return etat;
    }
    protected override void ExecuteOrder()
    {
        int i = _animator.GetInteger(Attacking);
        if (_listForOrder.Count > 0)
        {
            DoAnHarvest();
        }
        base.ExecuteOrder();

        i = _animator.GetInteger(Attacking);
        i = 0;
    }

    private void Build()
    {
        if(_buildings[ListOfBuildsIndex[0]].GetComponent<EntityManager>().CanDoIt(GetComponent<AggressifEntityManager>().ressources))
        {
            
            Collider[] colliders = DoAOverlap(ListOfBuildPosition[0]);

            if (colliders.Length == 0 || colliders.Length == 1 && colliders[0].gameObject.GetComponent<EntityManager>() == null || (colliders.Length == 2 && (colliders[1] == gameObject || colliders[0] == gameObject)))
            {
                DefenseManager gm = Instantiate(_buildings[ListOfBuildsIndex[0]], new Vector3(ListOfBuildPosition[0].x, gameObject.transform.position.y, ListOfBuildPosition[0].z), transform.rotation, transform.parent).GetComponent<DefenseManager>();
                GetComponent<AggressifEntityManager>().ressources.AddGold(-_buildings[ListOfBuildsIndex[0]].GetComponent<EntityManager>().GoldAmount);
                GetComponent<AggressifEntityManager>().ressources.AddWood(-_buildings[ListOfBuildsIndex[0]].GetComponent<EntityManager>().WoodAmount);
                gm.gameObject.tag = gameObject.tag;
                TowerIsBuild.Invoke(this,gm);
                gm.ActualiseSprite();
                ListOfBuildPosition.RemoveAt(0);
                ListOfBuildsIndex.RemoveAt(0);
            }
            else
            {
                foreach(Collider i in colliders)
                {
                    if(i.GetComponent<EntityController>() != null)
                    {
                        Vector3 iPosition = i.GetComponent<EntityController>().transform.position;
                        i.GetComponent<EntityController>().AddPath(iPosition - (iPosition - ListOfBuildPosition[0] - _buildings[ListOfBuildsIndex[0]].GetComponentInChildren<Renderer>().bounds.size) * 2);
                    }
                }
            }
        }
        else
        {
            ListOfBuildPosition.RemoveAt(0);
            ListOfBuildsIndex.RemoveAt(0);
        }
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
        _listForOrder.RemoveAll(x => x == Order.Harvest); 
    }
    private Collider[] DoAOverlap(Vector3 spawnPosition)
    {
        return Physics.OverlapSphere(spawnPosition, 1);
    }

    public void AddHarvestTarget(GameObject hit)
    {
        if (!_listOfRessource.Contains(hit.GetComponent<RessourceManager>()))
        {
            _listOfRessource.Add(hit.GetComponent<RessourceManager>());
            _listForOrder.Add(Order.Harvest);
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
}
