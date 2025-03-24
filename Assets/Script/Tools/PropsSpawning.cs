using LazySquirrelLabs.MinMaxRangeAttribute;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.ProBuilder;

public class PropsSpawning : MonoBehaviour
{
    [SerializeField] private GameObject spawningGameObject;
    public int nbOfSpawningItem;

    [SerializeField, MinMaxRange(0f, 1f)] Vector2 sizeMultiplicator;
    Matrix4x4[] matrice;
    BoxCollider boxCollider;
    Mesh mesh;
    RenderParams rp;
    float size;
    [SerializeField] LayerMask layer;
    public void Start()
    {
        boxCollider = GetComponent<BoxCollider>();

        if (spawningGameObject.GetComponentInChildren<ProBuilderMesh>())
        {
            spawningGameObject.GetComponentInChildren<ProBuilderMesh>().ToMesh();
            spawningGameObject.GetComponentInChildren<ProBuilderMesh>().Refresh();
            mesh = spawningGameObject.GetComponentInChildren<ProBuilderMesh>().GetComponent<MeshFilter>().sharedMesh;
            size = mesh.bounds.size.y/4;
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

            Vector3 position = new Vector3(x, boxCollider.bounds.max.y, z);
            position = RayToTuchGround(position);
            if (position == Vector3.zero) { continue; }

            Quaternion quaternion = Quaternion.Euler(0, Random.Range(0, 180), 0);
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
        Graphics.RenderMeshInstanced(rp,mesh, 0, matrice);
    }

    public Vector3 RayToTuchGround(Vector3 pos)
    {
        RaycastHit hit;

        if (Physics.Raycast(pos, Vector3.down, out hit, boxCollider.size.y, layer))
        {
            if (hit.collider.gameObject.GetComponent<Terrain>() || hit.collider.gameObject.GetComponent<NavMeshSurface>())
            {
                if(hit.collider.gameObject.GetComponent<Terrain>())
                {
                    Terrain terrain = hit.collider.gameObject.GetComponent<Terrain>();
                    float[,,] splatmap = terrain.terrainData.GetAlphamaps(
                        Mathf.FloorToInt((pos.x - terrain.transform.position.x)/terrain.terrainData.size.x * terrain.terrainData.alphamapWidth), 
                        Mathf.FloorToInt((pos.z - terrain.transform.position.z) / terrain.terrainData.size.z * terrain.terrainData.alphamapHeight), 
                        1, 
                        1
                    );
                    float Visible = 0;
                    int texindex = 0;
                    for (int i = 0; i < splatmap.GetLength(2); i++)
                    {
                        if (splatmap[0, 0, i] > Visible)
                        {
                            Visible = splatmap[0, 0, i];
                            texindex = i;
                        }
                    }

                    if (terrain.terrainData.terrainLayers[texindex].name == "NewLayer") {  return new Vector3(pos.x, hit.point.y + size, pos.z);  }
                }
                else { return new Vector3(pos.x, hit.point.y + size, pos.z); }
            }
        }
        return new Vector3();
    }

}
