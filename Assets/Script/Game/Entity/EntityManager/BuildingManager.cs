
public class BuildingManager : SelectableManager
{

    public override void TakeDamage(TroupeManager entity, float nb)
    {
        base.TakeDamage(entity, nb);
        if(hp <= 0 )
        {
            Death();
        }

    }
}
