using UnityEngine;
public class PoisonState : StateEffect
{
    float damage;
    public PoisonState(SelectableManager entity, float duration, float damage)
    {
        entityAffected = entity; 
        this.duration = duration; 
        actualTime = 0;
        this.damage = damage;
        nextTime = 0;

        if (entityAffected.GetType() == typeof(TroupeManager) && !entityAffected.gotTheEffect(GetType()))
        {
            entity.AddEffect(this);
        }
    }
    public override void Start()
    {
        
    }
    public override void Update()
    {
        actualTime += Time.deltaTime;
        if (actualTime < nextTime && nextTime<duration || !entityAffected) 
        {
            entityAffected.TakeDamage(damage);
            nextTime += 1;
        }
        else{ end(); }
    }
    virtual public void end() 
    {
        base.end();
    }
}
