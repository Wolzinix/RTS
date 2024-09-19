using System.Collections.Generic;
using UnityEngine;

public enum Order
{
    Move,
    Target,
    Follow,
    Patrol,
    Aggressive
}

public class EntityController : MonoBehaviour
{
    private NavMeshController _navMesh;

    [SerializeField] private ProjectilManager _projectile;

    public List<Order> _listForOrder;
    private List<Vector3> _listOfPath;
    private List<EntityManager> _listOfTarget;
    private List<EntityManager> _listOfAllie;
    private List<Vector3> _listForPatrol;
    private List<Vector3> _listForAttackingOnTravel;

    private List<GameObject> _listOfalliesOnRange;


    private bool _stayPosition;
    private bool _attacking;
    private int _patrolIteration;

    private EntityManager _entityManager;

    
    private Animator _animator;
    private static readonly int Moving = Animator.StringToHash("Mooving");
    private static readonly int Attacking = Animator.StringToHash("Attacking");
    

    
    
    void Awake()
    {
        _navMesh = GetComponent<NavMeshController>();

        _listOfPath = new List<Vector3>();
        _listOfTarget = new List<EntityManager>();
        _listForOrder = new List<Order>();
        _listOfAllie = new List<EntityManager>();
        _listForPatrol = new List<Vector3>();
        _listForAttackingOnTravel = new List<Vector3>();

        _listOfalliesOnRange = new List<GameObject>();
        
        _entityManager = GetComponent<EntityManager>();
        
        _animator = GetComponentInChildren<Animator>();

        GetComponent<EntityManager>().TakingDamageFromEntity.AddListener(AddAggresseurTarget);
    }

    private List<GameObject> DoCircleRaycast()
    {
        float numberOfRay = 30;
        float delta = 360 / numberOfRay;

        List<GameObject> listOfGameObejct = new List<GameObject>();

        for (int i = 0; i < numberOfRay; i++)
        {
            Vector3 dir = Quaternion.Euler(0, i * delta,0 ) * transform.forward;
            
            Ray ray = new Ray( transform.position,dir);
            RaycastHit hit;
            
            Physics.Raycast(ray, out hit , _entityManager.SeeRange);

            if (hit.transform && !hit.transform.gameObject.CompareTag("neutral") && hit.transform.gameObject.GetComponent<EntityManager>())
            {
                Debug.DrawLine(transform.position, hit.point, Color.green,1f);
                listOfGameObejct.Add( hit.transform.gameObject);
            }
        }

        return listOfGameObejct;
    }

    private void Update()
    {
        Physics.SyncTransforms();
    }

    void FixedUpdate()
    {
         isUnit();
    }

    private void SearchTarget()
    {
        if(_navMesh && !_navMesh.isStillOnTrajet() || _navMesh == null)
        {
            if (_listForOrder.Count == 0  || _listForOrder.Count != 0 && (_listForOrder[0] == Order.Patrol || _listForOrder[0] == Order.Aggressive))
            {
                List<GameObject> listOfRayTuch = DoCircleRaycast();
                List<GameObject> listOfAlly = new List<GameObject>();
                List<GameObject> listToRemove = new List<GameObject>();


                foreach (GameObject target in listOfRayTuch)
                {
                    if (target != gameObject && !_listOfTarget.Contains(target.GetComponent<EntityManager>()) && !target.CompareTag(gameObject.tag))
                    {
                        _listOfTarget.Insert(0, target.GetComponent<EntityManager>());
                        _listForOrder.Insert(0, Order.Target);
                    }
                    if(target != gameObject  && target.CompareTag(gameObject.tag))
                    {
                        if (!_listOfalliesOnRange.Contains(target))
                        {
                            target.GetComponent<EntityManager>().TakingDamageFromEntity.AddListener(AddTarget);
                            _listOfalliesOnRange.Add(target);
                        }
                        listOfAlly.Add(target);
                    }
                }
           
                foreach(GameObject i in _listOfalliesOnRange)
                {
                    if(!listOfAlly.Contains(i))
                    {
                        if(i)
                        {
                            i.GetComponent<EntityManager>().TakingDamageFromEntity.RemoveListener(AddTarget);
                            listToRemove.Add(i);
                        }
                        else
                        {
                            listOfAlly.Remove(i);
                        }
                    }
                }

                foreach(GameObject i in listToRemove)
                {
                    _listOfalliesOnRange.Remove(i);
                }

                if (_listOfTarget.Count > 0)
                {
                    _listOfTarget.Sort(SortTargetByProximity);
                    
                    if(_navMesh)
                    {
                        _navMesh.StopPath();
                    }
                }
                listOfRayTuch.Clear();
            }
        }
    }

    private void AggressTarget()
    {
        if (!_listOfTarget[0])
        {
            _listOfTarget.RemoveAt(0);
            _listForOrder.RemoveAt(0);
        }
        else
        {
            EntityManager target = _listOfTarget[0];

            if (Vector3.Distance(transform.position, target.transform.position) <= _entityManager.Range)
            {
                if(_navMesh)
                {
                    _navMesh.StopPath();
                }
                _animator.SetBool(Moving, false);

                if (!_animator.IsInTransition(0) &&
                    _animator.GetBool(Attacking) &&
                    _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5 &&
                    _attacking)
                {
                    DoAttack(target);
                    _attacking = false;
                }

                if (!_animator.IsInTransition(0) &&
                    _animator.GetBool(Attacking) &&
                    _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)

                { _animator.SetBool(Attacking, false); }

                else{ _animator.SetBool(Attacking, true); }

                if (_animator.IsInTransition(0) && _animator.GetBool(Attacking)) {_attacking = true;}
            }
            else
            {
                if (!_stayPosition && _navMesh){_navMesh.ActualisePath(target);}
            }
        }
    }

    private void isUnit()
    {
        if (_navMesh && !_navMesh.isStillOnTrajet()) { _animator.SetBool(Moving, true); }
        else { _animator.SetBool(Moving, false); }

        if (_listForOrder.Count == 0 || _listForOrder[0] != Order.Target) {_animator.SetBool(Attacking, false);}

        SearchTarget();

        if (_listForOrder.Count > 0)
        {
            if (_listForOrder[0] == Order.Move)
            {
                if (_navMesh && _navMesh.isStillOnTrajet())
                {
                    _navMesh.GetNewPath(_listOfPath[0]);
                    _listOfPath.RemoveAt(0);
                    _listForOrder.RemoveAt(0);
                }
            }

            else if (_listForOrder[0] == Order.Aggressive)
            {
                if (_navMesh && _navMesh.isStillOnTrajet())
                {
                    _navMesh.GetNewPath(_listForAttackingOnTravel[0]);

                    if (_navMesh.isStillOnTrajet())
                    {
                        _listForAttackingOnTravel.RemoveAt(0);
                        _listForOrder.RemoveAt(0);
                    }
                }
            }

            else if (_listForOrder[0] == Order.Patrol)
            {
                if (_navMesh && _navMesh.isStillOnTrajet())
                {
                    if (_patrolIteration == _listForPatrol.Count)
                    {
                        _patrolIteration = 0;
                    }

                    _navMesh.GetNewPath(_listForPatrol[_patrolIteration]);
                    _patrolIteration += 1;
                }
            }

            else if (_listForOrder[0] == Order.Follow)
            {
                if (!_listOfAllie[0])
                {
                    _listOfAllie.RemoveAt(0);
                    _listForOrder.RemoveAt(0);
                }
                else
                {
                    if (_navMesh && !_navMesh.notAtLocation())
                    {
                        _navMesh.ActualisePath(_listOfAllie[0]);
                    }
                }
            }

            else if (_listForOrder[0] == Order.Target)
            {
                AggressTarget();
            }
        }
    }

    void DoAttack(EntityManager target) 
    {
        if(_projectile)
        {
            ProjectilManager pj = Instantiate(_projectile);
            pj.SetDamage(GetComponent<EntityManager>().Attack);
            pj.SetTarget(target.gameObject);
            pj.SetInvoker(gameObject);
            pj.gameObject.transform.position = new Vector3( transform.position.x , transform.position.y +1, transform.position.z);
        }
        else
        {
            _entityManager.DoAttack(target);
        }
        
    }

    public void AddPath(Vector3 newPath)
    {
        _listOfPath.Add(newPath);
        _listForOrder.Add(Order.Move);
    }

    public void AddTarget(EntityManager target)
    {
        _listOfTarget.Add(target);
        _listForOrder.Add(Order.Target);
    }

    public void AddAllie(EntityManager target)
    {
        _listOfAllie.Add(target);
        _listForOrder.Add(Order.Follow);
    }
    
    public void AddPatrol(Vector3 point)
    {
        _listForPatrol.Add(point);
        if (_listForOrder.Count == 0 || _listForOrder[0] != Order.Patrol && _listForOrder[^1] != Order.Patrol)
        {
            _listForOrder.Add(Order.Patrol);
        }
    }
    public void AddAggressivePath(Vector3 newPath)
    {
        _listForAttackingOnTravel.Add(newPath);
        _listForOrder.Add(Order.Aggressive);
    }
    private void ClearAllPath() { _listOfPath.Clear(); }
    private void ClearAllTarget() { _listOfTarget.Clear(); }
    private void ClearPatrol() {_listForPatrol.Clear();}
    private void ClearAggressivePath() {_listForAttackingOnTravel.Clear();}
    private void ClearAllFollow(){_listOfAllie.Clear();}
    public void ClearAllOrder()
    {
        _listForOrder.Clear();
        ClearAllPath();
        ClearAllTarget();
        ClearPatrol();
        ClearAggressivePath();
        ClearAllFollow();
        Stay = false;
        
        if(_navMesh){_navMesh.StopPath();}
    }

    public bool Stay
    {
        set => _stayPosition = value;
    }

    private int SortTargetByProximity(EntityManager entity1, EntityManager entity2)
    {
        if(entity1 == null) return 1;
        if (entity2 == null) return 0;
        return Vector3.Distance(transform.position, entity1.gameObject.transform.position)
            .CompareTo(Vector3.Distance(transform.position, entity2.gameObject.transform.position));
    }

    private void AddAggresseurTarget(EntityManager entityToAggresse)
    {
        if(_listForOrder.Count == 0 && (_navMesh && _navMesh.isStillOnTrajet()|| !_navMesh)){AddTarget(entityToAggresse);}
    }


}
