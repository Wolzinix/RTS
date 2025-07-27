using Assets.Script.Tools;
using UnityEngine;
public class EntitySpawner : MonoBehaviour
{
    [SerializeField] private string _nameOfEntity;
    [SerializeField] private GameObject _entityToSpawn;
    [SerializeField] private IABrain _ia;
    void Awake()
    {
        if(gameObject.tag == "Allie")
        {
            GameObject entityFromSave = FindObjectsByType<SaveForNextlevel>(FindObjectsSortMode.None)[0].LoadEntity();
            if (entityFromSave)
            {
                _entityToSpawn = entityFromSave;
                _nameOfEntity = entityFromSave.GetComponent<EntityManager>().name;
            }
        }
        if( _entityToSpawn)
        {
            Spawn();
        }
        Destroy(gameObject);
    }
    private void Spawn()
    {
        GameObject go = Instantiate(_entityToSpawn, transform.parent);
        go.tag = gameObject.tag;
        if (_nameOfEntity == "") { _nameOfEntity = NameIndex.GetAName(); }
        go.name = _nameOfEntity;
        go.GetComponent<AggressifEntityManager>().ActualiseSprite();
        go.transform.position = RayCast.RaycastForGround(go, gameObject.transform.position);
        go.transform.rotation = transform.rotation;
        go.SetActive(true);
    }

    public void SetEntity(GameObject gameObject, string name ="")
    {
        _entityToSpawn = gameObject;

        _nameOfEntity = name;
    }
}
