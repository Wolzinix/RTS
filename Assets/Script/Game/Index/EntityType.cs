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
        public static bool IsABuilding(EntityType entityType)
        {
            return entityType == EntityType.Building;
        }
        public static bool IsADefense(EntityType entityType) 
        {
            return entityType == EntityType.DefenseBuilding;
        }
        public static bool IsAAggressif(EntityType entityType)
        {
            return IsATroupe(entityType) || IsADefense(entityType);
        }

        public static bool IsABuildeur(EntityType entityType)
        {
            return entityType == EntityType.Buildeur;
        }

        public static bool IsAggressifOrBuildeur(EntityType entityType)
        {
            return IsABuildeur(entityType) || IsAAggressif(entityType);
        }
        public static bool IsATroupeOrBuildeur(EntityType entityType)
        {
            return IsABuildeur(entityType) || IsATroupe(entityType);
        }
        public static bool IsSelectable(EntityType entityType)
        {
            return entityType != EntityType.None;
        }
    }
}