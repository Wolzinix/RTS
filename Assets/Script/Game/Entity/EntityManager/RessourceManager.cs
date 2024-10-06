

public class RessourceManager : EntityManager
{
    override protected void Awake()
    {
        base.Awake();
    }


    override public void TakeDamage(TroupeManager entity, float nb)
    {
        base.TakeDamage(entity, nb);

        changeStats.Invoke();

        if (hp <= 0)
        {
            entity.AddToRessourcesKilledEntity(GoldAmount, WoodAmount);
        }
    }

    override protected void Death()
    {
        base.Death();
        if (hp <= 0)
        {
            Destroy(gameObject);
        }

    }

}
