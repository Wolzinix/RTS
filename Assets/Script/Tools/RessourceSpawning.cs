using System.Collections.Generic;
using UnityEngine;

public class RessourceSpawning : MonoBehaviour
{
    [SerializeField] private GameObject spawningGameObject;
    public int nbOfSpawningItem;
    public float MeterBetween2Object;

    public List<GameObject> spawningItems;

    public BoxCollider boxCollider;

    public void DoA()
    {
        boxCollider = GetComponent<BoxCollider>();
        DestroyAllGameObject();
        Vector3 currentPosition = gameObject.transform.position;

        Vector3 boxSize = boxCollider.size / 2;

        
        for(int i = 0; i < nbOfSpawningItem; i++)
        {
            float x = currentPosition.x + Random.Range(-boxSize.x, boxSize.x);
            float z = currentPosition.z + Random.Range(-boxSize.z, boxSize.z);
            spawningItems.Add(Instantiate(spawningGameObject,new Vector3(x,currentPosition.y,z),gameObject.transform.rotation));
        }
    }

    public void DestroyAllGameObject()
    {
        foreach(GameObject item in spawningItems)
        {
            DestroyImmediate(item);
        }
        spawningItems.Clear();
    }

    public void ClearList()
    {
        spawningItems.Clear();
    }
}
