using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XR;

public class EntityController : BuildingController
{
    public NavMeshController _navMesh;
    public Transform pointOfSpawn;

    [SerializeField] private ProjectilManager _projectile;

    [HideInInspector] public UnityEvent EntityIsArrive = new();
    [HideInInspector] public UnityEvent resetEvent = new();
    [HideInInspector] public Animator _animator;

    protected bool _attacking;
    protected EntityStateManagement _EntityStateManagement;


    public int GetLenghtOfState()
    {
        return _EntityStateManagement.GetLenghtOfState();
    }
    public StateClassEntity GetFirstState()
    {
        return _EntityStateManagement.GetFirstState();
    }
    override protected void Awake()
    {
        base.Awake();

        _animator = GetComponentInChildren<Animator>();
        _navMesh = GetComponent<NavMeshController>();

        _EntityStateManagement = new EntityStateManagement();

        GetComponent<SelectableManager>().TakingDamageFromEntity.AddListener(AddAggresseurTarget);

        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        }

    }
    override protected void LateUpdate()
    {
        _EntityStateManagement.Update();

        if (!_EntityStateManagement.IsInStun() &&
            _EntityStateManagement.IsInSeachState()
        )
        {
            SearchTarget();
        }
    }
    override protected void SearchTarget()
    {
        base.SearchTarget();

        if (_EntityStateManagement.AlreadyGotThisOrder(typeof(TargetState)))
        {
            if(_navMesh)_navMesh.StopPath();
        }
    }
    public void InsertTarget(SelectableManager target)
    {
        _EntityStateManagement.InsertTarget(target, this, _navMesh);
    }
    override protected void AddEnnemi(SelectableManager target)
    {
        if (!_EntityStateManagement.IsInStun())
        {
            base.AddEnnemi(target);
            InsertTarget(target);
        }
    }
    protected override void OnDestroy() { }
    protected override void ClearListOfAlly(List<GameObject> list)
    {
        if (list.Count != _listOfalliesOnRange.Count)
        {
            foreach (GameObject i in _listOfalliesOnRange)
            {
                if (!list.Contains(i))
                {
                    if (i) { i.GetComponent<SelectableManager>().TakingDamageFromEntity.RemoveListener(AddAggresseurTarget); }
                }
            }

            base.ClearListOfAlly(list);
        }
    }
    public void RemoveFirstOrder()
    {
        AnimationController.CancelAnimation(_animator);
        _EntityStateManagement.RemoveFirstOrder();
        if(_entityManager.GetType() == typeof(TroupeManager))
        {

            TroupeManager manager = (TroupeManager)_entityManager;
            manager.SetSpeedWithoutAnimation(GetStartSpeed());
        }
    }

    public void AddAttackState(SelectableManager target)
    {
        _EntityStateManagement.AddAttackState(target, this, _projectile);
    }
    public void AddPath(Vector3 newPath)
    {
        _EntityStateManagement.AddPath(newPath, this, _navMesh);
    }

    public void AddPathWithRange(Vector3 newPath)
    {
        _EntityStateManagement.AddPathWithRange(newPath, this, _navMesh);
    }

    public void AddPathWithRange(Vector3 newPath, float range)
    {
        _EntityStateManagement.AddPathWithRange(newPath, range, this, _navMesh);
    }

    public void AddPathInFirst(Vector3 newPath)
    {
        _EntityStateManagement.AddPathInFirst(newPath, this, _navMesh);
    }
    public void AddTarget(SelectableManager target)
    {
        _EntityStateManagement.AddTarget(target,this,_navMesh);
    }
    
    public void AddAllie(SelectableManager target)
    {
        _EntityStateManagement.AddAllie(target, this, _navMesh);
    }

    public void ClearAllOrderOfType(Type type)
    {
        _EntityStateManagement.ClearAllOrderOfType(type);
    }
    public void AddPatrol(Vector3 point)
    {
        _EntityStateManagement.AddPatrol(point, this, _navMesh);
    }
    public void AddAggressivePath(Vector3 newPath)
    {
        _EntityStateManagement.AddAggressivePath(newPath, this, _navMesh);
    }
    public void AddStayOrder()
    {
        _EntityStateManagement.AddStayOrder(this, _navMesh);
    }
    public void AddStayOrderAtFirst()
    {
        _EntityStateManagement.AddStayOrderAtFirst(this, _navMesh);
    }

    public void AddStuntOrder()
    {
        _EntityStateManagement.AddStuntOrder(this, _navMesh);
    }
    private void AddAggresseurTarget(AggressifEntityManager entityToAggresse)
    {
        _EntityStateManagement.AddAggresseurTarget(entityToAggresse, this, _navMesh);
    }
    public void SortTarget()
    {
        _EntityStateManagement.SortTarget(this, _navMesh);
    }
    override public void ClearAllOrder()
    {
        foreach (CapacityController i in GetComponentsInChildren<CapacityController>())
        {
            i.CancelCapacity();
        }
        _EntityStateManagement.ClearAllOrder();
        base.ClearAllOrder();
        _navMesh.StopPath();
        resetEvent.Invoke();
        CancelAnimation();
    }

    public void CancelAnimation()
    {
        // agit un poil trop souvent
        AnimationController.CancelAnimation(_animator);
    }

    public void PlayAnimation(int categorie)
    {
        AnimationController.PlayAnimation(categorie,_animator);
    }

    public float GetAnimationInfo()
    {
        return AnimationController.GetAnimationStateInfo(_animator);
    }
    public bool IsMoving()
    {
        return AnimationController.IsMovingAnimation(_animator);
    }
    
    public void ChangeSpeed(float speed)
    {
        if (_entityManager.GetType() == typeof(TroupeManager))
        {
            TroupeManager entity = (TroupeManager)_entityManager;
            entity.SetSpeed(speed);
        }
    }
    public void ChangeSpeedExepctAnim(float speed)
    {
        if( _entityManager.GetType() == typeof(TroupeManager))
        {

            TroupeManager manager = (TroupeManager)_entityManager;
            float animCap = (int)(GetAnimationInfo() / 0.25f);
            manager.SetSpeedWithoutAnimation(speed * (GetAnimationInfo() - (animCap * 0.25f)) * 2 + (speed * 0.11f));
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