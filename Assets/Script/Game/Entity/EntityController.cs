using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

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

    private List<StateClassEntity> _ListOfstate;
    [SerializeField] private ProjectilManager _projectile;

    public List<Order> _listForOrder;
    protected List<SelectableManager> _listOfAllie;
    protected List<Vector3> _listForAttackingOnTravel;
    private List<GameObject> _listOfalliesOnRange;

    [HideInInspector] public UnityEvent EntityIsArrive = new UnityEvent();
    [HideInInspector] public UnityEvent resetEvent = new UnityEvent();



    [HideInInspector] public bool moving = false;
    protected bool _stayPosition;
    protected bool _attacking;

    public AggressifEntityManager _entityManager;
    [HideInInspector] public GroupManager groupManager;

    
    [HideInInspector] public Animator _animator;
    public static readonly int Moving = Animator.StringToHash("Mooving");
    public static readonly int Attacking = Animator.StringToHash("Attacking");

    [SerializeField] private SphereCollider _collider;
    private List<GameObject> _ListOfCollision;

    void Awake()
    {
        _navMesh = GetComponent<NavMeshController>();
        
        _collider.radius = gameObject.GetComponent<SelectableManager>().SeeRange;

        _ListOfstate = new List<StateClassEntity>();
        _listForOrder = new List<Order>();

        _listOfAllie = new List<SelectableManager>();
        _listForAttackingOnTravel = new List<Vector3>();
        _listOfalliesOnRange = new List<GameObject>();
        
        _entityManager = GetComponent<AggressifEntityManager>();
        
        _animator = GetComponentInChildren<Animator>();

        GetComponent<AggressifEntityManager>().TakingDamageFromEntity.AddListener(AddAggresseurTarget);
        _ListOfCollision = new List<GameObject>();
    }

    virtual protected void LateUpdate()
    {
        if(_ListOfstate.Count > 0)
        {
            _ListOfstate[0].Update();
        }
         isUnit();
    }
    virtual protected void isUnit()
    {
        if (_navMesh && !_navMesh.notOnTraject()) 
        { 
            _animator.SetBool(Moving, true); 
            moving = true; 
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX; 
        }
        else 
        {
            _animator.SetBool(Moving, false); 
            moving = false; 
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX; 
        }


        ExecuteOrder();
    }

    virtual protected void ExecuteOrder()
    {
        if (_listForOrder.Count > 0)
        {

            //if (DoAnAgressionPath()) { }

             if (DoAFollow()) { }

        }
    }
     virtual protected void SearchTarget()
    {
        if(_navMesh && _navMesh.notOnTraject() && _listForOrder.Count == 0 || _listForOrder.Count != 0 && (_listForOrder[0] == Order.Patrol || _listForOrder[0] == Order.Aggressive || _listForOrder[0] == Order.Follow) || _navMesh == null)
        {
            List<GameObject> listOfAlly = new List<GameObject>();

            foreach (GameObject hit in _ListOfCollision)
            {
                hitGestion(hit, listOfAlly);
            }

            ClearListOfAlly(listOfAlly);

            if (_ListOfstate.Exists(r => r.GetType() == typeof(TargetState)))
            {
                if (_navMesh) { _navMesh.StopPath(); }
            }
        }
    }
    private void hitGestion(GameObject hit, List<GameObject> listOfAlly)
    {
        if (hit.transform && !hit.CompareTag("neutral") && hit.GetComponent<SelectableManager>())
        {
            Debug.DrawLine(transform.position, hit.transform.localPosition, Color.green, 1f);
            GameObject target = hit.transform.gameObject;


            if (target != gameObject && !target.CompareTag(gameObject.tag)) { InsertTarget(target.GetComponent<SelectableManager>()); }

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
    private  void ClearListOfAlly(List<GameObject> list)
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

    public void AddAttackState(SelectableManager target)
    {
        if(_projectile) { _ListOfstate.Insert(0, new AttackState(this, _projectile, target)); }
        else{ _ListOfstate.Insert(0, new AttackState(this, target)); }
    }
    public void RemoveFirstOrder()
    {
        _listForOrder.RemoveAt(0);
        _ListOfstate.RemoveAt(0);
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
    
    public void AddPath(Vector3 newPath)
    {
        if (_navMesh && Vector3.Distance(gameObject.transform.position, newPath) >= _navMesh.HaveStoppingDistance() + 0.5)
        {
            _listForOrder.Add(Order.Move);
            _ListOfstate.Add(new MoveState(_navMesh,newPath,this));
        }
    }

    public void AddPathInFirst(Vector3 newPath)
    {
        if (_navMesh && Vector3.Distance(gameObject.transform.position, newPath) >= _navMesh.HaveStoppingDistance() + 0.5)
        {
            _listForOrder.Insert(0,Order.Move);
            _ListOfstate.Insert(0,new MoveState(_navMesh, newPath, this));
        }
    }
    public void AddTarget(SelectableManager target )
    {
        _listForOrder.Add(Order.Target);
        _ListOfstate.Add(new TargetState(target,this,_navMesh));
        
    }
    public void InsertTarget(SelectableManager target)
    {
        _ListOfstate.Insert(0,new TargetState(target, this, _navMesh));
        _listForOrder.Insert(0, Order.Target);
        
    }
    public void AddAllie(SelectableManager target)
    {
        _listOfAllie.Add(target);
        _listForOrder.Add(Order.Follow);
    }
    public void AddPatrol(Vector3 point)
    {
        if (_listForOrder.Count == 0 || (_listForOrder.Count == 2  && _listForOrder[1] != Order.Patrol))
        {
            List<Vector3> destination = new List<Vector3>
            {
                point
            };
            _ListOfstate.Add(new PatrolState(destination,_navMesh, this));
            _listForOrder.Add(Order.Patrol);
        }
        else
        {
            if (_listForOrder.Count == 1 && _ListOfstate[0].GetType() == typeof(PatrolState))
            {
                PatrolState patrol = (PatrolState)_ListOfstate[0];
                patrol.AddDestination(point);
            }
            if (_listForOrder.Count == 2 && _ListOfstate[1].GetType() == typeof(PatrolState))
            {
                PatrolState patrol = (PatrolState)_ListOfstate[1];
                patrol.AddDestination(point);
            }
        }
    }
    public void AddAggressivePath(Vector3 newPath)
    {
        //_listForAttackingOnTravel.Add(newPath);
        _ListOfstate.Add(new AggressifState(_navMesh,newPath,this));
        _listForOrder.Add(Order.Aggressive);
    }
    private void ClearAggressivePath() {_listForAttackingOnTravel.Clear();}
    private void ClearAllFollow(){_listOfAllie.Clear();}
    virtual public void ClearAllOrder()
    {
        _listForOrder.Clear();
        _ListOfstate.Clear();

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
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (_ListOfCollision.Contains(collision.gameObject))
        {
            _ListOfCollision.Remove(collision.gameObject);
            collision.gameObject.GetComponent<SelectableManager>().deathEvent.RemoveListener(RemoveToCollision);
            SearchTarget();
        }
    }

    private void RemoveToCollision(SelectableManager SM)
    {
        _ListOfCollision.Remove(SM.gameObject);
        SM.deathEvent.RemoveListener(RemoveToCollision);
        SearchTarget();
    }
}
