using System;
using Unity.VisualScripting;

public class PoisonEffect : StateEffect
{
    float damage;

    public void InitEffect(float duration, float damage)
    {
        base.InitEffect(duration);
        this.damage = damage;
    }
    public void InitEffect(SelectableManager entity, float duration, float damage) 
    {
        base.InitEffect(entity, duration);
        this.damage = damage;

        PoisonEffect effect = Array.Find(entityAffected.GetComponents(typeof(PoisonEffect)) as StateEffect[],x => x.entityAffected != null) as PoisonEffect;
        if (entityAffected.GetType() == typeof(TroupeManager))
        {
            if (!effect) { entityAffected.AddComponent<PoisonEffect>(); }
            else { effect.ResetEffect(); }
        }
    }

    public override void ApplyEffect()
    {
        entityAffected.TakeDamage(damage);
    }

    public override void SetEntity(SelectableManager entity)
    {
        base.SetEntity(entity);
        PoisonEffect effect = Array.Find(entityAffected.GetComponents(typeof(PoisonEffect)) as StateEffect[], x => x.entityAffected != null) as PoisonEffect;
        if (entityAffected.GetType() == typeof(TroupeManager))
        {
            if (!effect) { entityAffected.AddComponent<PoisonEffect>(); }
            else { effect.ResetEffect(); }
        }
    }
}
