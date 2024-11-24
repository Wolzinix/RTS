
public class PoisonState : StateEffect
{
    float damage;
    public PoisonState(SelectableManager entity, float duration, float actualTime, float damage)
    {
        entityAffected = entity; 
        this.duration = duration; 
        this.actualTime = actualTime;
        this.damage = damage;
    }
    protected override void Start()
    {
    }
    protected override void Update()
    {
        if(actualTime < duration || !entityAffected) {  entityAffected.TakeDamage(damage); }
        else{ end(); }
    }
    virtual protected void end() { }
}
