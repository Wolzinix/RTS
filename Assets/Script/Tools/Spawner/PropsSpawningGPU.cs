using UnityEngine;
using UnityEngine.ProBuilder;

public class PropsSpawningGPU : PropsSpawningCPU
{
    [SerializeField] private int LOD;

    Matrix4x4[] matrice;
    Mesh mesh;
    RenderParams rp;

    private GameObject GetGameObject()
    {
        return spawningGameObjects[LOD];
    }
    public override void Start()
    {
        GameObject spawningGO = GetGameObject();
        if(spawningGO)
        {
            float size;
            boxCollider = GetComponent<BoxCollider>();

        
            mesh = spawningGO.GetComponentInChildren<MeshFilter>().sharedMesh;
            size = mesh.bounds.size.y / 4;
        

            matrice = new Matrix4x4[nbOfSpawningItem];
            for (int i = 0; i < nbOfSpawningItem; i++)
            {

                float x = Random.Range(boxCollider.bounds.min.x, boxCollider.bounds.max.x);
                float z = Random.Range(boxCollider.bounds.min.z, boxCollider.bounds.max.z);

                Vector3 position = new (x, boxCollider.bounds.max.y, z);
                position = RayToTuchGroundWithMapLayer(position);
                position.y += size;
                if (position == Vector3.zero) { continue; }

                Quaternion quaternion = Quaternion.Euler(0, Random.Range(0, 360), 0);
                Vector3 sizeVector = spawningGO.transform.localScale * Random.Range(sizeMultiplicator.x, sizeMultiplicator.y);
                matrice[i] = Matrix4x4.TRS(position, quaternion, sizeVector);
            }

            spawningGO.GetComponentInChildren<MeshRenderer>().sharedMaterial.enableInstancing = true;
            rp = new RenderParams(spawningGO.GetComponentInChildren<MeshRenderer>().sharedMaterial);
        }
    }
    private void Update()
    {
        if(LOD < spawningGameObjects.Count)
        {
            SpawnObject();
        }
    }
    public void SpawnObject()
    {
        Graphics.RenderMeshInstanced(rp, mesh, 0, matrice);
    }
}