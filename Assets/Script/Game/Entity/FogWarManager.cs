using UnityEngine;

public class FogWarManager : MonoBehaviour
{
    FogWarController fogWar;
    string _tag;
    void Start()
    {
        fogWar = FindAnyObjectByType<FogWarController>();
        _tag = gameObject.tag;
        if(GetComponent<EntityController>())
        {
            fogWar.FogGestion(gameObject.GetComponent<EntityController>(), true);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(_tag != "" && _tag != "neutral")
        {
            if (collision.gameObject.GetComponent<EntityController>() && !collision.CompareTag(_tag))
            {
                fogWar.FogGestion(collision.gameObject.GetComponent<EntityController>(), false);
            }
        }
        
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.GetComponent<EntityController>() && !collision.CompareTag(tag))
        {
            fogWar.FogGestion(collision.gameObject.GetComponent<EntityController>(), true);
        }
    }

    public void SetTag(string newTag)
    {
        _tag = newTag;
    }
}
