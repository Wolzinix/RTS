using UnityEngine;

public abstract class StateEffect : MonoBehaviour
{
    public SelectableManager entityAffected;
    [SerializeField] protected float duration;
    public float actualTime;
    public float nextTime;

    public Sprite sprite;

    protected EntityController entityControllerAffected;

    virtual public void InitEffect(float duration)
    {
        this.duration = duration;
        actualTime = 0;
        nextTime = 1;
    }
    virtual public void InitEffect(SelectableManager entityAffected, float duration)
    {
        this.entityAffected = entityAffected;
        this.duration = duration;
        actualTime = 0;
        nextTime = 1;
        entityControllerAffected = entityAffected.GetComponent<EntityController>();
    }
    virtual public void SetEntity(SelectableManager entity)
    {
        entityAffected = entity;
        entityControllerAffected = entityAffected.GetComponent<EntityController>();
    }
    virtual public void Start() { }

    virtual public void Update()
    {
        if (entityAffected)
        {
            actualTime += Time.deltaTime;
            if (actualTime > nextTime && nextTime < duration || !entityAffected)
            {
                ApplyEffect();
                nextTime += 1;
            }
            if (nextTime >= duration) { End(); }
        }
    }

    public bool IsFinish()
    {
        return nextTime >= duration;
    }

    virtual public void End()
    {
        entityAffected.RemoveEffect(this);
        Destroy(this);
    }

    virtual public void ApplyEffect() { }

    public void ResetEffect()
    {
        ApplyEffect();
        actualTime = 0;
        nextTime = 1;
    }

    virtual public void AddEffectToTarget(SelectableManager entityAffected) {  }
}
