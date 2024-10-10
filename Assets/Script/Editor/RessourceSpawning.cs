using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class RessourceSpawning : MonoBehaviour
{
    [SerializeField] private GameObject spawningGameObject;
    public int nbOfSpawningItem;
    public float MeterBetween2Object;

    public List<GameObject> spawningItems;

    private float size;

    BoxCollider boxCollider;

    public void DoA()
    {
        boxCollider = GetComponent<BoxCollider>();
        DestroyAllGameObject();
        Vector3 currentPosition = gameObject.transform.position;

        size = getSize();

        Vector3 boxSize = boxCollider.size / 2;

        
        for(int i = 0; i < nbOfSpawningItem; i++)
        {
            float x = currentPosition.x + Random.Range(-boxSize.x, boxSize.x);
            float z = currentPosition.z + Random.Range(-boxSize.z, boxSize.z);
            Vector3 position = new Vector3(x, currentPosition.y, z);

            if(DoAOverlap(position) <= 2)
            {
                spawningItems.Add(Instantiate(spawningGameObject, position, gameObject.transform.rotation));
            }
        }
    }

    private float getSize()
    {
        float size = 0;
        float nb = 0;
        foreach(Renderer i in spawningGameObject.GetComponentsInChildren<Renderer>())
        {
            size += i.bounds.size.x /2;
            size += i.bounds.size.z /2;
            nb += 2;
        }
        return size / nb;
    }
    public void DestroyAllGameObject()
    {
        foreach(GameObject item in spawningItems)
        {
            if(item)
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

    private int DoAOverlap(Vector3 spawnPosition)
    {
        return Physics.OverlapSphere(spawnPosition, MeterBetween2Object + size).Length;
    }
}
