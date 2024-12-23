using System.Linq;
using UnityEngine;

public abstract class CapacityController : MonoBehaviour
{
    public string Name;
    public Sprite sprite;
    protected StateEffect effect;
    protected SelectableManager entityAffected;
    [SerializeField] float cooldown;
    public float actualTime = 0;
    public bool ready = true;
    public float range = 0;

    protected virtual void Start()
    {
        effect = GetComponentInChildren<StateEffect>();
    }

    protected virtual void Update()
    {
        if (!ready) { actualTime += Time.deltaTime; }

        if (actualTime >= cooldown) { ready = true; }
    }
    protected virtual void Apply()
    {
        if (ready)
        {
            if(entityAffected)
            {
                if (Vector3.Distance(transform.position, entityAffected.transform.position) <=  GetComponentInParent<NavMeshController>().HaveStoppingDistance() + 0.5 + range)
                {
                    DoEffect();
                    ready = false;
                    actualTime = 0;
                }
                else
                {
                    if (GetComponentInParent<TroupeManager>())
                    {
                        EntityController entityC = GetComponentInParent<EntityController>();
                        entityC.AddPathWithRange(entityAffected.transform.position, range);
                        StateClassEntity state = entityC._ListOfstate.First();
                        if (state.GetType() == typeof(MoveToDistanceState))
                        {
                            MoveToDistanceState MState = (MoveToDistanceState)state;
                            MState.Arrived.AddListener(Apply);
                        }
                    }
                }
            }
           
        }
    }
    public void AddTarget(SelectableManager target)
    {
        entityAffected = target;
        Apply();
    }

    protected virtual void DoEffect()
    {
        effect.AddEffectToTarget(entityAffected);
    }

    public void ReverseReady()
    {
        ready = !ready;
        Apply();
    }
}
