using UnityEngine;
using UnityEngine.AI;

public class NavMeshController : MonoBehaviour
{
    private NavMeshAgent _navMesh;
    private NavMeshPath _navPath ;
    private NavMeshObstacle _navObstacle;

    private Vector3 _destination;
    public float _stoppingDistance;
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
        SetSpeedWithNavMesh();
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

    private float GetnavMeshSpeed()
    {
        if (_navMesh)
        {
            _navMesh.enabled = true;
            return _navMesh.speed;
        }
        else { return 0f; }
    }

    public void SetSpeedWithNavMesh()
    {
        _speed = GetnavMeshSpeed();
    }
    public float HaveStoppingDistance()
    {
        return _stoppingDistance;
    }

    private void FixedUpdate()
    { 
        if(Vector3.Distance(transform.localPosition,_destination) > _stoppingDistance && _destination != Vector3.zero) { SetNextPosition(); }
        else { StopPath(); }
    }

    private void SetNextPosition()
    {
        GetNewPath(_destination);
        if (_navPath.corners.Length > 1)
        {
            transform.LookAt(new Vector3(0,_navPath.corners[1].y,0));
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, new Vector3(_navPath.corners[1].x, transform.localPosition.y, _navPath.corners[1].z) , _speed * Time.deltaTime);
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
        _destination = target.transform.localPosition;
    }

    public void StopPath()
    {
        if (_navMesh != null)
        {
            if(_navMesh.path != null)
            {
                _navMesh.enabled = true;
                if (_navMesh.isOnNavMesh) { _navMesh.ResetPath(); }
            }
            _navObstacle.enabled = true;
            _navMesh.enabled = false;
        }

        _destination = Vector3.zero;

    }

    public bool notAtLocation()
    {
        bool isnotarrived = Vector3.Distance(transform.localPosition, _destination) > _stoppingDistance && _destination == Vector3.zero;
        return isnotarrived;
    }
}
