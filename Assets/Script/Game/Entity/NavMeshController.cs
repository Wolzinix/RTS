using UnityEngine;
using UnityEngine.AI;

public class NavMeshController : MonoBehaviour
{
    private NavMeshAgent _navMesh;
    private NavMeshPath _navPath ;
    private NavMeshObstacle _navObstacle;

    public Vector3 _destination;
    [HideInInspector] public float _stoppingDistance;
    public float _speed;

    void Start()
    { 
        _navMesh = GetComponent<NavMeshAgent>();
        _navObstacle = GetComponent<NavMeshObstacle>();
        MeshRenderer meshrender = GetComponentInChildren<MeshRenderer>();
        _navPath = new NavMeshPath();

        _navMesh.stoppingDistance = meshrender.bounds.size.x + meshrender.bounds.size.z;

        _navMesh.updatePosition = false;
        _navMesh.updateRotation = false;
        _navMesh.ActivateCurrentOffMeshLink(true);
        _stoppingDistance = SetStoppingDistance();
    }
    public bool notOnTraject()
    {
        return !notAtLocation() || _destination == Vector3.zero;
    }
    public bool notAtLocation()
    {
        bool isnotarrived = Vector3.Distance(transform.localPosition, _destination) > _stoppingDistance && _destination != Vector3.zero;
        return isnotarrived;
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

    public void SetSpeedWithNavMesh(float newSpeed)
    {
        _speed = newSpeed;

        if(_navObstacle && _navMesh)
        {
            _navObstacle.enabled = false;
            _navMesh.enabled = true;

            _speed = _navMesh.speed;

            _navMesh.enabled = false;
            _navObstacle.enabled = true;
        }
        
    }
    public float HaveStoppingDistance()
    {
        return _stoppingDistance;
    }
    private void LateUpdate()
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
            _navMesh.enabled = false;
        }
    }

    public void GetNewPath(Vector3 point)
    {
        if (_destination !=  Vector3.zero || point != Vector3.zero)
        {
            _navObstacle.enabled = false;
            _navMesh.enabled = true;
            _destination = new Vector3( point.x , transform.position.y, point.z);

            if(_navMesh.isOnNavMesh)
            {
                _navMesh.CalculatePath(point, _navPath);
                if(_navPath.corners.Length < 1)
                {
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(point,out hit, 10, NavMesh.AllAreas))
                    {
                        _navMesh.CalculatePath(hit.position, _navPath);
                    }
                }
            }
        }
    }

    public void ActualisePath(EntityManager target) {  _destination = target.transform.localPosition; }

    public void StopPath()
    {
        if (_navMesh != null && _destination != Vector3.zero)
        {
            if(_navMesh.path != null)
            {
                _navObstacle.enabled = false;
                _navMesh.enabled = true;
                if (_navMesh.isOnNavMesh) { _navMesh.ResetPath(); }
            }
            _navMesh.enabled = false;
            _navObstacle.enabled = true;
            _destination = Vector3.zero;
        }
    }
}
