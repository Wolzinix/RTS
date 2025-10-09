using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    public List<SelectableManager> _EnnemieList;

    [SerializeField] private SphereCollider _collider;

    [HideInInspector] public AggressifEntityManager _entityManager;
    [HideInInspector] public GroupManager groupManager;

    protected FogWarManager fog;
    protected List<GameObject> _listOfalliesOnRange;

    private List<GameObject> _ListOfCollision;

    protected virtual void Awake()
    {
        _collider.radius = gameObject.GetComponent<SelectableManager>().SeeRange;
        _entityManager = GetComponent<AggressifEntityManager>();
        fog = GetComponent<FogWarManager>();

        _listOfalliesOnRange = new List<GameObject>();
        _EnnemieList = new List<SelectableManager>();
        _ListOfCollision = new List<GameObject>();
    }
    virtual protected void AddEnnemi(SelectableManager target)
    {
        if (!_EnnemieList.Contains(target))
        {
            _EnnemieList.Add(target);
        }
    }
    virtual protected void AddAllie(GameObject target)
    {
        if (!_listOfalliesOnRange.Contains(target))
        {
            _listOfalliesOnRange.Add(target);
        }
    }

    virtual protected void RemoveEnnemi(SelectableManager target)
    {
        int i = 0;
        while ( i < _EnnemieList.Count)
        {
            if (!_EnnemieList[i] || target == _EnnemieList[i])
            {
                _EnnemieList.RemoveAt(i);
                return;
            }
            i++;
        }
    }
    virtual protected void RemoveAllie(GameObject target)
    {
        int i = 0;
        while (i < _listOfalliesOnRange.Count)
        {
            if (!_listOfalliesOnRange[i] || target == _listOfalliesOnRange[i])
            {
                _listOfalliesOnRange.RemoveAt(i);
                return;
            }
            i++;
        }
    }
    private void CollisionGestion(GameObject CollisionedObject)
    {
        if (CollisionedObject.transform && 
            !CollisionedObject.CompareTag("neutral") && 
            CollisionedObject.GetComponent<SelectableManager>() &&
            CollisionedObject != gameObject)
        {
            //Debug.DrawLine(transform.position, CollisionedObject.transform.localPosition, Color.green, 1f);
            SelectableManager target = CollisionedObject.transform.gameObject.GetComponent<SelectableManager>();

            if (!target.CompareTag(gameObject.tag)){ AddEnnemi(target);}
            else { AddAllie(CollisionedObject); }
        }
    }
    virtual protected void ClearListOfAlly(List<GameObject> list)
    {
        if (list.Count != _listOfalliesOnRange.Count)
        {
            _listOfalliesOnRange.RemoveAll(i => !list.Contains(i));
        }
    }
    

    virtual protected void SearchTarget()
    {

        foreach (GameObject hit in _ListOfCollision)
        {
            if (hit)
            {
                CollisionGestion(hit);
            }
        }
    }
    virtual protected void LateUpdate()
    {
        if (fog)
        {
            foreach (SelectableManager go in _EnnemieList)
            {
                EntityController goController = go.GetComponent<EntityController>();
                if (goController)
                {
                    fog.ActualiseFog(goController, false);
                }
            }
        }
    }
    protected virtual void OnDestroy()
    {
        foreach(SelectableManager entityController in _EnnemieList)
        {
            if(entityController && entityController.GetComponent<EntityController>())
            {
                fog.AddToFog(entityController.GetComponent<EntityController>());
            }
        }
    }

    virtual public void ClearAllOrder()
    {
        _EnnemieList.Clear();
        SearchTarget();
    }

    protected void OnTriggerEnter(Collider collision)
    {
        SelectableManager collisionSelectable = collision.GetComponent<SelectableManager>();
        if (collisionSelectable)
        {
            _ListOfCollision.Add(collision.gameObject);
            collisionSelectable.deathEvent.AddListener(RemoveToCollision);
            SearchTarget();
        }
    }

    protected void OnTriggerExit(Collider collision)
    {
        if (_ListOfCollision.Contains(collision.gameObject))
        {
            _ListOfCollision.Remove(collision.gameObject);
            collision.gameObject.GetComponent<SelectableManager>().deathEvent.RemoveListener(RemoveToCollision);
            SearchTarget();
        }
    }

    protected void RemoveToCollision(SelectableManager SM)
    {
        _ListOfCollision.Remove(SM.gameObject);
        SM.deathEvent.RemoveListener(RemoveToCollision);
        SearchTarget();
    }

    private void ClearListOfEnnemi(List<SelectableManager> list)
    {
        if (list.Count != _EnnemieList.Count)
        {
            _EnnemieList.RemoveAll(i => !list.Contains(i));
        }
    }
}
