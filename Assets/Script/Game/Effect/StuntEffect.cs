public class StuntEffect : StateEffect
{
    public StuntEffect(SelectableManager entity, float duration) : base(entity, duration)
    {
        if (entityAffected.GetType() == typeof(TroupeManager))
        {
            if (!entityAffected.gotTheEffect(GetType())) { entity.AddEffect(this); }
            else { entity.RemoveFirstEffectOfType(typeof(StuntEffect)); entity.AddEffect(this); }
        }
    }

    public override void ApplyEffect()
    {
        if(entityAffected.GetComponent<EntityController>() != null)
        {
            entityAffected.GetComponent<EntityController>().AddStayOrder();
        }
    }

    override public void end()
    {
        if (entityAffected.GetComponent<EntityController>() != null)
        {
            entityAffected.GetComponent<EntityController>().ClearAllOrder();
        }
    }
}
