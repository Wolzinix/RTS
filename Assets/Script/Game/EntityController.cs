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
    private List<Vector3> _listForPatrouille;
    private List<Vector3> _listForAttackingOnTravel;


    private int _patrouilleIteration = 0;

    [SerializeField] private SpriteRenderer selectedSprite;

    private EntityManager _entityManager;

    private Animator _animator;
    
    private static readonly int Moving = Animator.StringToHash("Mooving");
    private static readonly int Attacking = Animator.StringToHash("Attacking");
    

    private bool _attacking;
    
    void Start()
    {
        _navMesh = GetComponent<NavMeshAgent>();

        _listOfPath = new List<Vector3>();
        _listOfTarget = new List<EntityManager>();
        _listForFile = new List<int>();
        _listOfAllie = new List<EntityManager>();
        _listForPatrouille = new List<Vector3>();
        _listForAttackingOnTravel = new List<Vector3>();
        
        selectedSprite.gameObject.SetActive(false);
        _entityManager = GetComponent<EntityManager>();
        
        _animator = GetComponentInChildren<Animator>();
    }

    private GameObject DoCircleRaycast()
    {
        float numberOfRay = 30;
        float delta = 360 / numberOfRay;

        for (int i = 0; i < numberOfRay; i++)
        {
            Vector3 dir = Quaternion.Euler(0, i * delta,0 ) * transform.forward;
            
            Ray ray = new Ray( transform.position,dir);
            RaycastHit hit;
            
            Physics.Raycast(ray, out hit , _entityManager.SeeRange);

            if (hit.transform && !hit.transform.gameObject.CompareTag(gameObject.tag) && hit.transform.gameObject.GetComponent<EntityManager>())
            {
                Debug.DrawLine(transform.position, hit.point, Color.green,1f);
                return hit.transform.gameObject;
            }
        }

        return gameObject;
    }

    void FixedUpdate()
    {
        Physics.SyncTransforms();

        if (_listForFile.Count == 0 || _listForFile[0] == 3 || _listForFile[0] == 4)
        {
            GameObject target =DoCircleRaycast();
            if (target != gameObject)
            {
                _listOfTarget.Insert(0,target.GetComponent<EntityManager>());
                _listForFile.Insert(0,1);
            }
        }
        
        if (_navMesh.pathPending && _navMesh.hasPath || _navMesh.remainingDistance >=1) { _animator.SetBool(Moving,true);}
        else { _animator.SetBool(Moving,false);}
        
        if (_listForFile.Count > 0 && _listForFile[0] == 0)
        {
            _animator.SetBool(Attacking, false);
            if (!_navMesh.pathPending && !_navMesh.hasPath || _navMesh.remainingDistance <=1)
            {
                GetNewPath(_listOfPath[0]);
            }
        }
        
        if (_listForFile.Count > 0 && _listForFile[0] == 4)
        {
            _animator.SetBool(Attacking, false);
            if (!_navMesh.pathPending && !_navMesh.hasPath || _navMesh.remainingDistance <= 1.2)
            {
                GetNewPath(_listForAttackingOnTravel[0]);
                
                if (_navMesh.remainingDistance <= 1.2 && _navMesh.hasPath && !_navMesh.pathPending)
                {
                    _listForAttackingOnTravel.RemoveAt(0);
                    _listForFile.RemoveAt(0);
                }
            }
        }
        
        if (_listForFile.Count > 0 && _listForFile[0] == 3)
        {
            _animator.SetBool(Attacking, false);
            if (!_navMesh.pathPending && !_navMesh.hasPath || _navMesh.remainingDistance <=1)
            {
                if (_patrouilleIteration == _listForPatrouille.Count) { _patrouilleIteration = 0; }
                GetNewPath(_listForPatrouille[_patrouilleIteration]);
                _patrouilleIteration += 1;
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
                    _animator.SetBool(Moving,false);
                    
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

    void GetNewPath(Vector3 point)
    {
        _navMesh.SetDestination(point);
        if (_listForFile[0] != 3 && _listForFile[0] != 4)
        {
            _listOfPath.RemoveAt(0);
            _listForFile.RemoveAt(0);
        }
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
    
    public void AddPatrouille(Vector3 point)
    {
        _listForPatrouille.Add(point);
        if (_listForFile.Count == 0 || _listForFile[0] != 3 && _listForFile[^1] != 3)
        {
            _listForFile.Add(3);
        }
    }
    public void AddAggresifPath(Vector3 newPath)
    {
        _listForAttackingOnTravel.Add(newPath);
        _listForFile.Add(4);
    }

    private void ClearAllPath() { _listOfPath.Clear(); }

    private void ClearAllOrder() { _listOfTarget.Clear(); }
    
    private void ClearPatrouille() {_listForPatrouille.Clear();}
    
    
    private void ClearAggressifPath() {_listForAttackingOnTravel.Clear();}

    public void ClearAllFile()
    {
        _listForFile.Clear();
        ClearAllPath();
        ClearAllOrder();
        ClearPatrouille();
        ClearAggressifPath();
    }

    public void OnSelected() { selectedSprite.gameObject.SetActive(true); }
    public void OnDeselected() { selectedSprite.gameObject.SetActive(false); }

    public void StopPath()
    {
        gameObject.GetComponent<NavMeshAgent>().ResetPath();
        _animator.SetBool(Moving,false);
    }

    
}
