using System;
using Unity.VisualScripting;

public class StuntEffect : StateEffect
{
    override public void InitEffect(float duration) 
    {
        base.InitEffect(duration);
        nextTime = 0;
    }

    override public void InitEffect(SelectableManager entity, float duration) 
    {
        base.InitEffect(entity,duration);
        StuntEffect effect = Array.Find(entityAffected.GetComponents(typeof(StuntEffect)) as StateEffect[], x => x.entityAffected != null) as StuntEffect;
        if (entityAffected.GetType() == typeof(TroupeManager))
        {
            if (!effect) { entityAffected.AddComponent<StuntEffect>(); }
            else { effect.ResetEffect(); }
        }
        nextTime = 0;
    }

    public override void ApplyEffect()
    {
        if(entityAffected.GetComponent<EntityController>() != null)
        {
            entityAffected.GetComponent<EntityController>().AddStayOrderAtFirst();
        }
    }

    override public void end()
    {
        if (entityAffected.GetComponent<EntityController>() != null)
        {
            entityAffected.GetComponent<EntityController>().ClearAllOrder();
        }
        base.end();
    }

    public override void SetEntity(SelectableManager entity)
    {
        base.SetEntity(entity);

        StuntEffect effect = Array.Find(entityAffected.GetComponents(typeof(StuntEffect)) as StateEffect[], x => x.entityAffected != null) as StuntEffect;
        if (entityAffected.GetType() == typeof(TroupeManager))
        {
            if (!effect) { entityAffected.AddComponent<StuntEffect>(); }
            else { effect.ResetEffect(); }
        }
    }
}
