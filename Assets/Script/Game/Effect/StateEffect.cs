using System;
using UnityEngine;

public  abstract class StateEffect
{
    protected SelectableManager entityAffected;
    protected float duration;
    protected float actualTime;
    protected float nextTime;

    protected StateEffect(SelectableManager entityAffected, float duration)
    {
        this.entityAffected = entityAffected;
        this.duration = duration;
        actualTime = 0;
        nextTime = 1;
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

    virtual protected void ApplyEffect() { }
}
