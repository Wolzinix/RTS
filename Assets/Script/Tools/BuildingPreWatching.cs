using Assets.Script.Tools;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(RessourceController))]
public class BuildingPreWatching : MonoBehaviour
{
    private GameObject currentBuilding;
    [SerializeField] private Shader shaderToApply;
    [SerializeField] private Shader shaderToNotApply;
    [SerializeField] private LayerMask _ExcludeLayer;
    private RessourceController _ressourcecontroller;
    private int _gold, _wood;

    private MeshRenderer[] _meshRenderers;
    private SkinnedMeshRenderer[] _skinnedMeshRenderer;

    private void Start()
    {
        _ressourcecontroller = GetComponent<RessourceController>();
    }

    private void ApplyShader(Shader shader)
    {
        if (_meshRenderers.Length > 0)
        {
            foreach (MeshRenderer meshRender in _meshRenderers) { meshRender.material.shader = shader; }
        }
        else
        {
            foreach (SkinnedMeshRenderer meshRender in _skinnedMeshRenderer)
            {
                foreach (Material material in meshRender.materials) { material.shader = shader; }
            }
        }
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
                if (RayCast.DoASphereOverlap(position,_ExcludeLayer).Count() <= 1 && _ressourcecontroller.CompareWood(_wood) && _ressourcecontroller.CompareGold(_gold)) 
                { ApplyShader(shaderToApply); }
                else { ApplyShader(shaderToNotApply); }
            }
            else { currentBuilding.SetActive(false); }
        }
    }
    
    
    public void SetBuilding(GameObject ghostBuilding, Transform ghostTransform,int gold , int wood)
    {
        currentBuilding = Instantiate(ghostBuilding);
        currentBuilding.transform.localScale = ghostTransform.localScale;
        _meshRenderers = currentBuilding.GetComponentsInChildren<MeshRenderer>();
        if(_meshRenderers.Length <= 0)
        {
            _skinnedMeshRenderer = currentBuilding.GetComponentsInChildren<SkinnedMeshRenderer>();
        }
        ApplyShader(shaderToApply);
        _gold = gold;
        _wood = wood;
    }

    public void BuildingIsCancel()
    {
        if(currentBuilding) { Destroy(currentBuilding); }
    }
}
