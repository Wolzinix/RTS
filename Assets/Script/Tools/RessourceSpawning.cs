using LazySquirrelLabs.MinMaxRangeAttribute;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class RessourceSpawning : MonoBehaviour
{
    [SerializeField] private GameObject spawningGameObject;
    public int nbOfSpawningItem;
    public float MeterBetween2Object;

    public List<GameObject> spawningItems;
    [SerializeField] int NumberOfTentative;

    private float size;
    [SerializeField,MinMaxRange(0f, 1f)] Vector2 sizeMultiplicator;
    [SerializeField] GameObject ObjectStorage;
    
    BoxCollider boxCollider;

    [SerializeField] LayerMask LayerMask;

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
            position = RayToTuchGround(position);
            int w = 0;
            while((DoAOverlap(position, multiple) > 2 || position == Vector3.zero) && w <= NumberOfTentative)
            {
                x = Random.Range(boxCollider.bounds.min.x, boxCollider.bounds.max.x);
                z = Random.Range(boxCollider.bounds.min.z, boxCollider.bounds.max.z);
                position = new Vector3(x, boxCollider.bounds.max.y, z);
                position = RayToTuchGround(position);
                w += 1;
            }
            if (DoAOverlap(position, multiple) <= 2 && position != Vector3.zero)
            {
                if (ObjectStorage)
                {
                    spawningItems.Add(Instantiate(spawningGameObject, position, gameObject.transform.rotation,ObjectStorage.transform));
                }
                else
                {
                    spawningItems.Add(Instantiate(spawningGameObject, position, gameObject.transform.rotation));
                }
                
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
    public Vector3 RayToTuchGround(Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos, Vector3.down, out hit, boxCollider.size.y, LayerMask))
        {
            Debug.DrawLine(pos, hit.point, Color.red, 10f);
            return new Vector3(pos.x, hit.point.y, pos.z);
        }
        
        return Vector3.zero;
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
