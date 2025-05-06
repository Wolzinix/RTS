using Assets.Script.Tools;
using LazySquirrelLabs.MinMaxRangeAttribute;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;

public class PropsSpawningCPU : MonoBehaviour
{
    [SerializeField] protected List<GameObject> spawningGameObjects;
    [SerializeField] protected int nbOfSpawningItem;
    [SerializeField] protected LayerMask layer;
    [SerializeField, MinMaxRange(0f, 3f)] protected Vector2 sizeMultiplicator;
    [SerializeField] List<TerrainLayer> terrainLayer;
    [SerializeField] int NumberOfTentative;

    protected BoxCollider boxCollider;

    public int nbOfObject;
    public virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider>();

        for (int i = 0; i < nbOfSpawningItem; i++)
        {
            float x = Random.Range(boxCollider.bounds.min.x, boxCollider.bounds.max.x);
            float z = Random.Range(boxCollider.bounds.min.z, boxCollider.bounds.max.z);
            float size = Random.Range(sizeMultiplicator.x, sizeMultiplicator.y);
            Quaternion quaternion = Quaternion.Euler(0, Random.Range(0, 180), 0);

            Vector3 position = new (x, boxCollider.bounds.max.y, z);
            position = RayToTuchGroundWithMapLayer(position);

            int w = 0;
            while ((Physics.CheckSphere(position, size, ~(layer + gameObject.layer)) == false || position == Vector3.zero) && w <= NumberOfTentative)
            {
                x = Random.Range(boxCollider.bounds.min.x, boxCollider.bounds.max.x);
                z = Random.Range(boxCollider.bounds.min.z, boxCollider.bounds.max.z);
                position = new Vector3(x, boxCollider.bounds.max.y, z);
                position = RayToTuchGroundWithMapLayer(position);
                w += 1;
            }

            if (position == Vector3.zero) { continue; }
            if (Physics.CheckSphere(position, size, ~(layer + gameObject.layer)) == false) {  continue; }

            GameObject go = Instantiate(spawningGameObjects[Random.Range(0, spawningGameObjects.Count)], position,quaternion, gameObject.transform);
            go.transform.localScale *= size;
            go.layer = gameObject.layer;
            nbOfObject += 1;
        }
    }

    public Vector3 RayToTuchGroundWithMapLayer(Vector3 pos)
    {
        if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, boxCollider.size.y, layer))
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

                    if (terrainLayer.Contains( terrain.terrainData.terrainLayers[texindex] )) { return new Vector3(pos.x, hit.point.y, pos.z); }
                }
                else { return new Vector3(pos.x, hit.point.y, pos.z); }
            }
        }
        return new Vector3();
    }

}