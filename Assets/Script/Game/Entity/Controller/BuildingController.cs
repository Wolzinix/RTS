using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    public List<SelectableManager> _EnnemieList;
    [HideInInspector] public AggressifEntityManager _entityManager;
    [HideInInspector] public GroupManager groupManager;

    [SerializeField] private SphereCollider _collider;
    private List<GameObject> _ListOfCollision;

    protected FogWarManager fog;
    protected List<GameObject> _listOfalliesOnRange;
    protected virtual void Awake()
    {
        _collider.radius = gameObject.GetComponent<SelectableManager>().SeeRange;

        _listOfalliesOnRange = new List<GameObject>();

        _entityManager = GetComponent<AggressifEntityManager>();

        _EnnemieList = new List<SelectableManager>();
        _ListOfCollision = new List<GameObject>();
        fog = GetComponent<FogWarManager>();
    }

    virtual protected void LateUpdate()
    {
        SearchTarget();
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
    virtual protected void ClearListOfAlly(List<GameObject> list)
    {
        if (list.Count != _listOfalliesOnRange.Count)
        {
            _listOfalliesOnRange.RemoveAll(i => !list.Contains(i));
        }
    }

    virtual protected void SearchTarget()
    {
        List<GameObject> listOfAlly = new List<GameObject>();
        List<SelectableManager> listOfennemie = new List<SelectableManager>();

        foreach (GameObject hit in _ListOfCollision)
        {
            if (hit)
            {
                hitGestion(hit, listOfAlly, listOfennemie);
            }
        }
        ClearListOfEnnemi(listOfennemie);
        ClearListOfAlly(listOfAlly);
    }
    
    private void ClearListOfEnnemi(List<SelectableManager> list)
    {
        if (list.Count != _EnnemieList.Count) { _EnnemieList.RemoveAll(i => !list.Contains(i)); }
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

    private void hitGestion(GameObject hit, List<GameObject> listOfAlly, List<SelectableManager> listOfennemie)
    {
        if (hit.transform && !hit.CompareTag("neutral") && hit.GetComponent<SelectableManager>())
        {
            Debug.DrawLine(transform.position, hit.transform.localPosition, Color.green, 1f);
            SelectableManager target = hit.transform.gameObject.GetComponent<SelectableManager>();

            if (target.gameObject != gameObject && !target.CompareTag(gameObject.tag))
            {
                if (!_EnnemieList.Contains(target)) { AddEnnemi(target); }

                if (!listOfennemie.Contains(target)){ listOfennemie.Add(target); }
            }

            if (target.gameObject != gameObject && target.CompareTag(gameObject.tag))
            {
                if (!_listOfalliesOnRange.Contains(target.gameObject))
                {
                    _listOfalliesOnRange.Add(target.gameObject);
                }
                if (!listOfAlly.Contains(target.gameObject)) { listOfAlly.Add(target.gameObject); }
            }
        }
    }

    virtual protected void AddEnnemi(SelectableManager target)
    {
        _EnnemieList.Add(target);
    }

    virtual public void ClearAllOrder()
    {
        ClearListOfAlly(new List<GameObject>());
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
}
