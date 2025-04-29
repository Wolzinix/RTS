using UnityEngine;

public class FogWarController : MonoBehaviour
{
    [SerializeField] MapMod mod;
    void Start()
    {
        foreach (EntityController i in FindObjectsOfType<EntityController>())
        {
            if (!i.gameObject.CompareTag(tag))
            {
                ActualiseFog(i, false);
            }
        }
    }

    public void FogGestion(EntityController entity, bool hide)
    {
        if(entity)
        {
            if (!entity.gameObject.CompareTag(tag))
            {
                if (hide && entity._EnnemieList.Count <= 0) { ActualiseFog(entity, false); }
                else { ActualiseFog(entity, true); }

                mod.ActualiseOneUnit(entity.GetComponent<SelectableManager>());
            }
        }
    }

    private void ActualiseFog(EntityController entity, bool visible)
    {
        foreach(SkinnedMeshRenderer i in entity.GetComponent<SelectableManager>().CurrentShape.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            i.enabled = visible;
        }

        foreach (MeshRenderer i in entity.GetComponent<SelectableManager>().CurrentShape.GetComponentsInChildren<MeshRenderer>())
        {
            i.enabled = visible;
        }
    }
}
