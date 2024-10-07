using System.Collections.Generic;
using UnityEngine;

public class BuilderController : EntityController
{
    [SerializeField] List<GameObject> _buildings;

    private List<int> ListOfBuildsIndex = new List<int>();
    private List<Vector3> ListOfBuildPosition = new List<Vector3>();
    private List<RessourceManager> _listOfRessource = new List<RessourceManager>();
    public List<GameObject> getBuildings() { return _buildings; }



    private void Start()
    {
        resetEvent.AddListener(ResetBuildingOrder);
    }
    public void DoAbuild(int nb, RaycastHit hit)
    {
        ListOfBuildsIndex.Add(nb);
        ListOfBuildPosition.Add(hit.point);
    }

    private void Update()
    {
        if(ListOfBuildsIndex.Count != 0)
        {

            bool location = gameObject.GetComponent<NavMeshController>().notOnTraject();
            bool distance = Vector3.Distance(gameObject.transform.position, ListOfBuildPosition[0]) <= gameObject.GetComponent<NavMeshController>().HaveStoppingDistance() + 0.5;

            if (location && distance) { Build(); }
            else if(location && !distance)  {  AddPath(ListOfBuildPosition[0]); }

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

    private void DoHarvest()
    {
        if (!_listOfRessource[0])
        {
            _listOfRessource.RemoveAt(0);   
            _listForOrder.RemoveAt(0);
        }
        else
        {
            RessourceManager target = _listOfRessource[0];

            if (Vector3.Distance(transform.position, target.transform.position) <= _entityManager.Range)
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
        if(_buildings[ListOfBuildsIndex[0]].GetComponent<EntityManager>().CanDoIt(GetComponent<TroupeManager>().ressources))
        {
            GetComponent<TroupeManager>().ressources.AddGold(-_buildings[ListOfBuildsIndex[0]].GetComponent<EntityManager>().GoldAmount);
            GetComponent<TroupeManager>().ressources.AddWood(-_buildings[ListOfBuildsIndex[0]].GetComponent<EntityManager>().WoodAmount);
            Collider[] colliders = DoAOverlap(ListOfBuildPosition[0]);

            if (colliders.Length == 1 || (colliders.Length == 2 && (colliders[1] == gameObject || colliders[0] == gameObject)))
            {
                TroupeManager gm = Instantiate(_buildings[ListOfBuildsIndex[0]], ListOfBuildPosition[0] + new Vector3(0, 2, 0), transform.rotation).GetComponent<TroupeManager>();
                gm.gameObject.tag = gameObject.tag;
                gm.ActualiseSprite();
            }
        }
      

        ListOfBuildPosition.RemoveAt(0);
        ListOfBuildsIndex.RemoveAt(0);
    }

    private void ResetBuildingOrder()
    {
        ListOfBuildPosition.Clear();
        ListOfBuildsIndex.Clear();
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
}
