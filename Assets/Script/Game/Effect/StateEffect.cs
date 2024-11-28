using UnityEngine;

public abstract class StateEffect: MonoBehaviour
{
    public SelectableManager entityAffected;
    [SerializeField] protected float duration;
    public float actualTime;
    public float nextTime;

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
    }
    virtual public void SetEntity(SelectableManager entity)
    {
        entityAffected = entity;
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
            else if (nextTime >= duration) { end(); }
        }
    }

    virtual public void end() 
    {
        Destroy(this);
    }

    virtual public void ApplyEffect() { }

    public void ResetEffect()
    {
        ApplyEffect();
        actualTime = 0;
        nextTime = 1;
    }
}
