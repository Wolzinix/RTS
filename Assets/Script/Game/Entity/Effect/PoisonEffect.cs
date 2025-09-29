using Assets.Script.Game;
using Unity.VisualScripting;
using UnityEngine;

public class PoisonEffect : StateEffect
{
    [SerializeField] float damage;
    public void InitEffect(float duration, float damage)
    {
        base.InitEffect(duration);
        this.damage = damage;
    }
    public void InitEffect(SelectableManager entity, float duration, float damage)
    {
        base.InitEffect(entity, duration);
        this.damage = damage;
    }

    private PoisonEffect VerifyIfEffectAlreadyExist(SelectableManager entity)
    {
        PoisonEffect effect = null;
        foreach (PoisonEffect i in entity.GetComponents(typeof(PoisonEffect)))
        {
            if (i.entityAffected != null)
            {
                effect = i;
                break;
            }
        }
        return effect;
    }
    public override void ApplyEffect()
    {
        entityAffected.TakeDamage(damage);
    }

    public override void SetEntity(SelectableManager entity)
    {
        PoisonEffect effect = VerifyIfEffectAlreadyExist(entity);

        if (EntityTypeCalcul.IsATroupe(entityAffected.entityType))
        {
            if (!effect) 
            { 
                PoisonEffect poison = entityAffected.AddComponent<PoisonEffect>();
                poison.sprite = sprite;
            }
            else { effect.ResetEffect(); End(); }
        }

        base.SetEntity(entity);
    }


    override public void AddEffectToTarget(SelectableManager entityAffected)
    {
        PoisonEffect effect = VerifyIfEffectAlreadyExist(entityAffected);
        if (effect)
        {
            effect.ResetEffect();
        }
        else
        {
            effect = entityAffected.AddComponent<PoisonEffect>();
            effect.sprite = sprite;
            effect.InitEffect(entityAffected, duration, damage);
            entityAffected.AddEffect(effect);
        }
    }
}
