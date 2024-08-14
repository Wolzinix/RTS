using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityManager : MonoBehaviour
{
    private NavMeshAgent _navMesh;

    private List<Vector3> _listOfPath;

    [SerializeField] private SpriteRenderer selectedSprite;
    
    // Start is called before the first frame update
    void Start()
    {
        _navMesh = GetComponent<NavMeshAgent>();
        _listOfPath = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_navMesh.pathPending && !_navMesh.hasPath || _navMesh.remainingDistance <=1)
        {
            ActualisePath();
        }
    }

    void ActualisePath()
    {
        if (_listOfPath.Count > 0) {
            _navMesh.SetDestination(_listOfPath[0]);
            _listOfPath.RemoveAt(0);
        }
    }

    public void AddPath(Vector3 newPath)
    {
        _listOfPath.Add(newPath);
    }

    public void ClearAllPath()
    {
        _listOfPath.Clear();
    }

    public void OnSelected()
    {
        selectedSprite.gameObject.SetActive(true);
    }
    public void OnDeselected()
    {
        selectedSprite.gameObject.SetActive(false);
    }
}
