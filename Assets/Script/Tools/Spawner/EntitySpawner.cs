using Assets.Script.Tools;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{

    [SerializeField] private string _nameOfEntity;
    [SerializeField] private GameObject _entityToSpawn;
    [SerializeField] private IABrain _ia;
    void Awake()
    {
        if( _entityToSpawn)
        {
            GameObject go = Instantiate(_entityToSpawn,transform.parent);
            go.tag = gameObject.tag;
            if(_nameOfEntity =="") { _nameOfEntity = NameIndex.GetAName(); }
            go.name = _nameOfEntity;
            go.GetComponent<AggressifEntityManager>().ActualiseSprite();
            go.transform.position = RayCast.RaycastForGround(go, gameObject.transform.position);
            go.transform.rotation = transform.rotation;
        }
        Destroy(gameObject);
    }

    private void getEntity()
    {

    }
}
