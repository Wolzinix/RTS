using UnityEngine;

public class FogWarController : MonoBehaviour
{
    void Start()
    {
        foreach (EntityController i in FindObjectsOfType<EntityController>())
        {
            if (i.gameObject.tag != gameObject.tag)
            {
                AddFromFog(i);
            }
        }
    }

    public void FogGestion(EntityController entity, bool hide)
    {
        if (entity.gameObject.tag != gameObject.tag)
        {
            if (hide && entity.GetComponent<EntityController>()._EnnemieList.Count <= 0 ) { AddFromFog(entity); }
            else { RemoveFromFog(entity); }
        }
    }

    private void RemoveFromFog(EntityController entity)
    {
        entity.GetComponentInChildren<MeshRenderer>().enabled = true;
    }

    private void AddFromFog(EntityController entity)
    {
        entity.GetComponentInChildren<MeshRenderer>().enabled = false;
    }
}
