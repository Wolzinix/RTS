using UnityEngine;

public class EntitySpawner : MonoBehaviour
{

    [SerializeField] private string _nameOfEntity = "entity";
    [SerializeField] private GameObject _entityToSpawn;
    void Start()
    {
        if( _entityToSpawn)
        {
            GameObject go = Instantiate(_entityToSpawn,transform.parent);
            go.tag = gameObject.tag;
            go.name = _nameOfEntity;
        }
        Destroy(this);
    }

    private void getEntity()
    {

    }
}
