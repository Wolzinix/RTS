using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.InputSystem.XR;

public abstract class CapacityController : MonoBehaviour
{
    public string Name;
    public Sprite sprite;
    StateEffect effect;
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
            if (Vector3.Distance(transform.position, entityAffected.transform.position) <= range)
            {
                DoEffect();
                ready = false;
                actualTime = 0;
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
}
