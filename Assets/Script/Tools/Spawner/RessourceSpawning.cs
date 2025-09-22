using Assets.Script.Tools;
using LazySquirrelLabs.MinMaxRangeAttribute;
using System.Collections.Generic;
using UnityEngine;

public class RessourceSpawning : MonoBehaviour
{
    public int nbOfSpawningItem;
    public float MeterBetween2Object;
    public List<GameObject> spawningItems;

    [SerializeField] private int NumberOfTentative;
    [SerializeField, MinMaxRange(0f, 10f)] private Vector2 sizeMultiplicator;
    [SerializeField] private GameObject ObjectStorage;
    [SerializeField] private GameObject spawningGameObject;
    [SerializeField] private LayerMask LayerMask;

    private float size;
    private BoxCollider boxCollider;
    private Renderer[] renderersOfSpawning;

    private Quaternion GetNewRotation()
    {
        Vector3 EuleurRotation = new()
        {
            y = Random.Range(0, 360)
        };
        return Quaternion.Euler(EuleurRotation);
    }

    private float GetSize()
    {
        float size = 0;
        float nb = 0;
        foreach (Renderer i in renderersOfSpawning)
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
            if (item) { DestroyImmediate(item); }
        }
        spawningItems.Clear();
    }

    private Vector3 FindAPlace()
    {
        float x = Random.Range(boxCollider.bounds.min.x, boxCollider.bounds.max.x);
        float z = Random.Range(boxCollider.bounds.min.z, boxCollider.bounds.max.z);
        Vector3 position = new(x, boxCollider.bounds.max.y, z);
        return RayCast.RaycastForGround(position, LayerMask, boxCollider.size.y);
    }

    private Vector3 FindAPlaceWithTry(float multiple)
    {
        int i = 0;
        Vector3 position;
        while (i <= NumberOfTentative)
        {
            position = FindAPlace();
            i += 1;
            if (RayCast.DoASphereOverlap(position, MeterBetween2Object + size * multiple, LayerMask) <= 2 && position != Vector3.zero)
            {
                return position;
            }
        }
        return Vector3.zero;
    }
    public void SpawnObject()
    {
        DestroyAllGameObject();
        size = GetSize();

        for (int i = 0; i < nbOfSpawningItem; i++)
        {
            float multiple = Random.Range(sizeMultiplicator.x, sizeMultiplicator.y);
            Vector3 position = FindAPlaceWithTry(multiple);
            if (position != Vector3.zero)
            {
                Quaternion rotation = GetNewRotation();
                if (ObjectStorage){ spawningItems.Add(Instantiate(spawningGameObject, position, rotation, ObjectStorage.transform)); }
                else { spawningItems.Add(Instantiate(spawningGameObject, position, rotation)); }
                spawningItems[^1].transform.localScale *= multiple;
            }
        }
    }
    private void Start()
    {
        renderersOfSpawning = spawningGameObject.GetComponentsInChildren<Renderer>();
        boxCollider = GetComponent<BoxCollider>();
        SpawnObject();
    }
    public void ClearList()
    {
        spawningItems.Clear();
    }
}
