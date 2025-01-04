public class AttackEffectActivableCapacity : ActivableCapacity
{
    protected override void Start()
    {
        entityAffected = GetComponentInParent<SelectableManager>();
    }
}
