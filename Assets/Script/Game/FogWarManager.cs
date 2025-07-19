using UnityEngine;

public class FogWarManager : MonoBehaviour
{
    FogWarController fogWar;
    EntityController entityController;
    void Start()
    {
        fogWar = FindAnyObjectByType<FogWarController>();
        entityController = GetComponent<EntityController>();
        if (entityController && fogWar) { fogWar.FogGestion(entityController, true); }
    }

    public void ActualiseFog(EntityController controller, bool hide)
    {
        if(fogWar){ fogWar.FogGestion(controller, hide); }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(tag != "" && tag != "neutral")
        {
            EntityController collisionController = collision.GetComponent<EntityController>();
            if (collisionController && !collision.CompareTag(tag)) { ActualiseFog(collisionController, false); }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        EntityController collisionController = collision.GetComponent<EntityController>();
        if (collisionController && !collision.CompareTag(tag)){ ActualiseFog(collisionController, true); }
    }

    public void AddToFog(EntityController collisionController)
    {
        ActualiseFog(collisionController, true);
    }
}
