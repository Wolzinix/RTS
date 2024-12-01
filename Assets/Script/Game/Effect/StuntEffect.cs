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
        StuntEffect effect = null;
        foreach (StuntEffect i in entity.GetComponents(typeof(StuntEffect)))
        {
            if(i.entityAffected != null && i!=this)
            {
                effect = i;
                break;
            }
        }
        if (entity.GetType() == typeof(TroupeManager))
        {
            if (effect) { effect.ResetEffect(); end(); }
        }

        base.InitEffect(entity, duration);
        nextTime = 0;
    }

    public override void ApplyEffect()
    {
        if(entityAffected.GetComponent<EntityController>() != null)
        {
            entityAffected.GetComponent<EntityController>().AddStuntOrder();
        }
    }

    override public void end()
    {
        if (entityAffected != null && entityAffected.GetComponent<EntityController>() != null)
        {
            entityAffected.GetComponent<EntityController>().RemoveFirstOrder();
        }
        base.end();
    }

    public override void SetEntity(SelectableManager entity)
    {
        base.SetEntity(entity);
        StuntEffect effect = null;
        foreach (StuntEffect i in entityAffected.GetComponents(typeof(StuntEffect)))
        {
            if (i.entityAffected != null)
            {
                effect = i;
                break;
            }
        }
        if (entityAffected.GetType() == typeof(TroupeManager))
        {
            if (!effect) { entityAffected.AddComponent<StuntEffect>(); }
            else { effect.ResetEffect(); end(); }
        }
    }

    override public void AddEffectToTarget(SelectableManager entityAffected)
    {
        StuntEffect effect = entityAffected.AddComponent<StuntEffect>();
        effect.InitEffect(entityAffected, duration);
    }
}
