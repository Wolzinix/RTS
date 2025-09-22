using Assets.Script.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawnerForNumber : MonoBehaviour
{
    [SerializeField] private List<GameObject> _entityToSpawn;
    [SerializeField] private int NumberOfEntityToBeSpawn = 1;
    [SerializeField] private Transform Target;

    private int numberOfEntity;
    private void RemoveEntity(SelectableManager entity)
    {
        entity.deathEvent.RemoveListener(RemoveEntity);
        numberOfEntity--;
    }

    private GameObject SpawnEntity()
    {
        GameObject go = Instantiate(_entityToSpawn[Random.Range(0, _entityToSpawn.Count)], transform.parent);
        go.tag = gameObject.tag;
        go.name = NameIndex.GetARandomName();
        go.transform.SetPositionAndRotation(RayCast.RaycastForGround(go, gameObject.transform.position), transform.rotation);
        SelectableManager EntityManager = go.GetComponent<SelectableManager>();
        EntityManager.ActualiseSprite();
        EntityManager.deathEvent.AddListener(RemoveEntity);
        return go;

    }
    IEnumerator SpawnOneEntity()
    {
        int RandomSecond = Random.Range(1, 10);
        yield return new WaitForSeconds(RandomSecond);
        GameObject go = SpawnEntity();
        yield return new WaitForEndOfFrame();
        OrderAttackGiver.OrderGiver(go.GetComponent<EntityController>(), Target.position);
        yield return null;
    }

    void Update()
    {
        while (numberOfEntity < NumberOfEntityToBeSpawn)
        {
            numberOfEntity++;
            StartCoroutine(SpawnOneEntity());
        }
    }
}
