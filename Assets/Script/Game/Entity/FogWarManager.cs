using UnityEngine;

public class FogWarManager : MonoBehaviour
{
    FogWarController fogWar;
    void Start()
    {
        fogWar = FindAnyObjectByType<FogWarController>();
        if(GetComponent<EntityController>() && fogWar)
        {
            fogWar.FogGestion(gameObject.GetComponent<EntityController>(), true);
        }
    }

    public void ActualiseFog(EntityController controller, bool hide)
    {
        if(fogWar)
        {
            fogWar.FogGestion(controller, hide);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(tag != "" && tag != "neutral")
        {
            if (collision.gameObject.GetComponent<EntityController>() && !collision.CompareTag(tag))
            {
                ActualiseFog(collision.gameObject.GetComponent<EntityController>(), false);
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.GetComponent<EntityController>() && !collision.CompareTag(tag))
        {
            ActualiseFog(collision.gameObject.GetComponent<EntityController>(), true);
        }
    }
}
