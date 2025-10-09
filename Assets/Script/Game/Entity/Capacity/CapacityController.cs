using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public abstract class CapacityController : MonoBehaviour
{
    public string Name;
    public Sprite sprite;
    public string Description;
    protected StateEffect effect;
    protected SelectableManager entityAffected;
    [SerializeField] protected float cooldown;
    public float actualTime = 0;
    public bool ready = true;
    public float range = 0;

    protected EntityController _controller;
    protected TroupeManager _troupeManager;
    protected NavMeshController _navMeshController;
    public UnityEvent<CapacityController> ActivateEvent;

    protected virtual void Start()
    {
        effect = GetComponentInChildren<StateEffect>();
        _controller = GetComponentInParent<EntityController>();
        _troupeManager = GetComponentInParent<TroupeManager>();
        _navMeshController = GetComponentInParent<NavMeshController>();
    }

    protected virtual void Update()
    {
        if (!ready) { actualTime += Time.deltaTime; }

        if (actualTime >= cooldown) { ready = true; }
    }
    public float GetCooldown() { return cooldown; }
    protected virtual void Apply()
    {
        if (gameObject && ready && entityAffected)
        {
            ActivateEvent.Invoke(this);
            if (Vector3.Distance(transform.position, entityAffected.transform.position) <= _navMeshController.HaveStoppingDistance() + 0.5 + range)
            {
                DoEffect();
                ready = false;
                actualTime = 0;
            }
            else
            {
                if (_troupeManager)
                {
                    _controller.AddPathWithRange(entityAffected.transform.position, range);
                    StateClassEntity state = _controller.GetFirstState();
                    if (state.GetType() == typeof(MoveToDistanceState))
                    {
                        MoveToDistanceState MState = (MoveToDistanceState)state;
                        MState.Arrived.AddListener(Apply);
                    }
                }
            }
        }
    }
    public void CancelCapacity()
    {
        if( _controller && _controller.GetLenghtOfState() > 0)
        {
            StateClassEntity state = _controller.GetFirstState();
            if (state.GetType() == typeof(MoveToDistanceState))
            {
                MoveToDistanceState MState = (MoveToDistanceState)state;
                MState.Arrived.RemoveListener(Apply);
            }
        }
        entityAffected = null;
    }
    public virtual void AddTarget(SelectableManager target)
    {
        if(target != _troupeManager)
        {
            entityAffected = target;
            Apply();
        }
    }

    protected virtual void DoEffect()
    {
        effect.AddEffectToTarget(entityAffected);
        CancelCapacity();
    }
}