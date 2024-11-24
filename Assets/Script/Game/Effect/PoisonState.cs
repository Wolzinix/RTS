
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
        if(entityAffected.GetType() != typeof(TroupeManager) && entityAffected.gotTheEffect(GetType()))
        {
            end();
        }
    }
    protected override void Update()
    {
        if(actualTime < duration || !entityAffected) {  entityAffected.TakeDamage(damage); }
        else{ end(); }
    }
    virtual protected void end() 
    {
        Destroy(this);
    }
}
