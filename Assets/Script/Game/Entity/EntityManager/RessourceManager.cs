

public class RessourceManager : EntityManager
{
    override protected void Awake()
    {
        base.Awake();
    }


    public void TakeDamage(TroupeManager entity, float nb)
    {
        base.TakeDamage(entity, nb);

        changeStats.Invoke();

        if (hp <= 0)
        {
            entity.AddToRessourcesKilledEntity(PriceWhenDestroy);
        }
    }
}
