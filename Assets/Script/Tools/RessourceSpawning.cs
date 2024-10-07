using System.Collections.Generic;
using UnityEngine;

public class RessourceSpawning : MonoBehaviour
{
    [SerializeField] private GameObject spawningGameObject;
    public int nbOfSpawningItem;
    public float MeterBetween2Object;

    public List<GameObject> spawningItems;

    public Collider collider;

    public void DoA()
    {
        collider = GetComponent<Collider>();
        DestroyAllGameObject();
        spawningItems.Add(Instantiate(spawningGameObject));
    }

    public void DestroyAllGameObject()
    {
        foreach(GameObject item in spawningItems)
        {
            DestroyImmediate(item);
        }
        spawningItems.Clear();
    }
}
