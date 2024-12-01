using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityController : BuildingController
{
    protected NavMeshController _navMesh;

    public List<StateClassEntity> _ListOfstate;
    [SerializeField] private ProjectilManager _projectile;


    [HideInInspector] public UnityEvent EntityIsArrive = new UnityEvent();
    [HideInInspector] public UnityEvent resetEvent = new UnityEvent();


    [HideInInspector] public bool moving = false;
    protected bool _attacking;

    [HideInInspector] public Animator _animator;
    public static readonly int Moving = Animator.StringToHash("Mooving");
    public static readonly int Attacking = Animator.StringToHash("Attacking");

    override protected void Awake()
    {
        base.Awake();

        _navMesh = GetComponent<NavMeshController>();
        _ListOfstate = new List<StateClassEntity>();

        _animator = GetComponentInChildren<Animator>();
        GetComponent<SelectableManager>().TakingDamageFromEntity.AddListener(AddAggresseurTarget);

    }
    override protected void LateUpdate()
    {
        if (_ListOfstate.Count > 0)
        {
            _ListOfstate[0].Update();
        }
        if (_navMesh && _ListOfstate.Count == 0 || _ListOfstate.Count != 0 && (_ListOfstate[0].GetType() == typeof(PatrolState) || _ListOfstate[0].GetType() == typeof(AggressifState) || _ListOfstate[0].GetType() == typeof(FollowState) || _navMesh == null))
        {
            if(_ListOfstate.Count == 0 || _ListOfstate.Count != 0 && _ListOfstate[0].GetType() != typeof(StuntState))
            {
                SearchTarget();
            }
        }
    }
    override protected void SearchTarget()
    {
        base.SearchTarget();

        if (_ListOfstate.Count == 0 || _ListOfstate.Exists(r => r.GetType() == typeof(TargetState)))
        {
            if (_navMesh) { _navMesh.StopPath(); }
        }

    }
    override protected void AddEnnemi(SelectableManager target)
    {
        if (_ListOfstate.Count == 0 || _ListOfstate.Count != 0 && _ListOfstate[0].GetType() != typeof(StuntState))
        {
            base.AddEnnemi(target);
            InsertTarget(target);
        }
    }

    protected override void ClearListOfAlly(List<GameObject> list)
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

            base.ClearListOfAlly(list);
        }
    }

    private void StartFirstOrder()
    {
        if (_ListOfstate.Count == 1) { _ListOfstate[0].Start(); }
    }
    public void RemoveFirstOrder()
    {
        _ListOfstate.RemoveAt(0);
        if (_ListOfstate.Count > 0) { _ListOfstate[0].Start(); }
    }

    public void AddAttackState(SelectableManager target)
    {
        if (_ListOfstate.Count == 0 || _ListOfstate.Count != 0 && _ListOfstate[0].GetType() != typeof(StuntState))
        {
            if (_projectile) { _ListOfstate.Insert(0, new AttackState(this, _projectile, target)); }
            else { _ListOfstate.Insert(0, new AttackState(this, target)); }
            StartFirstOrder();
        }
    }
    public void AddPath(Vector3 newPath)
    {
        if (_navMesh && Vector3.Distance(gameObject.transform.position, newPath) >= _navMesh.HaveStoppingDistance() + 0.5)
        {
            _ListOfstate.Add(new MoveState(_navMesh, newPath, this));
            StartFirstOrder();
        }
    }

    public void AddPathInFirst(Vector3 newPath)
    {
        if (_ListOfstate.Count == 0 || _ListOfstate.Count != 0 && _ListOfstate[0].GetType() != typeof(StuntState))
        {
            if (_navMesh && Vector3.Distance(gameObject.transform.position, newPath) >= _navMesh.HaveStoppingDistance() + 0.5)
            {
                _ListOfstate.Insert(0, new MoveState(_navMesh, newPath, this));
                StartFirstOrder();
            }
        }
    }
    public void AddTarget(SelectableManager target)
    {
        _ListOfstate.Add(new TargetState(target, this, _navMesh));
        StartFirstOrder();
    }
    public void InsertTarget(SelectableManager target)
    {
        if (_ListOfstate.Count == 0 || _ListOfstate.Count != 0 && _ListOfstate[0].GetType() != typeof(StuntState))
        {
            _ListOfstate.Insert(0, new TargetState(target, this, _navMesh));
            StartFirstOrder();
        }
    }
    public void AddAllie(SelectableManager target)
    {
        _ListOfstate.Add(new FollowState(target, _navMesh, this));
        StartFirstOrder();
    }

    public void ClearAllOrderOfType(Type type)
    {
        _ListOfstate.RemoveAll(x => x.GetType() == type);
    }
    public void AddPatrol(Vector3 point)
    {
        List<Vector3> destination = new List<Vector3>
            {
                point
            };

        if (_ListOfstate.Count == 0 || (_ListOfstate.Count == 2 && _ListOfstate[1].GetType() != typeof(PatrolState)))
        {
            _ListOfstate.Add(new PatrolState(destination, _navMesh, this));
            StartFirstOrder();
        }
        else
        {
            if (_ListOfstate.Count == 1 && _ListOfstate[0].GetType() == typeof(PatrolState))
            {
                PatrolState patrol = (PatrolState)_ListOfstate[0];
                patrol.AddDestination(point);
            }
            else if (_ListOfstate.Count == 2 && _ListOfstate[1].GetType() == typeof(PatrolState))
            {
                PatrolState patrol = (PatrolState)_ListOfstate[1];
                patrol.AddDestination(point);
            }
            else
            {
                _ListOfstate.Add(new PatrolState(destination, _navMesh, this));
                StartFirstOrder();
            }
        }
    }
    public void AddAggressivePath(Vector3 newPath)
    {
        _ListOfstate.Add(new AggressifState(_navMesh, newPath, this));
        StartFirstOrder();
    }
    public void AddStayOrder()
    {
        _ListOfstate.Add(new StayState(_navMesh, this));
        StartFirstOrder();
    }
    public void AddStayOrderAtFirst()
    {
        if (_ListOfstate.Count == 0 || _ListOfstate.Count != 0 && _ListOfstate[0].GetType() != typeof(StuntState))
        {
            _ListOfstate.Insert(0, new StayState(_navMesh, this));
            StartFirstOrder();
        }
    }

    public void AddStuntOrder()
    {
        if (_ListOfstate.Count == 0 || _ListOfstate.Count != 0 && _ListOfstate[0].GetType() != typeof(StuntState))
        {
            _ListOfstate.Insert(0, new StuntState(_navMesh, this));
            StartFirstOrder();
        }
    }
    private void AddAggresseurTarget(AggressifEntityManager entityToAggresse)
    {
        if (_navMesh && _navMesh.notOnTraject() && _ListOfstate.Count == 0 || _ListOfstate.Count != 0 && (_ListOfstate[0].GetType() == typeof(PatrolState) || _ListOfstate[0].GetType() == typeof(AggressifState)) || _navMesh == null) { InsertTarget(entityToAggresse); }
    }
    override public void ClearAllOrder()
    {
        _ListOfstate.Clear();
        base.ClearAllOrder();

        if (_navMesh) { _navMesh.StopPath(); }
        resetEvent.Invoke();
    }
    public void SortTarget()
    {
        TargetState nearest = (TargetState)_ListOfstate.Find(x => x.GetType() == typeof(TargetState));
        if (nearest != null)
        {
            foreach (StateClassEntity i in _ListOfstate)
            {
                if (i.GetType() == typeof(TargetState))
                {
                    TargetState c = (TargetState)i;
                    if (c.target && nearest != i)
                    {
                        if (Vector3.Distance(transform.localPosition, nearest.target.gameObject.transform.localPosition) > Vector3.Distance(transform.localPosition, c.target.gameObject.transform.localPosition))
                        {
                            nearest = (TargetState)i;
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
        if (_entityManager.GetType() == typeof(TroupeManager))
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
}
