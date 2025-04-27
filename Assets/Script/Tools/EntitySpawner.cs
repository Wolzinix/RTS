using Assets.Script.Tools;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{

    [SerializeField] private string _nameOfEntity = "entity";
    [SerializeField] private GameObject _entityToSpawn;
    [SerializeField] private IABrain _ia;
    void Awake()
    {
        if( _entityToSpawn)
        {
            GameObject go = Instantiate(_entityToSpawn,transform.parent);
            go.tag = gameObject.tag;
            go.name = _nameOfEntity;
            go.transform.position = RayCast.RaycastForGround(go, gameObject.transform.position);
            go.transform.rotation = transform.rotation;
        }
        Destroy(gameObject);
    }

    private void getEntity()
    {

    }
}
