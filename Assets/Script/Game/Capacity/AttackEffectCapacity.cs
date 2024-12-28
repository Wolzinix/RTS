public class AttackEffectCapacity : ActiveCapacity
{
    public bool IsActive;
    override protected void DoEffect()
    {
        AggressifEntityManager entity = GetComponent<AggressifEntityManager>();
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
