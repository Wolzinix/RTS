using Assets.Script.Tools;
using LazySquirrelLabs.MinMaxRangeAttribute;
using System.Collections.Generic;
using UnityEngine;

public class RessourceSpawning : MonoBehaviour
{
    public int nbOfSpawningItem;
    public float MeterBetween2Object;
    public List<GameObject> spawningItems;

    [SerializeField] int NumberOfTentative;
    [SerializeField, MinMaxRange(0f, 10f)] Vector2 sizeMultiplicator;
    [SerializeField] GameObject ObjectStorage;
    [SerializeField] private GameObject spawningGameObject;
    [SerializeField] LayerMask LayerMask;

    private float size;
    private BoxCollider boxCollider;

    private void Start()
    {
        SpawnObject();
    }
    public void SpawnObject()
    {
        boxCollider = GetComponent<BoxCollider>();
        DestroyAllGameObject();

        size = getSize();

        for (int i = 0; i < nbOfSpawningItem; i++)
        {

            float multiple = Random.Range(sizeMultiplicator.x, sizeMultiplicator.y);
            float x = Random.Range(boxCollider.bounds.min.x, boxCollider.bounds.max.x);
            float z = Random.Range(boxCollider.bounds.min.z, boxCollider.bounds.max.z);
            Vector3 position = new Vector3(x, boxCollider.bounds.max.y, z);
            position = RayCast.RaycastForGround(position, LayerMask, boxCollider.size.y);
            int w = 0;
            while((DoAOverlap(position, multiple) > 2 || position == Vector3.zero) && w <= NumberOfTentative)
            {
                x = Random.Range(boxCollider.bounds.min.x, boxCollider.bounds.max.x);
                z = Random.Range(boxCollider.bounds.min.z, boxCollider.bounds.max.z);
                position = new Vector3(x, boxCollider.bounds.max.y, z);
                position = RayCast.RaycastForGround(position, LayerMask, boxCollider.size.y);
                w += 1;
            }
            if (DoAOverlap(position, multiple) <= 2 && position != Vector3.zero)
            {
                if (ObjectStorage){ spawningItems.Add(Instantiate(spawningGameObject, position, gameObject.transform.rotation,ObjectStorage.transform)); }
                else { spawningItems.Add(Instantiate(spawningGameObject, position, gameObject.transform.rotation)); }
                spawningItems[spawningItems.Count - 1].transform.localScale *= multiple;
            }
        }
    }
    private float getSize()
    {
        float size = 0;
        float nb = 0;
        foreach (Renderer i in spawningGameObject.GetComponentsInChildren<Renderer>())
        {
            size += i.bounds.size.x / 2;
            size += i.bounds.size.z / 2;
            nb += 2;
        }
        return size / nb;
    }
    public void DestroyAllGameObject()
    {
        foreach (GameObject item in spawningItems)
        {
            if (item)
            {
                DestroyImmediate(item);
            }
        }
        spawningItems.Clear();
    }
    public void ClearList()
    {
        spawningItems.Clear();
    }

    private int DoAOverlap(Vector3 spawnPosition, float multiple =1)
    {
        return Physics.OverlapSphere(spawnPosition, MeterBetween2Object + size * multiple, LayerMask).Length;
    }
}
