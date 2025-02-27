using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FogWarController : MonoBehaviour
{
    [SerializeField] MapMod mod;
    void Start()
    {
        foreach (EntityController i in FindObjectsOfType<EntityController>())
        {
            if (i.gameObject.CompareTag(tag))
            {
                ActualiseFog(i, false);
            }
        }
    }

    public void FogGestion(EntityController entity, bool hide)
    {
        if (entity.gameObject.CompareTag(tag))
        {
            if (hide && entity.GetComponent<EntityController>()._EnnemieList.Count <= 0 ) { ActualiseFog(entity,false); }
            else { ActualiseFog(entity,true); }

            mod.ActualiseOneUnit(entity.GetComponent<SelectableManager>());
        }
    }

    private void ActualiseFog(EntityController entity, bool visible)
    {
        List<MeshRenderer> list = entity.GetComponentsInChildren<MeshRenderer>().ToList();
        if (list.Count == 0)
        {
            List<SkinnedMeshRenderer> render = entity.GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
            foreach (SkinnedMeshRenderer ren in render)
            {
                ren.enabled = visible;
            }
        }
        else
        {
            foreach (MeshRenderer renderer in list)
            {
                renderer.enabled = visible;
            }
        }
    }
}
