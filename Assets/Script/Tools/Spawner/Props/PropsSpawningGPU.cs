using Assets.Script.Tools;
using System.Collections.Generic;
using UnityEngine;

public class PropsSpawningGPU : PropsSpawningCPU
{
    [SerializeField] private int LOD;

    private Matrix4x4[] matrice;
    private Mesh mesh;
    private RenderParams rp;

    private GameObject GetGameObject()
    {
        return spawningGameObjects[LOD];
    }

    private void CreateMatrix(int NumberOfSpawn,List<Vector3> listOfPosition, Vector3 scale)
    {
        matrice = new Matrix4x4[NumberOfSpawn];
        foreach (Vector3 i in listOfPosition)
        {
            Quaternion quaternion = Quaternion.Euler(0, Random.Range(0, 360), 0);
            Vector3 sizeVector = scale * Random.Range(sizeMultiplicator.x, sizeMultiplicator.y);
            matrice[listOfPosition.IndexOf(i)] = Matrix4x4.TRS(i, quaternion, sizeVector);
        }
    }

    private void CreatePostion(GameObject spawningGO)
    {
        int actualSpawn = 0;
        float size = mesh.bounds.size.y / 4;

        List<Vector3> listOfPosition = new();
        for (int i = 0; i < nbOfSpawningItem; i++)
        {
            float x = Random.Range(boxCollider.bounds.min.x, boxCollider.bounds.max.x);
            float z = Random.Range(boxCollider.bounds.min.z, boxCollider.bounds.max.z);

            Vector3 position = new(x, boxCollider.bounds.max.y, z);

            position = RayCast.RayToTuchGroundWithMapLayer(position, boxCollider.size.y, layer, terrainLayer);

            if (position != Vector3.zero)
            {
                position.y += size;
                listOfPosition.Add(position);
                actualSpawn += 1;
            }
        }

        CreateMatrix(actualSpawn, listOfPosition, spawningGO.transform.localScale);
    }

    public override void Start()
    {
        GameObject spawningGO = GetGameObject();
        if(spawningGO)
        {
            
            boxCollider = GetComponent<BoxCollider>();
            mesh = spawningGO.GetComponentInChildren<MeshFilter>().sharedMesh;

            CreatePostion(spawningGO);

            MeshRenderer meshRender = spawningGO.GetComponentInChildren<MeshRenderer>();
            meshRender.sharedMaterial.enableInstancing = true;
            rp = new RenderParams(meshRender.sharedMaterial);
        }
    }
    public void SpawnObject()
    {
        Graphics.RenderMeshInstanced(rp, mesh, 0, matrice);
    }
    private void Update()
    {
        if(LOD < spawningGameObjects.Count)
        {
            SpawnObject();
        }
    }
}