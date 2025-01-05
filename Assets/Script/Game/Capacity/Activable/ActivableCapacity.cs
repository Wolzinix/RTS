public abstract class ActivableCapacity : ActiveCapacity
{
    public bool actif;
    protected bool onlyOnce;


    public void ChangeActif()
    {
        Apply();
        actif = !actif;
        Apply();
    }

    protected override void Apply()
    {
        if (actif || onlyOnce)
        {
            if (ready)
            {
                if (entityAffected)
                {
                    DoEffect();
                    ready = false;
                    actualTime = 0;
                }
            }
        }
        onlyOnce = false;
        DoEffect();
    }

    protected override void DoEffect()
    {
        onlyOnce = false;
    }

    public void DoOnce()
    {
        onlyOnce = true;
    }
}
