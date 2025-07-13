using UnityEngine;

public class CursorActionGestion : MonoBehaviour
{
    [SerializeField] Material goodMaterial;
    [SerializeField] Material badMaterial;
    [SerializeField] MeshRenderer meshRenderer;
    public bool bad;
    void Start()
    {
        WhatMaterialToUse();
        Destroy(gameObject,0.5f);
    }

    private void WhatMaterialToUse()
    {
        if(bad) { meshRenderer.material = badMaterial; }
        else { meshRenderer.material = goodMaterial;}
    }

}
