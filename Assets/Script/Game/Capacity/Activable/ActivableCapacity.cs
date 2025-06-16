using UnityEngine;
using UnityEngine.Events;

public abstract class ActivableCapacity : ActiveCapacity
{
    public bool actif;
    protected bool onlyOnce;
    [HideInInspector]public UnityEvent<ActivableCapacity> changeActif = new UnityEvent<ActivableCapacity>();

    public void ChangeActif()
    {
        Apply();
        actif = !actif;
        changeActif.Invoke(this);
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
