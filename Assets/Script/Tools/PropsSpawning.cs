using LazySquirrelLabs.MinMaxRangeAttribute;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;

public class PropsSpawning : MonoBehaviour
{
    [SerializeField] private GameObject spawningGameObject;
    public int nbOfSpawningItem;

    [SerializeField, MinMaxRange(0f, 10f)] Vector2 sizeMultiplicator;
    List<Matrix4x4[]> matrices = new List<Matrix4x4[]>();
    BoxCollider boxCollider;
    List<Mesh> meshs = new List<Mesh>();
    List<RenderParams> rps = new List<RenderParams>();
    float size;
    [SerializeField] LayerMask layer;
    public void Start()
    {
        boxCollider = GetComponent<BoxCollider>();

        if (spawningGameObject.GetComponentInChildren<ProBuilderMesh>())
        {
            spawningGameObject.GetComponentInChildren<ProBuilderMesh>().ToMesh();
            spawningGameObject.GetComponentInChildren<ProBuilderMesh>().Refresh();
            foreach(MeshFilter mesh in spawningGameObject.GetComponentInChildren<ProBuilderMesh>().GetComponents<MeshFilter>())
            {
                meshs.Add(mesh.sharedMesh);

                size += mesh.sharedMesh.bounds.size.y / 4;
            }
        }
        else
        {
            foreach(MeshFilter mesh in spawningGameObject.GetComponentsInChildren<MeshFilter>())
            {
                meshs.Add(mesh.sharedMesh);
                size = mesh.sharedMesh.bounds.size.y / 4;
            }
        }

        size /= meshs.Count;

        for (int w = 0;w<meshs.Count; w++)
        {
            Matrix4x4[] matrice = new Matrix4x4[nbOfSpawningItem];
            for (int i = 0; i < nbOfSpawningItem; i++)
            {
                float x = Random.Range(boxCollider.bounds.min.x, boxCollider.bounds.max.x);
                float z = Random.Range(boxCollider.bounds.min.z, boxCollider.bounds.max.z);

                Vector3 position = new Vector3(x, boxCollider.bounds.max.y, z);

                if(w == 0) 
                {
                    position = RayToTuchGround(position);
                    if (position == Vector3.zero) { continue; }
                    Vector3 vectorToAdd = (w > 0 ? matrices[w - 1][i].GetPosition() : Vector3.zero);
                    position += meshs[w].bounds.center + vectorToAdd;

                    Quaternion quaternion = Quaternion.Euler(0, Random.Range(0, 180), 0);
                    Vector3 sizeVector = spawningGameObject.transform.localScale * Random.Range(sizeMultiplicator.x, sizeMultiplicator.y);
                    matrice[i] = Matrix4x4.TRS(position, quaternion, sizeVector);
                }
                else
                {
                    if (matrices[w - 1][i].GetPosition() == Vector3.zero) { continue; }

                    Vector3 vectorToAdd = matrices[w - 1][i].GetPosition();

                    position = meshs[w].bounds.center + vectorToAdd;
                    Vector3 sizeVector = matrices[w - 1][i].lossyScale;

                    Quaternion quaternion = matrices[w - 1][i].rotation;

                    matrice[i] = Matrix4x4.TRS(position, quaternion, sizeVector);
                }
            }
            matrices.Add(matrice);
        }
        foreach(MeshRenderer meshRenderer in spawningGameObject.GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.sharedMaterial.enableInstancing = true;
            rps.Add(new RenderParams(meshRenderer.sharedMaterial));
        }

    }
    private void Update()
    {
        SpawnObject();
    }
    public void SpawnObject()
    {
        int i = 0;
        while(i < meshs.Count)
        {
            Graphics.RenderMeshInstanced(rps[i], meshs[i], 0, matrices[i]);
            i++;
        }
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
