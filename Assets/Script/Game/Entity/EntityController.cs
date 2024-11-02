using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

public enum Order
{
    Move,
    Target,
    Follow,
    Patrol,
    Aggressive,
    Harvest
}

public class EntityController : MonoBehaviour
{
    protected NavMeshController _navMesh;

    [SerializeField] private ProjectilManager _projectile;

    public List<Order> _listForOrder;
    [SerializeField] protected List<Vector3> _listOfPath;
    protected List<SelectableManager> _listOfTarget;
    protected List<SelectableManager> _listOfAllie;
    protected List<Vector3> _listForPatrol;
    protected List<Vector3> _listForAttackingOnTravel;
    private List<GameObject> _listOfalliesOnRange;

    [HideInInspector] public UnityEvent EntityIsArrive = new UnityEvent();
    [HideInInspector] public UnityEvent resetEvent = new UnityEvent();



    [HideInInspector] public bool moving = false;
    protected bool _stayPosition;
    protected bool _attacking;
    private int _patrolIteration;

    protected AggressifEntityManager _entityManager;
    [HideInInspector] public GroupManager groupManager;

    
    [HideInInspector] public Animator _animator;
    protected static readonly int Moving = Animator.StringToHash("Mooving");
    protected static readonly int Attacking = Animator.StringToHash("Attacking");


    void Awake()
    {
        _navMesh = GetComponent<NavMeshController>();

        _listOfPath = new List<Vector3>();
        _listOfTarget = new List<SelectableManager>();
        _listForOrder = new List<Order>();
        _listOfAllie = new List<SelectableManager>();
        _listForPatrol = new List<Vector3>();
        _listForAttackingOnTravel = new List<Vector3>();

        _listOfalliesOnRange = new List<GameObject>();
        
        _entityManager = GetComponent<AggressifEntityManager>();
        
        _animator = GetComponentInChildren<Animator>();

        GetComponent<AggressifEntityManager>().TakingDamageFromEntity.AddListener(AddAggresseurTarget);
    }

    private RaycastHit[] DoCircleRaycast()
    {
        float numberOfRay = 30;
        float delta = 360 / numberOfRay;
        RaycastHit[] hits = null;


        for (int i = 0; i < numberOfRay; i++)
        {
            Vector3 dir = Quaternion.Euler(0, i * delta,0 ) * transform.forward;
            
            Ray ray = new Ray( transform.position,dir);
            

            hits = Physics.RaycastAll(ray, _entityManager.SeeRange);

            return hits;
            
        }

        return hits;
    }

    virtual protected void FixedUpdate()
    {
         isUnit();
    }
    virtual protected void isUnit()
    {
        if (_navMesh && !_navMesh.notOnTraject()) { _animator.SetBool(Moving, true); moving = true; }
        else { _animator.SetBool(Moving, false); moving = false; }

        if (_listForOrder.Count == 0 && _animator.GetInteger(Attacking) == 1) { _animator.SetInteger(Attacking, 0); }

        SearchTarget();

        ExecuteOrder();
    }

    virtual protected void ExecuteOrder()
    {
        if (_listForOrder.Count > 0)
        {
            if (DoAMove()) { }

            else if (DoAnAgressionPath()) { }

            else if (DoAPatrol()) { }

            else if (DoAFollow()) { }

            else if (DoAnAggression()) { }
        }
    }
   

    virtual protected void SearchTarget()
    {
        if(_navMesh && _navMesh.notOnTraject() && _listForOrder.Count == 0 || _listForOrder.Count != 0 && (_listForOrder[0] == Order.Patrol || _listForOrder[0] == Order.Aggressive || _listForOrder[0] == Order.Follow) || _navMesh == null)
        {
            RaycastHit[] listOfRayTuch = DoCircleRaycast();
            List<GameObject> listOfAlly = new List<GameObject>();

            foreach (RaycastHit hit in listOfRayTuch)
            {
                GameObject target;
                if (hit.transform && !hit.transform.gameObject.CompareTag("neutral") && hit.transform.gameObject.GetComponent<SelectableManager>())
                {
                    Debug.DrawLine(transform.position, hit.point, Color.green, 1f);
                    target = hit.transform.gameObject;
                    if (target != gameObject && !target.CompareTag(gameObject.tag)) { InsertTarget(target.GetComponent<SelectableManager>()); }

                    if (target != gameObject && target.CompareTag(gameObject.tag))
                    {
                        if (!_listOfalliesOnRange.Contains(target))
                        {
                            target.GetComponent<SelectableManager>().TakingDamageFromEntity.AddListener(AddTargetAttacked);
                            _listOfalliesOnRange.Add(target);
                        }
                        if (!listOfAlly.Contains(target)) { listOfAlly.Add(target); }

                    }
                }
            }
            ClearListOfAlly(listOfAlly);
        }
    }

    private void ClearListOfAlly(List<GameObject> list)
    {
        List<GameObject> listToRemove = new List<GameObject>();
        foreach (GameObject i in _listOfalliesOnRange)
        {
            if (!list.Contains(i))
            {
                if (i) {  i.GetComponent<SelectableManager>().TakingDamageFromEntity.RemoveListener(AddTargetAttacked);}
                listToRemove.Add(i); 
            }
        }

        foreach (GameObject i in listToRemove) { _listOfalliesOnRange.Remove(i); }

        if (_listOfTarget.Count > 0)
        {
            _listOfTarget.Sort(SortTargetByProximity);

            if (_navMesh) { _navMesh.StopPath(); }
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
            SelectableManager target = _listOfTarget[0];

            if (Vector3.Distance(transform.position, target.transform.position) <= _entityManager.Range + target.size)
            {
                if(_navMesh)
                {
                    _navMesh.StopPath();
                }
                _animator.SetBool(Moving, false);

                if (!_animator.IsInTransition(0) &&
                    _animator.GetInteger(Attacking) == 1 &&
                    _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5 &&
                    _attacking)
                {
                    DoAttack(target);
                    _attacking = false;
                }

                if (!_animator.IsInTransition(0) &&
                   _animator.GetInteger(Attacking) == 1 &&
                    _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)

                { _animator.SetInteger(Attacking, 0); }

                else
                {
                    transform.LookAt(target.transform);
                    _animator.SetInteger(Attacking, 1); 
                }

                if (_animator.IsInTransition(0) && _animator.GetInteger(Attacking) == 1) {_attacking = true;}
            }
            else
            {
                if (!_stayPosition && _navMesh){_navMesh.ActualisePath(target);}
            }
        }
    }

    protected bool DoAMove()
    {
        bool etat = false;
        if (_listForOrder[0] == Order.Move)
        {
            etat = true;
            if (_navMesh && _navMesh.notOnTraject())
            {
                _navMesh.GetNewPath(_listOfPath[0]);
                if (Vector3.Distance(gameObject.transform.position, _listOfPath[0]) <= _navMesh.HaveStoppingDistance() + 0.5)
                {
                    _listOfPath.RemoveAt(0);
                    _listForOrder.RemoveAt(0);
                    EntityIsArrive.Invoke();
                    
                }
            }
        }
        return etat;
    }

    protected bool DoAnAgressionPath()
    {
        bool etat = false;
        if (_listForOrder[0] == Order.Aggressive)
        {
            etat = true;
            if (_navMesh)
            {
                if (_navMesh.notOnTraject())
                {
                    _navMesh.GetNewPath(_listForAttackingOnTravel[0]);
                }
                if (!_navMesh.notOnTraject() && _listForAttackingOnTravel.Count > 0 && Vector3.Distance(gameObject.transform.position, _listForAttackingOnTravel[0]) <= _navMesh.HaveStoppingDistance() + 0.5)
                {
                    _listForAttackingOnTravel.RemoveAt(0);
                    _listForOrder.RemoveAt(0);
                    EntityIsArrive.Invoke();
                }
                if (_navMesh.notOnTraject() && Vector3.Distance(gameObject.transform.position, _listForAttackingOnTravel[0]) <= _navMesh.HaveStoppingDistance() + 0.5)
                {
                    _listForAttackingOnTravel.RemoveAt(0);
                    _listForOrder.RemoveAt(0);
                    EntityIsArrive.Invoke();
                }
            }
        }

        return etat;
    }

    protected bool DoAPatrol()
    {
        bool etat = false;
        bool waitForGroup = false;
        if(groupManager != null)
        {
            waitForGroup = groupManager.EveryOneIsStop();
        }
        if (_listForOrder[0] == Order.Patrol && !waitForGroup)
        {
            etat = true;
            if (_navMesh && _navMesh.notOnTraject())
            {
                if (_patrolIteration == _listForPatrol.Count)
                {
                    _patrolIteration = 0;
                }

                _navMesh.GetNewPath(_listForPatrol[_patrolIteration]);
                _patrolIteration += 1;
            }
        }
        return etat;
    }

    protected bool DoAFollow()
    {
        bool etat = false;
        if (_listForOrder[0] == Order.Follow)
        {
            etat = true;
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

        return etat;
    }

    protected bool DoAnAggression()
    {
        bool etat = false;
        if (_listForOrder[0] == Order.Target)
        {
            etat = true;
            AggressTarget();
        }
        return etat;
    }

  

    void DoAttack(SelectableManager target) 
    {
        if (_projectile)
        {
            ProjectilManager pj = Instantiate(_projectile);
            pj.SetDamage(GetComponent<AggressifEntityManager>().Attack);
            pj.SetTarget(target.gameObject);
            pj.SetInvoker(GetComponent<AggressifEntityManager>());
            pj.gameObject.transform.position = new Vector3( transform.position.x , transform.position.y +1, transform.position.z);
        }
        else {  _entityManager.DoAttack(target); }
    }

    public void AddPath(Vector3 newPath)
    {
        if (_navMesh && Vector3.Distance(gameObject.transform.position, newPath) >= _navMesh.HaveStoppingDistance() + 0.5)
        {
            _listForOrder.Add(Order.Move);
            _listOfPath.Add(newPath);
        }
           
    }

    private void AddTargetAttacked(SelectableManager target)
    {
        InsertTarget(target);
    }

    public void AddTarget(SelectableManager target )
    {
        if(!_listOfTarget.Contains(target))
        {
            _listOfTarget.Add(target);
            _listForOrder.Add(Order.Target);
        }
    }

    public void InsertTarget(SelectableManager target)
    {
        if (!_listOfTarget.Contains(target))
        {
            _listOfTarget.Insert(0,target);
            _listForOrder.Insert(0, Order.Target);
        }
    }

    public void AddAllie(SelectableManager target)
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
    virtual public void ClearAllOrder()
    {
        _listForOrder.Clear();
        ClearAllPath();
        ClearAllTarget();
        ClearPatrol();
        ClearAggressivePath();
        ClearAllFollow();
        Stay = false;
        
        if(_navMesh){_navMesh.StopPath();}

        ClearListOfAlly(new List<GameObject>());
        resetEvent.Invoke();

    }

    public bool Stay
    {
        set => _stayPosition = value;
    }

    private int SortTargetByProximity(SelectableManager entity1, SelectableManager entity2)
    {
        if(entity1 == null) return 1;
        if (entity2 == null) return 0;
        return Vector3.Distance(transform.position, entity1.gameObject.transform.position)
            .CompareTo(Vector3.Distance(transform.position, entity2.gameObject.transform.position));
    }

    private void AddAggresseurTarget(AggressifEntityManager entityToAggresse)
    {
        if (_navMesh && _navMesh.notOnTraject() && _listForOrder.Count == 0 || _listForOrder.Count != 0 && (_listForOrder[0] == Order.Patrol || _listForOrder[0] == Order.Aggressive) || _navMesh == null) { InsertTarget(entityToAggresse);}
    }

    public void ChangeSpeed(float speed)
    {
        if(_entityManager.GetType() == typeof(TroupeManager))
        {
            TroupeManager entity = (TroupeManager)_entityManager;
            entity.SetSpeed(speed);
            _navMesh.SetSpeedWithNavMesh();
        }
    }

    public float GetSpeed()
    {
        if (_entityManager.GetType() == typeof(TroupeManager))
        {
            TroupeManager entity = (TroupeManager)_entityManager;
            return entity.Speed;
        }
        return 0;
    }

    public float GetStartSpeed()
    {
        if (_entityManager.GetType() == typeof(TroupeManager))
        {
            TroupeManager entity = (TroupeManager)_entityManager;
            return entity.StartSpeed;
        }
        return 0;
    }
}
