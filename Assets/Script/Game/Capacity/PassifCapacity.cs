public class PassifCapacity : CapacityController
{
    protected override void Start()
    {
        base.Start();
        entityAffected = GetComponentInParent<SelectableManager>();
        base.Apply();
    }
    protected override void Apply()
    {
    }
}
