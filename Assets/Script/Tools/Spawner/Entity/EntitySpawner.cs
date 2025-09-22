using Assets.Script.Tools;
using UnityEngine;
public class EntitySpawner : MonoBehaviour
{
    public bool IsLockToTakeSave;
    [SerializeField] private string _nameOfEntity;
    [SerializeField] private GameObject _entityToSpawn;
    [SerializeField] private AIBrain _ia;
    
    private void Spawn()
    {
        GameObject go = Instantiate(_entityToSpawn, transform.parent);
        go.tag = gameObject.tag;
        if (_nameOfEntity == "") { _nameOfEntity = NameIndex.GetARandomName(); }
        go.name = _nameOfEntity;
        go.GetComponent<AggressifEntityManager>().ActualiseSprite();
        go.transform.position = RayCast.RaycastForGround(go, gameObject.transform.position);
        go.transform.rotation = transform.rotation;
        go.SetActive(true);
    }
    void Awake()
    {
        if(gameObject.tag == "Allie" && !IsLockToTakeSave)
        {
            SaveForNextlevel save = FindObjectOfType<SaveForNextlevel>();
            if(save )
            {
                GameObject entityFromSave = save.LoadEntity();
                if (entityFromSave)
                {
                    _entityToSpawn = entityFromSave;
                    _nameOfEntity = entityFromSave.GetComponent<EntityManager>().name;
                }
            }
        }
        if( _entityToSpawn)
        {
            Spawn();
        }
        Destroy(gameObject);
    }
    public void SetEntity(GameObject gameObject, string name ="")
    {
        _entityToSpawn = gameObject;

        _nameOfEntity = name;
    }
}
