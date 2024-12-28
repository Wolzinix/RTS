public class BonusStatsEffect : StateEffect
{
    public float hp = 0;
    public float Attack = 0;
    public float AttackSpeed = 0;
    public float Range = 0;
    public float Speed = 0;
    override public void InitEffect(float duration)
    {
        this.duration = duration;
        actualTime = 0;
        nextTime = 0;
    }
    override public void InitEffect(SelectableManager entityAffected, float duration)
    {
        this.entityAffected = entityAffected;
        this.duration = duration;
        actualTime = 0;
        nextTime = 0;
    }
    override public void SetEntity(SelectableManager entity)
    {
        entityAffected = entity;
    }
    override public void Start() {}

    override public void Update() {}

    override public void ApplyEffect() 
    {
        entityAffected.SetHp(entityAffected.Hp + hp);
        entityAffected.MaxHp += hp;
        if(entityAffected.GetType() == typeof(AggressifEntityManager) || entityAffected.GetType() ==  typeof(TroupeManager))
        {
            AggressifEntityManager entity = (AggressifEntityManager) entityAffected;
            entity.Attack += Attack;
            entity.SetAttackSpeed(entity.AttackSpeed + AttackSpeed);
            entity.Range += Range;
            if(entity.GetType() == typeof(TroupeManager))
            {
                TroupeManager troupe = (TroupeManager) entityAffected;
                troupe.StartSpeed += Speed;
            }
        }
    }

    override public void AddEffectToTarget(SelectableManager entityAffected) 
    {
        SetEntity(entityAffected);
        ApplyEffect(); 
    }
}
