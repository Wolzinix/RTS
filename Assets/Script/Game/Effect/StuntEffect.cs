public class StuntEffect : StateEffect
{
    public StuntEffect(float duration) : base(duration)
    {
        nextTime = 0;
    }

    public StuntEffect(SelectableManager entity, float duration) : base(entity, duration)
    {
        if (entityAffected.GetType() == typeof(TroupeManager))
        {
            if (!entityAffected.gotTheEffect(GetType())) { entity.AddEffect(this); }
            else { entity.RemoveFirstEffectOfType(typeof(StuntEffect)); entity.AddEffect(this); }
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
        if (entityAffected.GetType() == typeof(TroupeManager))
        {
            if (!entityAffected.gotTheEffect(GetType())) { entity.AddEffect(this); }
            else { entity.RemoveFirstEffectOfType(typeof(PoisonEffect)); entity.AddEffect(this); }
        }
    }
}
