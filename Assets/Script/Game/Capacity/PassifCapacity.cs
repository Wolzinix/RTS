public class PassifCapacity : CapacityController
{
    protected override void Start()
    {
        entityAffected = GetComponent<SelectableManager>();
        base.Apply();
    }
    protected override void Apply()
    {
    }
}
