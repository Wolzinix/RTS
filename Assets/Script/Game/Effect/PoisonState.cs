
public class PoisonState : StateEffect
{
    float damage;
    public PoisonState(SelectableManager entity, float duration, float damage)
    {
        entityAffected = entity; 
        this.duration = duration; 
        actualTime = 0;
        this.damage = damage;
    }
    public override void Start()
    {
        if(entityAffected.GetType() != typeof(TroupeManager) && entityAffected.gotTheEffect(GetType()))
        {
            end();
        }
    }
    public override void Update()
    {
        if(actualTime < duration || !entityAffected) {  entityAffected.TakeDamage(damage); }
        else{ end(); }
    }
    virtual public void end() 
    {
        base.end();
    }
}
