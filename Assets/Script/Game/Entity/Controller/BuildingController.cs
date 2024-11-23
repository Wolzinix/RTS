using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    private List<GameObject> _listOfalliesOnRange;
    public List<SelectableManager> _EnnemieList;

    [HideInInspector] public AggressifEntityManager _entityManager;
    [HideInInspector] public GroupManager groupManager;
    [SerializeField] private SphereCollider _collider;
    private List<GameObject> _ListOfCollision;

    void Start()
    {
        _collider.radius = gameObject.GetComponent<SelectableManager>().SeeRange;

        _listOfalliesOnRange = new List<GameObject>();

        _entityManager = GetComponent<AggressifEntityManager>();

        _EnnemieList = new List<SelectableManager>();
        _ListOfCollision = new List<GameObject>();
    }

    virtual protected void LateUpdate()
    {
        SearchTarget();
    }
    private void ClearListOfAlly(List<GameObject> list)
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
            hitGestion(hit, listOfAlly, listOfennemie);
        }
        ClearListOfEnnemi(listOfennemie);
        ClearListOfAlly(listOfAlly);
    }

    private void ClearListOfEnnemi(List<SelectableManager> list)
    {
        if (list.Count != _EnnemieList.Count) { _EnnemieList.RemoveAll(i => !list.Contains(i)); }
    }

    private void hitGestion(GameObject hit, List<GameObject> listOfAlly, List<SelectableManager> listOfennemie)
    {
        if (hit.transform && !hit.CompareTag("neutral") && hit.GetComponent<SelectableManager>())
        {
            Debug.DrawLine(transform.position, hit.transform.localPosition, Color.green, 1f);
            GameObject target = hit.transform.gameObject;

            if (target != gameObject && !target.CompareTag(gameObject.tag))
            {
                if (!_EnnemieList.Contains(target.GetComponent<SelectableManager>()))
                {
                    _EnnemieList.Add(target.GetComponent<SelectableManager>());
                }

                if (!listOfennemie.Contains(target.GetComponent<SelectableManager>()))
                {
                    listOfennemie.Add(target.GetComponent<SelectableManager>());
                }
            }

            if (target != gameObject && target.CompareTag(gameObject.tag))
            {
                if (!_listOfalliesOnRange.Contains(target))
                {
                    _listOfalliesOnRange.Add(target);
                }
                if (!listOfAlly.Contains(target)) { listOfAlly.Add(target); }
            }
        }
    }

    virtual public void ClearAllOrder()
    {
        ClearListOfAlly(new List<GameObject>());
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<SelectableManager>() != null)
        {
            _ListOfCollision.Add(collision.gameObject);
            collision.gameObject.GetComponent<SelectableManager>().deathEvent.AddListener(RemoveToCollision);
            SearchTarget();
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (_ListOfCollision.Contains(collision.gameObject))
        {
            _ListOfCollision.Remove(collision.gameObject);
            collision.gameObject.GetComponent<SelectableManager>().deathEvent.RemoveListener(RemoveToCollision);
            SearchTarget();
        }
    }

    private void RemoveToCollision(SelectableManager SM)
    {
        _ListOfCollision.Remove(SM.gameObject);
        SM.deathEvent.RemoveListener(RemoveToCollision);
        SearchTarget();
    }
}
