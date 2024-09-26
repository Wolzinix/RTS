using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class NavMeshController : MonoBehaviour
{
    private NavMeshAgent _navMesh;

    void Start()
    { 
        _navMesh = GetComponent<NavMeshAgent>();
        MeshRenderer meshrender = GetComponentInChildren<MeshRenderer>();
        _navMesh.stoppingDistance = meshrender.bounds.size.x + meshrender.bounds.size.z;

        _navMesh.radius =( meshrender.bounds.size.x + meshrender.bounds.size.z) /2;
    }
    public bool isStillOnTrajet()
    {
        return !_navMesh.pathPending && !_navMesh.hasPath || !notAtLocation();
    }

    public float HaveStoppingDistance()
    {
        if(_navMesh) { return _navMesh.stoppingDistance; }
        else { return 0f; }
        
    }

    private void Update()
    {
        if(!notAtLocation())
        {
            StopPath();
            
            if (GetComponent<Rigidbody>()) { GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().freezeRotation = true;
            }

        }
    }

    public void GetNewPath(Vector3 point)
    {
        if (!_navMesh.SetDestination(point))
        {
            NavMeshHit ClosestPoint;
            NavMeshPath NavPath = new NavMeshPath();
            NavMesh.SamplePosition(point, out ClosestPoint, 200, 0);
            _navMesh.CalculatePath(ClosestPoint.position, NavPath);
            _navMesh.SetPath(NavPath);
        }
    }

    public void ActualisePath(EntityManager target)
    {

        _navMesh.SetDestination(target.transform.position);
    }

    public void StopPath()
    { 
        if(_navMesh != null)
        {

            _navMesh.ResetPath();
        }
    }

    public bool notAtLocation()
    {
        bool isnotarrived = _navMesh.remainingDistance > _navMesh.stoppingDistance;
        
        return isnotarrived;
    }
}
