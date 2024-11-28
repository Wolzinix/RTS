using UnityEngine;

public  abstract class StateEffect
{
    protected SelectableManager entityAffected;
    protected float duration;
    public float actualTime;
    public float nextTime;

    protected StateEffect(float duration)
    {
        this.duration = duration;
        actualTime = 0;
        nextTime = 1;
    }
    protected StateEffect(SelectableManager entityAffected, float duration)
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
        actualTime += Time.deltaTime;
        if (actualTime > nextTime && nextTime < duration || !entityAffected)
        {
            ApplyEffect();
            nextTime += 1;
        }
        else if (nextTime >= duration) { end(); }
    }

    virtual public void end() 
    {
        entityAffected.RemoveEffect(this);
    }

    virtual public void ApplyEffect() { }
}
