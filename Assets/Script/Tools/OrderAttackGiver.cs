using UnityEngine;

public abstract class OrderAttackGiver : MonoBehaviour
{
    public static void OrderGiver(EntityController entity, Vector3 target)
    {
        entity.AddAggressivePath(target);
    }
}
