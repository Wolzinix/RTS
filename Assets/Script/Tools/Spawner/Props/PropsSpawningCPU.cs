using Assets.Script.Tools;
using LazySquirrelLabs.MinMaxRangeAttribute;
using System.Collections.Generic;
using UnityEngine;

public class PropsSpawningCPU : MonoBehaviour
{
    [HideInInspector] public int nbOfObject;

    [SerializeField] protected List<GameObject> spawningGameObjects;
    [SerializeField] protected int nbOfSpawningItem;
    [SerializeField] protected LayerMask layer;
    [SerializeField, MinMaxRange(0f, 3f)] protected Vector2 sizeMultiplicator;

    [SerializeField] protected List<TerrainLayer> terrainLayer;
    [SerializeField] int NumberOfTentative;
    [SerializeField] GPUInstancing GPUInstancing;

    protected BoxCollider boxCollider;

    private Vector3 TryForSuccesOrNumber(float size)
    {
        int w = 0;
        Vector3 position;
        while (w <= NumberOfTentative)
        {
            float x = Random.Range(boxCollider.bounds.min.x, boxCollider.bounds.max.x);
            float z = Random.Range(boxCollider.bounds.min.z, boxCollider.bounds.max.z);
            position = new Vector3(x, boxCollider.bounds.max.y, z);
            position = RayCast.RayToTuchGroundWithMapLayer(position, boxCollider.size.y, layer, terrainLayer);
            w += 1;
            if(Physics.CheckSphere(position, size, ~(layer + gameObject.layer)) == true && position != Vector3.zero)
            {
                return position;
            }
            
        }
        return Vector3.zero;
    }

    private void SpawnObject()
    {
        for (int i = 0; i < nbOfSpawningItem; i++)
        {
            float size = Random.Range(sizeMultiplicator.x, sizeMultiplicator.y);
            Quaternion quaternion = Quaternion.Euler(0, Random.Range(0, 180), 0);

            Vector3 position = TryForSuccesOrNumber(size);

            if (position == Vector3.zero) { continue; }

            GameObject go = Instantiate(spawningGameObjects[Random.Range(0, spawningGameObjects.Count)], position, quaternion, gameObject.transform);
            RessourceManager goRM = go.GetComponent<RessourceManager>();

            go.transform.localScale *= size;
            go.layer = gameObject.layer;
            if (goRM && GPUInstancing)
            {
                goRM.GPUInstancing = GPUInstancing;
                goRM.AddInMatrice();
            }
            nbOfObject += 1;
        }
    }
    public virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider>();

        SpawnObject();
    }
}