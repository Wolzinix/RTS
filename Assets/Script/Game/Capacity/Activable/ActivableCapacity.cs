public abstract class ActivableCapacity : ActiveCapacity
{
    public bool actif;
    private bool onlyOnce;

    public void ChangeActif()
    {
        actif = !actif;
        Apply();
    }

    protected override void Apply()
    {
        if (actif || onlyOnce)
        {
            base.Apply();
        }
    }

    protected override void DoEffect()
    {
        base.DoEffect();
        onlyOnce = false;
    }

    public void DoOnce()
    {
        onlyOnce = true;
    }
}
