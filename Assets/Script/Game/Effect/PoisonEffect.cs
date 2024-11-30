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

    private bool verifyIfEffectAlreadyExist(SelectableManager entity)
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
        if (entity.GetType() == typeof(TroupeManager))
        {
            if (effect) { return true; }
        }
        return false;
    }
    public override void ApplyEffect()
    {
        entityAffected.TakeDamage(damage);
    }

    public override void SetEntity(SelectableManager entity)
    {
        base.SetEntity(entity); 
        PoisonEffect effect = null;
        foreach (PoisonEffect i in entityAffected.GetComponents(typeof(PoisonEffect)))
        {
            if (i.entityAffected != null)
            {
                effect = i;
                break;
            }
        }
        if (entityAffected.GetType() == typeof(TroupeManager))
        {
            if (!effect) { entityAffected.AddComponent<PoisonEffect>(); }
            else { effect.ResetEffect(); end(); }
        }
    }


    override public void AddEffectToTarget(SelectableManager entityAffected) 
    {
        if(verifyIfEffectAlreadyExist(entityAffected))
        {
            foreach (PoisonEffect i in entityAffected.GetComponents(typeof(PoisonEffect)))
            {
                if (i.entityAffected != null)
                {
                    i.ResetEffect();
                    break;
                }
            }
        }
        else
        {
            PoisonEffect effect = entityAffected.AddComponent<PoisonEffect>();
            effect.InitEffect(entityAffected, duration, damage);
        }
    }

}
