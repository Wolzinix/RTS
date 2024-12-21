using UnityEngine;

public abstract class CapacityController : MonoBehaviour
{
    [SerializeField] StateEffect effect;
    protected SelectableManager entityAffected;
    [SerializeField] float cooldown;
    float actualTime = 0;
    public bool ready = true;

    protected virtual void Start()
    {
        
    }
    protected virtual void Apply()
    {
        if (ready)
        {
            effect.AddEffectToTarget(entityAffected);
            ready = false;
            actualTime = 0;
        }
        else{ actualTime += Time.deltaTime; }

        if (actualTime >= cooldown) { ready = true; }
    }

    protected void AddTarget(SelectableManager target)
    {
        entityAffected = target;
        Apply();
    }
}
