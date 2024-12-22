using UnityEngine;

public abstract class CapacityController : MonoBehaviour
{
    public string Name;
    public Sprite sprite;
    StateEffect effect;
    protected SelectableManager entityAffected;
    [SerializeField] float cooldown;
    float actualTime = 0;
    public bool ready = true;

    protected virtual void Start()
    {
        effect = GetComponentInChildren<StateEffect>();
    }
    protected virtual void Apply()
    {
        if (ready)
        {
            DoEffect();
            ready = false;
            actualTime = 0;
        }
        else{ actualTime += Time.deltaTime; }

        if (actualTime >= cooldown) { ready = true; }
    }

    public void AddTarget(SelectableManager target)
    {
        entityAffected = target;
        ready = true;
        Apply();
    }

    protected virtual void DoEffect()
    {

        effect.AddEffectToTarget(entityAffected);
    }
}
