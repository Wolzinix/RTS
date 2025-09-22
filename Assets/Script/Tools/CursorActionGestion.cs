using UnityEngine;

public class CursorActionGestion : MonoBehaviour
{
    public bool bad;
    [SerializeField] Material goodMaterial;
    [SerializeField] Material badMaterial;
    [SerializeField] MeshRenderer meshRenderer;
    private void WhatMaterialToUse()
    {
        if (bad) { meshRenderer.material = badMaterial; }
        else { meshRenderer.material = goodMaterial; }
    }
    void Start()
    {
        WhatMaterialToUse();
        Destroy(gameObject,0.5f);
    }
}
