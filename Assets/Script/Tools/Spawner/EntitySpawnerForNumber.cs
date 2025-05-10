using Assets.Script.Tools;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawnerForNumber : MonoBehaviour
{

    [SerializeField] private List<GameObject> _entityToSpawn;
    [SerializeField] private IABrain _ia;
    [SerializeField] private int NumberOfEntityToBeSpawn = 1;
    private List<AggressifEntityManager> _entitysSpawn = new();
    void Update()
    {
        while (_entitysSpawn.Count < NumberOfEntityToBeSpawn)
        {
            GameObject go = Instantiate(_entityToSpawn[Random.Range(0, _entityToSpawn.Count)], transform.parent);
            go.tag = gameObject.tag;
            go.name = NameIndex.GetAName();
            go.transform.SetPositionAndRotation(RayCast.RaycastForGround(go, gameObject.transform.position), transform.rotation);
            AggressifEntityManager aggressifEntityManager = go.GetComponent<AggressifEntityManager>();
            aggressifEntityManager.ActualiseSprite();
            _entitysSpawn.Add(aggressifEntityManager);
            aggressifEntityManager.deathEvent.AddListener(removeEntity);
        }
    }

    private void removeEntity(SelectableManager entity)
    {
        AggressifEntityManager aggressifEntityManager = entity.GetComponent<AggressifEntityManager>();
        aggressifEntityManager.deathEvent.RemoveListener(removeEntity);
        _entitysSpawn.Remove(aggressifEntityManager);
    }
}
