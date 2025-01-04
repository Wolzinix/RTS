public abstract class ActivableCapacity : ActiveCapacity
{
    public bool actif;

    public void ChangeActif()
    {
        actif = !actif;
        Apply();
    }

    protected override void Apply()
    {
        if (actif)
        {
            base.Apply();
        }
    }
}
