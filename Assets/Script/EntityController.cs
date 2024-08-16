using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityController : MonoBehaviour
{
    private NavMeshAgent _navMesh;

    private List<Vector3> _listOfPath;

    [SerializeField] private SpriteRenderer selectedSprite;

    private EntityManager _entityManager;

    private Animator _animator;

    
    
    void Start()
    {
        _navMesh = GetComponent<NavMeshAgent>();
        _listOfPath = new List<Vector3>();
        selectedSprite.gameObject.SetActive(false);
        _entityManager = GetComponent<EntityManager>();
        _animator = GetComponentInChildren<Animator>();
        
        _animator.SetFloat("WalkSpeed",_entityManager.Speed);
    }

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
            _animator.SetBool("Mooving",true);
            _listOfPath.RemoveAt(0);
        }
        else
        {
            _animator.SetBool("Mooving",false);
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

    public void StopPath()
    {
        gameObject.GetComponent<NavMeshAgent>().ResetPath();
        _animator.SetBool("Mooving",false);
    }
}
