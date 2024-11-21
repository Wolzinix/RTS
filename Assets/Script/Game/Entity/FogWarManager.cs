using UnityEngine;

public class FogWarManager : MonoBehaviour
{
    FogWarController fogWar;
    void Start()
    {
        fogWar = FindAnyObjectByType<FogWarController>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<EntityController>() && !collision.CompareTag(tag)) 
        {
            fogWar.FogGestion(collision.gameObject.GetComponent<EntityController>(), false); 
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.GetComponent<EntityController>() && !collision.CompareTag(tag))
        {
            fogWar.FogGestion(collision.gameObject.GetComponent<EntityController>(), true);
        }
    }
}
