using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshController : MonoBehaviour
{
    private NavMeshAgent _navMesh;

    private Rigidbody _rb;

    private List<NavMeshAgent> _ListOfPosition = new List<NavMeshAgent>();

    void Start()
    { 
        _navMesh = GetComponent<NavMeshAgent>();
        MeshRenderer meshrender = GetComponentInChildren<MeshRenderer>();

        _rb = GetComponent<Rigidbody>();
        _navMesh.stoppingDistance = meshrender.bounds.size.x + meshrender.bounds.size.z;

        _navMesh.radius =( meshrender.bounds.size.x + meshrender.bounds.size.z) /2;

        _navMesh.updatePosition = false;
        _navMesh.updateRotation = false;
    }
    public bool notOnTraject()
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

        SetNextPosition();
        if (!notAtLocation())
        {
            StopPath();
            if (GetComponent<Rigidbody>()) { GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().freezeRotation = true;
            }

        }
    }

    private void SetNextPosition()
    {
        GetNewPath(_navMesh.destination);
        _rb.MovePosition(new Vector3(_navMesh.nextPosition.x, _rb.position.y ,_navMesh.nextPosition.z));
    }

    public void GetNewPath(Vector3 point)
    {
        if (!_navMesh.SetDestination(point))
        {
            NavMeshHit ClosestPoint;
            NavMeshPath NavPath = new NavMeshPath();
            NavMesh.SamplePosition(point, out ClosestPoint, 200, 0);
            _navMesh.CalculatePath(ClosestPoint.position, NavPath);
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
        bool isnotarrived = false;
        isnotarrived = _navMesh.remainingDistance > _navMesh.stoppingDistance;
        
        
        return isnotarrived;
    }
}
