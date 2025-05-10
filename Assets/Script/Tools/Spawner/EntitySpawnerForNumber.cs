using Assets.Script.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EntitySpawnerForNumber : MonoBehaviour
{

    [SerializeField] private List<GameObject> _entityToSpawn;
    [SerializeField] private IABrain _ia;
    [SerializeField] private int NumberOfEntityToBeSpawn = 1;
    private int numberOfEntity;
    void Update()
    {
        while (numberOfEntity < NumberOfEntityToBeSpawn)
        {
            numberOfEntity++;
            StartCoroutine(SpawnOneEntity());
        }
    }

    private void removeEntity(SelectableManager entity)
    {
        AggressifEntityManager aggressifEntityManager = entity.GetComponent<AggressifEntityManager>();
        aggressifEntityManager.deathEvent.RemoveListener(removeEntity);
        numberOfEntity --;
    }

    private void SpawnEntity()
    {
        GameObject go = Instantiate(_entityToSpawn[Random.Range(0, _entityToSpawn.Count)], transform.parent);
        go.tag = gameObject.tag;
        go.name = NameIndex.GetAName();
        go.transform.SetPositionAndRotation(RayCast.RaycastForGround(go, gameObject.transform.position), transform.rotation);
        AggressifEntityManager aggressifEntityManager = go.GetComponent<AggressifEntityManager>();
        aggressifEntityManager.ActualiseSprite();
        aggressifEntityManager.deathEvent.AddListener(removeEntity);
    }
    IEnumerator SpawnOneEntity()
    {
        int RandomSecond = Random.Range(0, 10);
        yield return new WaitForSeconds(RandomSecond);
        SpawnEntity();
        yield return null;
    }
}
