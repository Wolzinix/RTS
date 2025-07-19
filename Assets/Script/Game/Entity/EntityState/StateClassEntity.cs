using System;

public abstract class StateClassEntity
{
    public virtual void Start() { }
    public virtual void Update() { }
    public virtual void End() { }

    public void Dispose() { GC.SuppressFinalize(this); }
}
