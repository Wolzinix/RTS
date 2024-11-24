using System;
using UnityEngine;

public  abstract class StateEffect
{
    protected SelectableManager entityAffected;
    protected float duration;
    protected float actualTime;
    protected float nextTime;
    virtual public void Start() { }

    virtual public void Update() { }

    virtual public void end() 
    {
        entityAffected.RemoveEffect(this);
    }
}
