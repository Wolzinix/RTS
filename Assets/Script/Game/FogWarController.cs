using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

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
        List<MeshRenderer> list = entity.GetComponentsInChildren<MeshRenderer>().ToList();

        

        if (list.Count == 0)
        { 
            List < SkinnedMeshRenderer> render = entity.GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
            foreach (SkinnedMeshRenderer ren in render)
            {
                ren.enabled = true;
            }
        }
        else
        {
            foreach (MeshRenderer renderer in list)
            {
                renderer.enabled = true;
            }
        }    
    }

    private void AddFromFog(EntityController entity)
    {
        List<MeshRenderer> list = entity.GetComponentsInChildren<MeshRenderer>().ToList();
        if (list.Count == 0)
        {
            List<SkinnedMeshRenderer> render = entity.GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
            foreach (SkinnedMeshRenderer ren in render)
            {
                ren.enabled = false;
            }
        }
        else
        {
            foreach (MeshRenderer renderer in list)
            {
                renderer.enabled = false;
            }
        }
    }
}
