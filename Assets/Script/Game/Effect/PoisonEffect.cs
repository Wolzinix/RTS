public class PoisonEffect : StateEffect
{
    float damage;

    public PoisonEffect(float duration, float damage) : base(duration)
    {
        this.damage = damage;
    }
    public PoisonEffect(SelectableManager entity, float duration, float damage) : base(entity, duration)
    {
        this.damage = damage;

        if (entityAffected.GetType() == typeof(TroupeManager))
        {
            if (!entityAffected.gotTheEffect(GetType())) { entity.AddEffect(this); }
            else { entity.RemoveFirstEffectOfType(typeof(PoisonEffect)); entity.AddEffect(this); }
        }
    }

    public override void ApplyEffect()
    {
        entityAffected.TakeDamage(damage);
    }

    public override void SetEntity(SelectableManager entity)
    {
        base.SetEntity(entity);
        if (entityAffected.GetType() == typeof(TroupeManager))
        {
            if (!entityAffected.gotTheEffect(GetType())) { entity.AddEffect(this); }
            else { entity.RemoveFirstEffectOfType(typeof(PoisonEffect)); entity.AddEffect(this); }
        }
    }
}
