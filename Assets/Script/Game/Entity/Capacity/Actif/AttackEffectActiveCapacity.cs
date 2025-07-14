public class AttackEffectActiveCapacity : ActiveCapacity
{
    public bool IsActive;

    private AggressifEntityManager entity;
    protected override void Start()
    {
        base.Start();
        entity = GetComponent<AggressifEntityManager>();
    }
    override protected void DoEffect()
    {
        if(entity)
        {
            if(IsActive) { entity.effect = effect; }
            else { entity.effect = null; }
        }
    }

    public void ReverseActive()
    {
        IsActive = !IsActive;
    }
}
