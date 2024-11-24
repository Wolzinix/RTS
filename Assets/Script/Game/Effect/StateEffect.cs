public  abstract class StateEffect
{
    protected SelectableManager entityAffected;
    protected float duration;
    protected float actualTime;
    virtual public void Start() { }

    virtual public void Update() { }

    virtual public void end() { }
}
