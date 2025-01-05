public class AttackEffectActivableCapacity : ActivableCapacity
{

    private SelectableManager target;
    protected override void Start()
    {
        base.Start();
        entityAffected = GetComponentInParent<SelectableManager>();
    }

    protected override void DoEffect()
    {
        
        if (target)
        {
            if(onlyOnce)
            {
                GetComponentInParent<TroupeManager>().effect = effect;
                GetComponentInParent<EntityController>().AddTarget(target);
                GetComponentInParent<TroupeManager>().DoAnAttack.AddListener(DesactiveOnce);
                GetComponentInParent<EntityController>().resetEvent.AddListener(DesactiveOnce);
            }
            else
            {
                target = null;
            }
        }
        else
        {
            if (GetComponentInParent<TroupeManager>().effect)
            {
                GetComponentInParent<TroupeManager>().effect = null;
            }
            else
            {
                GetComponentInParent<TroupeManager>().effect = effect;
            }
        }
        base.DoEffect();
    }

    public override void AddTarget(SelectableManager target)
    {
        this.target = target;
        Apply();
    }

    public void DesactiveOnce()
    {
        onlyOnce = false;
        GetComponentInParent<TroupeManager>().DoAnAttack.RemoveListener(DesactiveOnce);
        GetComponentInParent<EntityController>().resetEvent.RemoveListener(DesactiveOnce);
        Apply();
    }
}
