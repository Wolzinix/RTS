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
            if (effect) { effect.ResetEffect();  }
        }

        base.InitEffect(entity, duration);
        nextTime = 0;
    }
    override public void ResetEffect()
    {
        actualTime = 0;
        nextTime = 1;
        base.End();
    }
    public override void ApplyEffect()
    {
        if(entityControllerAffected)
        {
            entityControllerAffected.AddStuntOrder();
        }
    }

    override public void End()
    {
        if (entityAffected && entityControllerAffected)
        {
            entityControllerAffected.RemoveFirstOrder();
        }
        base.End();
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
            if (!effect) { effect = entityAffected.AddComponent<StuntEffect>();
                effect.sprite = sprite;
            }
            else { effect.ResetEffect();}
        }
    }

    override public void AddEffectToTarget(SelectableManager entityAffected)
    {
        StuntEffect effect = entityAffected.AddComponent<StuntEffect>();
        effect.sprite = sprite;
        entityAffected.AddEffect(effect);
        effect.InitEffect(entityAffected, duration);
    }
}
