using LazySquirrelLabs.MinMaxRangeAttribute;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
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

    public void SpawnObject()
    {
        boxCollider = GetComponent<BoxCollider>();
        DestroyAllGameObject();
        Vector3 currentPosition = gameObject.transform.position;

        size = getSize();

        Vector3 boxSize = boxCollider.size / 2;


        for (int i = 0; i < nbOfSpawningItem; i++)
        {
            float x = currentPosition.x + Random.Range(-boxSize.x, boxSize.x);
            float z = currentPosition.z + Random.Range(-boxSize.z, boxSize.z);
            Vector3 position = new Vector3(x, currentPosition.y + boxSize.y/2, z);
            position = RayToTuchGround(position);
            int w = 0;
            while(DoAOverlap(position) > 2 || position == transform.position || w <= NumberOfTentative)
            {
                x = currentPosition.x + Random.Range(-boxSize.x, boxSize.x);
                z = currentPosition.z + Random.Range(-boxSize.z, boxSize.z);
                position = new Vector3(x, currentPosition.y + boxSize.y / 2, z);
                position = RayToTuchGround(position);
                w += 1;
            }
            if (DoAOverlap(position) <= 2 && position != transform.position)
            {
                if(ObjectStorage)
                {
                    spawningItems.Add(Instantiate(spawningGameObject, position, gameObject.transform.rotation,ObjectStorage.transform));
                }
                else
                {
                    spawningItems.Add(Instantiate(spawningGameObject, position, gameObject.transform.rotation));
                }
                float multiple = Random.Range(sizeMultiplicator.x, sizeMultiplicator.y);
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
        Ray ray = new Ray(pos, Vector3.down);
        RaycastHit[] hits = Physics.RaycastAll(ray, boxCollider.size.y);

        foreach (RaycastHit hit in hits)
        {
            Debug.DrawLine(pos, hit.point, Color.red, 10f);
            if (hit.collider.gameObject.GetComponent<Terrain>() || hit.collider.gameObject.GetComponent<NavMeshSurface>())
            {
                return new Vector3(pos.x, hit.point.y, pos.z);
            }
        }
        return transform.position;
    }

    public void ClearList()
    {
        spawningItems.Clear();
    }

    private int DoAOverlap(Vector3 spawnPosition)
    {
        return Physics.OverlapSphere(spawnPosition, MeterBetween2Object + size).Length;
    }
}
