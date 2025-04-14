using LazySquirrelLabs.MinMaxRangeAttribute;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class PropsSpawningCPU : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawningGameObjects;
    [SerializeField] private int nbOfSpawningItem;

    [SerializeField, MinMaxRange(0f, 10f)] private Vector2 sizeMultiplicator;
    private BoxCollider boxCollider;
    [SerializeField] private LayerMask layer;
    public int nbOfObject;
    public void Start()
    {
        boxCollider = GetComponent<BoxCollider>();

        for (int i = 0; i < nbOfSpawningItem; i++)
        {
            float x = Random.Range(boxCollider.bounds.min.x, boxCollider.bounds.max.x);
            float z = Random.Range(boxCollider.bounds.min.z, boxCollider.bounds.max.z);
            float size = Random.Range(sizeMultiplicator.x, sizeMultiplicator.y);
            Quaternion quaternion = Quaternion.Euler(0, Random.Range(0, 180), 0);

            Vector3 position = new Vector3(x, boxCollider.bounds.max.y, z);
            position = RayToTuchGround(position);
            if (position == Vector3.zero) { continue; }
            if (Physics.CheckSphere(position, size, ~(layer + gameObject.layer)) == false) {  continue; }

            GameObject go = Instantiate(spawningGameObjects[Random.Range(0, spawningGameObjects.Count)], position,quaternion, gameObject.transform);
            go.transform.localScale *= size;
            go.layer = gameObject.layer;
            nbOfObject += 1;
        }
    }

    public Vector3 RayToTuchGround(Vector3 pos)
    {
        RaycastHit hit;

        if (Physics.Raycast(pos, Vector3.down, out hit, boxCollider.size.y, layer))
        {
            if (hit.collider.gameObject.GetComponent<Terrain>() || hit.collider.gameObject.GetComponent<NavMeshSurface>())
            {
                if (hit.collider.gameObject.GetComponent<Terrain>())
                {
                    Terrain terrain = hit.collider.gameObject.GetComponent<Terrain>();
                    float[,,] splatmap = terrain.terrainData.GetAlphamaps(
                        Mathf.FloorToInt((pos.x - terrain.transform.position.x) / terrain.terrainData.size.x * terrain.terrainData.alphamapWidth),
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

                    if (terrain.terrainData.terrainLayers[texindex].name == "NewLayer") { return new Vector3(pos.x, hit.point.y, pos.z); }
                }
                else { return new Vector3(pos.x, hit.point.y, pos.z); }
            }
        }
        return new Vector3();
    }

}