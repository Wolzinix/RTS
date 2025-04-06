using Assets.Script.Tools;
using System.Linq;
using UnityEngine;

public class BuildingPreWatching : MonoBehaviour
{
    private GameObject currentBuilding;
    [SerializeField] private Shader shaderToApply;
    [SerializeField] private Shader shaderToNotApply;
    [SerializeField] private LayerMask _ExcludeLayer;

    public Collider[] DoAOverlap(Vector3 spawnPosition)
    {
        return Physics.OverlapSphere(spawnPosition, 1, ~_ExcludeLayer, QueryTriggerInteraction.Ignore);
    }
    void LateUpdate()
    {
        if(currentBuilding) 
        { 
            Vector3 position = RayCast.DoARayCastToGroundFromMouse().point;
            if(position != Vector3.zero ) 
            {
                currentBuilding.SetActive(true);
                currentBuilding.transform.position = position;
                if (DoAOverlap(position).Count() <= 1) { ApplyShader(shaderToApply); }
                else { ApplyShader(shaderToNotApply); }
            }
            else { currentBuilding.SetActive(false); }
        }
    }
    
    private void ApplyShader(Shader shader)
    {
        MeshRenderer[] meshRenderers = currentBuilding.GetComponentsInChildren<MeshRenderer>();
        if (meshRenderers.Length > 0)
        {
            foreach (MeshRenderer meshRender in meshRenderers) { meshRender.material.shader = shader; }
        }
        else
        {
            SkinnedMeshRenderer[] SkinnedMeshRenderer = currentBuilding.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer meshRender in SkinnedMeshRenderer)
            {
                foreach (Material material in meshRender.materials) { material.shader = shader; }
            }
        }
    }
    public void SetBuilding(GameObject ghostBuilding, Transform ghostTransform)
    {
        currentBuilding = Instantiate(ghostBuilding);
        currentBuilding.transform.localScale = ghostTransform.localScale;
        ApplyShader(shaderToApply);
    }

    public void BuildingIsCancel()
    {
        if(currentBuilding) { Destroy(currentBuilding); }
    }
}
