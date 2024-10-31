using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshController : MonoBehaviour
{
    private NavMeshAgent _navMesh;
    private NavMeshPath _navPath ;
    private NavMeshObstacle _navObstacle;

    private Vector3 _destination;
    private float _stoppingDistance;
    private float _speed;

    void Start()
    { 
        _navMesh = GetComponent<NavMeshAgent>();
        _navObstacle = GetComponent<NavMeshObstacle>();
        MeshRenderer meshrender = GetComponentInChildren<MeshRenderer>();
        _navPath = new NavMeshPath();

        _navMesh.stoppingDistance = meshrender.bounds.size.x + meshrender.bounds.size.z;


        _navMesh.updatePosition = false;
        _navMesh.updateRotation = false;
        _stoppingDistance = SetStoppingDistance();
        _speed = SetSpeed();
    }
    public bool notOnTraject()
    {
        return !notAtLocation() || _destination == Vector3.zero;
    }

    private float SetStoppingDistance()
    {
        if(_navMesh) 
        {
            _navMesh.enabled = true;
            return _navMesh.stoppingDistance; 
        }
        else { return 0f; }
    }

    private float SetSpeed()
    {
        if (_navMesh)
        {
            _navMesh.enabled = true;
            return _navMesh.speed;
        }
        else { return 0f; }
    }
    public float HaveStoppingDistance()
    {
        return _stoppingDistance;
    }

    private void FixedUpdate()
    { 
        if(Vector3.Distance(transform.position,_destination) > _stoppingDistance)
        {
            _navMesh.enabled = false;
            _navObstacle.enabled = true;
            SetNextPosition();
        }
        else
        {
            StopPath();
        }
    }

    private void SetNextPosition()
    {
        GetNewPath(_destination);
        if (_navPath.corners.Length > 1)
        {
            transform.LookAt(_navPath.corners[1]);
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(_navPath.corners[1].x, transform.position.y, _navPath.corners[1].z) , _speed * Time.deltaTime);
        }
    }

    public void GetNewPath(Vector3 point)
    {
        if (_destination !=  Vector3.zero || point != Vector3.zero)
        {
            _navMesh.enabled = true;
            _navObstacle.enabled = false;
            _destination = new Vector3( point.x , transform.position.y, point.z);
            if(_navMesh.isOnNavMesh)
            {
                _navMesh.CalculatePath(point, _navPath);
            }
        }
    }

    public void ActualisePath(EntityManager target)
    {
        _destination = target.transform.position;
        SetNextPosition();
    }

    public void StopPath()
    {
        if (_navMesh != null)
        {
            if(_navMesh.path != null)
            {
                _navMesh.enabled = true;
                if (_navMesh.isOnNavMesh)
                {
                    _navMesh.ResetPath();
                }
            }
            
            _navObstacle.enabled = true;
            _navMesh.enabled = false;
            _destination = Vector3.zero;
        }

    }

    public bool notAtLocation()
    {
        bool isnotarrived = Vector3.Distance(transform.position, _destination) > _stoppingDistance && _destination == Vector3.zero;



        return isnotarrived;
    }
}
