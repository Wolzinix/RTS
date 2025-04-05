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
    void Update()
    {
        if(currentBuilding) 
        { 
            Vector3 position = RayCast.DoARayCastToGroundFromMouse().point;
            if(position != Vector3.zero ) 
            {
                foreach (MeshRenderer meshRender in currentBuilding.GetComponents<MeshRenderer>())
                {
                    if (DoAOverlap(position).Count() <= 1) {  meshRender.material.shader = shaderToApply; }
                    else { meshRender.material.shader = shaderToNotApply; }
                }
                currentBuilding.SetActive(true);
                currentBuilding.transform.position = position; 
            }
            else 
            {
                currentBuilding.SetActive(false); 
            }
            
        }
    }

    public void SetBuilding(GameObject ghostBuilding, Transform ghostTransform)
    {
        currentBuilding = Instantiate(ghostBuilding);
        currentBuilding.transform.localScale = ghostTransform.localScale;
        foreach (MeshRenderer meshRender in currentBuilding.GetComponents<MeshRenderer>() )
        {
            meshRender.material.shader = shaderToApply;
        }
    }

    public void BuildingIsCancel()
    {
        Destroy(currentBuilding);
    }
}
