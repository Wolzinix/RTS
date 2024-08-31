using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityController : MonoBehaviour
{
    private NavMeshAgent _navMesh;

    private List<Vector3> _listOfPath;
    private List<EntityManager> _listOfTarget;
    
    private List<EntityManager> _listOfAllie;
    private List<int> _listForFile;

    [SerializeField] private SpriteRenderer selectedSprite;

    private EntityManager _entityManager;

    private Animator _animator;
    
    private static readonly int Mooving = Animator.StringToHash("Mooving");
    private static readonly int Attacking = Animator.StringToHash("Attacking");
    

    private bool _attacking;
    
    void Start()
    {
        _navMesh = GetComponent<NavMeshAgent>();

        _listOfPath = new List<Vector3>();
        _listOfTarget = new List<EntityManager>();
        _listForFile = new List<int>();
        _listOfAllie = new List<EntityManager>();
        
        selectedSprite.gameObject.SetActive(false);
        _entityManager = GetComponent<EntityManager>();
        
        _animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Physics.SyncTransforms();
        
        if (_navMesh.pathPending && _navMesh.hasPath || _navMesh.remainingDistance >=1) { _animator.SetBool(Mooving,true);}
        else { _animator.SetBool(Mooving,false);}
        
        if (_listForFile.Count > 0 && _listForFile[0] == 0)
        {
            _animator.SetBool(Attacking, false);
            if (!_navMesh.pathPending && !_navMesh.hasPath || _navMesh.remainingDistance <=1)
            {
                GetNewPath();
            }
        }
        
        if (_listForFile.Count > 0 && _listForFile[0] == 1)
        {
            if (!_listOfTarget[0])
            {
                _listOfTarget.RemoveAt(0);
                _listForFile.RemoveAt(0);
            }
            else
            {
                EntityManager target = _listOfTarget[0];
                
                if (Vector3.Distance(transform.position, target.transform.position) <= _entityManager.Range)
                {
                    _animator.SetBool(Mooving,false);
                    
                    if (!_animator.IsInTransition(0) &&
                        _animator.GetBool(Attacking) &&
                        _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5 &&
                        _attacking)
                     { 
                         DoAttack(target);
                         _attacking = false;
                     }

                    if (!_animator.IsInTransition(0) &&
                        _animator.GetBool(Attacking) &&
                        _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                    {
                        _animator.SetBool(Attacking, false);
                    }
                        
                        
                    else { _animator.SetBool(Attacking,true); }
                    
                    if (_animator.IsInTransition(0) && _animator.GetBool(Attacking)) { _attacking = true; }
                }
                else { ActualisePath(target); }
            }
        }

        if (_listForFile.Count > 0 && _listForFile[0] == 2)
        {
            if (!_listOfAllie[0])
            {
                _listOfAllie.RemoveAt(0);
                _listForFile.RemoveAt(0);
            }
            else
            {
                EntityManager target = _listOfAllie[0];
                if (_navMesh.remainingDistance is >= 2 or 0) { ActualisePath(target); }
            }
        }
    }

    void GetNewPath()
    {
        _navMesh.SetDestination(_listOfPath[0]);
        _listOfPath.RemoveAt(0);
        _listForFile.RemoveAt(0);
    }

    void ActualisePath(EntityManager target) { _navMesh.SetDestination(target.transform.position); }

    void DoAttack(EntityManager target) { target.AddHp(-_entityManager.Attack); }

    public void AddPath(Vector3 newPath)
    {
        _listOfPath.Add(newPath);
        _listForFile.Add(0);
    }

    public void AddTarget(EntityManager target)
    {
        _listOfTarget.Add(target);
        _listForFile.Add(1);
    }

    public void AddAllie(EntityManager target)
    {
        _listOfAllie.Add(target);
        _listForFile.Add(2);
    }
    
    public void ClearAllPath() { _listOfPath.Clear(); }

    public void ClearAllOrder() { _listOfTarget.Clear(); }

    public void ClearAllFile()
    {
        _listForFile.Clear();
        ClearAllPath();
        ClearAllOrder();
    }

    public void OnSelected() { selectedSprite.gameObject.SetActive(true); }
    public void OnDeselected() { selectedSprite.gameObject.SetActive(false); }

    public void StopPath()
    {
        gameObject.GetComponent<NavMeshAgent>().ResetPath();
        _animator.SetBool(Mooving,false);
    }
}
