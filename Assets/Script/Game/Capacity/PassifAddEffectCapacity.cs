using Unity.VisualScripting;
using UnityEngine;

public class PassifAddEffectCapacity : ActiveCapacity
{
    public bool actif;

    protected override void Start()
    {
        effect = GetComponentInChildren<StateEffect>();
        entityAffected = GetComponentInParent<SelectableManager>();
    }


    public void ChangeActif()
    {
        actif = !actif;
        DoEffect();
    }


    protected override void Apply()
    {
        if (actif)
        {
            if (entityAffected)
            {
                DoEffect();
                ready = false;
                actualTime = 0;
            }
        }
    }

    protected virtual void DoEffect()
    {
        bool a = false;
        if (entityAffected.IsConvertibleTo(typeof(AggressifEntityManager),a))
        {
            AggressifEntityManager entity = (AggressifEntityManager)entityAffected;
            if (ready && actif) { entity.effect = effect; }
            else if(!ready || !actif) { entity.effect = null; }
        }
    }
    protected override void Update()
    {
        if (!ready) { actualTime += Time.deltaTime; }

        if (actualTime >= cooldown) { ready = true; }
    }
}
