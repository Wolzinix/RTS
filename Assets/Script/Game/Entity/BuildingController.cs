using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BuildingController : MonoBehaviour
{
    [SerializeField] private Dictionary<EntityController, List<float>> entityDictionary;
    
    void Start()
    {
        entityDictionary = new Dictionary<EntityController, List<float>>();
    }

    private void Update()
    {
        Physics.SyncTransforms();
        foreach (EntityController i in entityDictionary.Keys)
        {
            entityDictionary[i][0] -= Time.deltaTime;
            if (entityDictionary[i][0] <= 0)
            {
                SpawnEntity(i);
            }
        }
    }

    private void SpawnEntity(EntityController entityToSpawn)
    {
        EntityController newEntity = Instantiate(entityToSpawn,
            new Vector3(transform.position.x + 2, transform.position.y, transform.position.z), 
            transform.rotation);

        newEntity.gameObject.tag = transform.tag;
    }
}
