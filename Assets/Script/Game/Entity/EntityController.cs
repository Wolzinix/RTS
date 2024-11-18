using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Order
{
    Move,
    Target,
    Follow,
    Patrol,
    Aggressive,
    Harvest,
    Attack,
    Stay,
    Build
}

public class EntityController : MonoBehaviour
{
    protected NavMeshController _navMesh;

    public List<StateClassEntity> _ListOfstate;
    [SerializeField] private ProjectilManager _projectile;

    public List<Order> _ListForOrder;
    private List<GameObject> _listOfalliesOnRange;

    [HideInInspector] public UnityEvent EntityIsArrive = new UnityEvent();
    [HideInInspector] public UnityEvent resetEvent = new UnityEvent();

    public List<SelectableManager> _EnnemieList;

    [HideInInspector] public bool moving = false;
    protected bool _attacking;

    public AggressifEntityManager _entityManager;
    [HideInInspector] public GroupManager groupManager;

    
    [HideInInspector] public Animator _animator;
    public static readonly int Moving = Animator.StringToHash("Mooving");
    public static readonly int Attacking = Animator.StringToHash("Attacking");

    [SerializeField] private SphereCollider _collider;
    private List<GameObject> _ListOfCollision;

    virtual protected void Awake()
    {
        _navMesh = GetComponent<NavMeshController>();
        
        _collider.radius = gameObject.GetComponent<SelectableManager>().SeeRange;

        _ListOfstate = new List<StateClassEntity>();
        _ListForOrder = new List<Order>();

        _listOfalliesOnRange = new List<GameObject>();
        
        _entityManager = GetComponent<AggressifEntityManager>();
        
        _animator = GetComponentInChildren<Animator>();
        _EnnemieList = new List<SelectableManager>();
        GetComponent<AggressifEntityManager>().TakingDamageFromEntity.AddListener(AddAggresseurTarget);
        _ListOfCollision = new List<GameObject>();
        
    }
    private void Start()
    {

        FindAnyObjectByType<FogWarManager>().FogGestion(GetComponent<AggressifEntityManager>(), true);
    }

    virtual protected void LateUpdate()
    {
        if(_ListOfstate.Count > 0) 
        {
            _ListOfstate[0].Update(); 
        }
        if (_navMesh && _ListForOrder.Count == 0 || _ListForOrder.Count != 0 && (_ListForOrder[0] == Order.Patrol || _ListForOrder[0] == Order.Aggressive || _ListForOrder[0] == Order.Follow) || _navMesh == null)
        {
            SearchTarget();
        }
    }
    virtual protected void SearchTarget()
    {
        List<GameObject> listOfAlly = new List<GameObject>();
        List<SelectableManager> listOfennemie = new List<SelectableManager>();

        foreach (GameObject hit in _ListOfCollision)
        {
            hitGestion(hit, listOfAlly, listOfennemie);
        }
        ClearListOfEnnemi(listOfennemie);
        ClearListOfAlly(listOfAlly);

        if (_ListOfstate.Exists(r => r.GetType() == typeof(TargetState)))
        {
            if (_navMesh) { _navMesh.StopPath(); }
        }
        
    }
    private void hitGestion(GameObject hit, List<GameObject> listOfAlly, List<SelectableManager> listOfennemie)
    {
        if (hit.transform && !hit.CompareTag("neutral") && hit.GetComponent<SelectableManager>())
        {
            Debug.DrawLine(transform.position, hit.transform.localPosition, Color.green, 1f);
            GameObject target = hit.transform.gameObject;

            if (target != gameObject && !target.CompareTag(gameObject.tag)) 
            { 
                if(!_EnnemieList.Contains(target.GetComponent<SelectableManager>()))
                {

                    InsertTarget(target.GetComponent<SelectableManager>());
                    _EnnemieList.Add(target.GetComponent<SelectableManager>());
                }
                
                if (!listOfennemie.Contains(target.GetComponent<SelectableManager>()))
                {
                    listOfennemie.Add(target.GetComponent<SelectableManager>());
                }
            }

            if (target != gameObject && target.CompareTag(gameObject.tag))
            {
                if (!_listOfalliesOnRange.Contains(target))
                {
                    target.GetComponent<SelectableManager>().TakingDamageFromEntity.AddListener(InsertTarget);
                    _listOfalliesOnRange.Add(target);
                }
                if (!listOfAlly.Contains(target)) { listOfAlly.Add(target); }
            }
        }
    }
    private void ClearListOfAlly(List<GameObject> list)
    {
        if (list.Count != _listOfalliesOnRange.Count)
        {
            foreach (GameObject i in _listOfalliesOnRange)
            {
                if (!list.Contains(i))
                {
                    if (i) { i.GetComponent<SelectableManager>().TakingDamageFromEntity.RemoveListener(InsertTarget); }
                }
            }
            _listOfalliesOnRange.RemoveAll(i => !list.Contains(i));
        }
    }
    private void ClearListOfEnnemi(List<SelectableManager> list)
    {
        if (list.Count != _EnnemieList.Count){ _EnnemieList.RemoveAll(i => !list.Contains(i)); }
    }

    private void StartFirstOrder()
    {
        if(_ListOfstate.Count == 1) { _ListOfstate[0].Start(); }
    }
    public void RemoveFirstOrder()
    {
        _ListForOrder.RemoveAt(0);
        _ListOfstate.RemoveAt(0);
        if(_ListOfstate.Count > 0) { _ListOfstate[0].Start(); }
    }
   
    public void AddAttackState(SelectableManager target)
    {
        if (_projectile) { _ListOfstate.Insert(0, new AttackState(this, _projectile, target)); }
        else { _ListOfstate.Insert(0, new AttackState(this, target)); }
        _ListForOrder.Insert(0, Order.Attack);
        StartFirstOrder();
    }
    public void AddPath(Vector3 newPath)
    {
        if (_navMesh && Vector3.Distance(gameObject.transform.position, newPath) >= _navMesh.HaveStoppingDistance() + 0.5)
        {
            _ListForOrder.Add(Order.Move);
            _ListOfstate.Add(new MoveState(_navMesh,newPath,this));
            StartFirstOrder();
        }
    }

    public void AddPathInFirst(Vector3 newPath)
    {
        if (_navMesh && Vector3.Distance(gameObject.transform.position, newPath) >= _navMesh.HaveStoppingDistance() + 0.5)
        {
            _ListForOrder.Insert(0,Order.Move);
            _ListOfstate.Insert(0,new MoveState(_navMesh, newPath, this));
            StartFirstOrder();
        }
    }
    public void AddTarget(SelectableManager target )
    {
        _ListForOrder.Add(Order.Target);
        _ListOfstate.Add(new TargetState(target,this,_navMesh));
        StartFirstOrder();

    }
    public void InsertTarget(SelectableManager target)
    {
        _ListOfstate.Insert(0,new TargetState(target, this, _navMesh));
        _ListForOrder.Insert(0, Order.Target);
        StartFirstOrder();
    }
    public void AddAllie(SelectableManager target)
    {
        _ListOfstate.Add(new FollowState(target,_navMesh,this));
        _ListForOrder.Add(Order.Follow);
        StartFirstOrder();
    }
    public void AddPatrol(Vector3 point)
    {
        List<Vector3> destination = new List<Vector3>
            {
                point
            };

        if (_ListForOrder.Count == 0 || (_ListForOrder.Count == 2  && _ListForOrder[1] != Order.Patrol))
        {
           
            _ListOfstate.Add(new PatrolState(destination,_navMesh, this));
            _ListForOrder.Add(Order.Patrol);
            StartFirstOrder();
        }
        else
        {
            if (_ListForOrder.Count == 1 && _ListOfstate[0].GetType() == typeof(PatrolState))
            {
                PatrolState patrol = (PatrolState)_ListOfstate[0];
                patrol.AddDestination(point);
            }
            else if (_ListForOrder.Count == 2 && _ListOfstate[1].GetType() == typeof(PatrolState))
            {
                PatrolState patrol = (PatrolState)_ListOfstate[1];
                patrol.AddDestination(point);
            }
            else
            {
                _ListOfstate.Add(new PatrolState(destination, _navMesh, this));
                _ListForOrder.Add(Order.Patrol);
                StartFirstOrder();
            }
        }
    }
    public void AddAggressivePath(Vector3 newPath)
    {
        _ListOfstate.Add(new AggressifState(_navMesh,newPath,this));
        _ListForOrder.Add(Order.Aggressive);
        StartFirstOrder();
    }
    public void AddStayOrder()
    {
        _ListForOrder.Add(Order.Stay);
        _ListOfstate.Add(new StayState(_navMesh, this));
        StartFirstOrder();
    }
    private void AddAggresseurTarget(AggressifEntityManager entityToAggresse)
    {
        if (_navMesh && _navMesh.notOnTraject() && _ListForOrder.Count == 0 || _ListForOrder.Count != 0 && (_ListForOrder[0] == Order.Patrol || _ListForOrder[0] == Order.Aggressive) || _navMesh == null) { InsertTarget(entityToAggresse); }
    }
    virtual public void ClearAllOrder()
    {
        _ListOfstate.Clear();
        _ListForOrder.Clear();

        
        if(_navMesh){_navMesh.StopPath();}

        ClearListOfAlly(new List<GameObject>());
        resetEvent.Invoke();

    }
    public void SortTarget()
    {
        TargetState nearest = (TargetState)_ListOfstate.Find(x => x.GetType() == typeof(TargetState));
        if(nearest != null)
        {
            foreach (StateClassEntity i in _ListOfstate)
            {
                if (i.GetType() == typeof(TargetState))
                {
                    TargetState c = (TargetState)i;
                    if (c.target)
                    {
                        if (nearest != i)
                        {

                            if (Vector3.Distance(transform.localPosition, nearest.target.gameObject.transform.localPosition) > Vector3.Distance(transform.localPosition, c.target.gameObject.transform.localPosition))
                            {
                                nearest = (TargetState)i;
                            }
                        }
                    }
                   
                }
            }
            _ListOfstate.Remove(nearest);
            InsertTarget(nearest.target);
        }
    }
    public void ChangeSpeed(float speed)
    {
        if(_entityManager.GetType() == typeof(TroupeManager))
        {
            TroupeManager entity = (TroupeManager)_entityManager;
            entity.SetSpeed(speed);
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

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.GetComponent<SelectableManager>() != null)
        {
            _ListOfCollision.Add(collision.gameObject);
            collision.gameObject.GetComponent<SelectableManager>().deathEvent.AddListener(RemoveToCollision);
            SearchTarget();

            if (_EnnemieList.Count > 0) { FindAnyObjectByType<FogWarManager>().FogGestion(GetComponent<AggressifEntityManager>(), false); }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (_ListOfCollision.Contains(collision.gameObject))
        {
            _ListOfCollision.Remove(collision.gameObject);
            collision.gameObject.GetComponent<SelectableManager>().deathEvent.RemoveListener(RemoveToCollision);
            SearchTarget();
            if (_EnnemieList.Count == 0){ FindAnyObjectByType<FogWarManager>().FogGestion(GetComponent<AggressifEntityManager>(),true); }
        }
    }

    private void RemoveToCollision(SelectableManager SM)
    {
        _ListOfCollision.Remove(SM.gameObject);
        SM.deathEvent.RemoveListener(RemoveToCollision);
        SearchTarget();

        bool hide;
        if (_EnnemieList.Count == 0) { hide = true; }
        else  { hide = false; }
        FindAnyObjectByType<FogWarManager>().FogGestion(GetComponent<AggressifEntityManager>(), hide);
    }
}
