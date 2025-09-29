namespace Assets.Script.Game
{
    public enum EntityType
    {
        None,
        Building,
        SoldatEpee,
        SoldatBouclier,
        SoldatArcher,
        Buildeur,
        DefenseBuilding
    }

    public static class EntityTypeCalcul
    {
        public static bool IsATroupe(EntityType entityType)
        {
            return entityType == EntityType.SoldatEpee ||
               entityType == EntityType.SoldatBouclier ||
                entityType == EntityType.SoldatArcher
            ;
        }
        public static bool IsAAggressif(EntityType entityType)
        {
            return entityType == EntityType.SoldatEpee ||
               entityType == EntityType.SoldatBouclier ||
                entityType == EntityType.SoldatArcher ||
                entityType == EntityType.DefenseBuilding
            ;
        }
    }
}