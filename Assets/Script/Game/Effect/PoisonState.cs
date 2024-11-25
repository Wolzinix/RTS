using UnityEngine;
public class PoisonState : StateEffect
{
    float damage;
    public PoisonState(SelectableManager entity, float duration, float damage) : base(entity, duration)
    {
        this.damage = damage;

        if (entityAffected.GetType() == typeof(TroupeManager))
        {
            if (!entityAffected.gotTheEffect(GetType())) { entity.AddEffect(this); }
            else { entity.RemoveFirstEffectOfType(typeof(PoisonState)); entity.AddEffect(this); }
        }
    }

    protected override void ApplyEffect()
    {
        entityAffected.TakeDamage(damage);
    }
}
