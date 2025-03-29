public class AttackEffectActivableCapacity : ActivableCapacity
{

    private SelectableManager _target;
    
    protected override void Start()
    {
        base.Start();
        entityAffected = GetComponentInParent<SelectableManager>();
    }

    protected override void DoEffect()
    {
        if (_target)
        {
            if(onlyOnce)
            {
                _troupeManager.effect = effect;
                _controller.AddTarget(_target);
                _troupeManager.DoAnAttack.AddListener(DesactiveOnce);
                _controller.resetEvent.AddListener(DesactiveOnce);
            }
            else{ _target = null; }
        }
        else
        {
            if (_troupeManager.effect) { _troupeManager.effect = null; }
            else { _troupeManager.effect = effect; }
        }
        base.DoEffect();
    }

    public override void AddTarget(SelectableManager target)
    {
        _target = target;
        Apply();
    }

    public void DesactiveOnce()
    {
        onlyOnce = false;
        _troupeManager.DoAnAttack.RemoveListener(DesactiveOnce);
        _controller.resetEvent.RemoveListener(DesactiveOnce);
        Apply();
    }
}
