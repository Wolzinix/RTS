using UnityEngine;
using UnityEngine.ProBuilder;

public class PropsSpawningGPU : PropsSpawningCPU
{
    [SerializeField] private GameObject spawningGameObject;

    Matrix4x4[] matrice;
    Mesh mesh;
    RenderParams rp;
    public override void Start()
    {

        float size;
        boxCollider = GetComponent<BoxCollider>();

        if (spawningGameObject.GetComponentInChildren<ProBuilderMesh>())
        {
            spawningGameObject.GetComponentInChildren<ProBuilderMesh>().ToMesh();
            spawningGameObject.GetComponentInChildren<ProBuilderMesh>().Refresh();
            mesh = spawningGameObject.GetComponentInChildren<ProBuilderMesh>().GetComponent<MeshFilter>().sharedMesh;
            size = mesh.bounds.size.y / 4;
        }
        else
        {
            mesh = spawningGameObject.GetComponentInChildren<MeshFilter>().sharedMesh;
            size = mesh.bounds.size.y / 4;
        }

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
            Vector3 sizeVector = spawningGameObject.transform.localScale * Random.Range(sizeMultiplicator.x, sizeMultiplicator.y);
            matrice[i] = Matrix4x4.TRS(position, quaternion, sizeVector);
        }

        spawningGameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.enableInstancing = true;
        rp = new RenderParams(spawningGameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial);
    }
    private void Update()
    {
        SpawnObject();
    }
    public void SpawnObject()
    {
        Graphics.RenderMeshInstanced(rp, mesh, 0, matrice);
    }
}