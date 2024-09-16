using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshController : MonoBehaviour
{
    private NavMeshAgent _navMesh;

    void Start()
    {
        _navMesh = GetComponent<NavMeshAgent>();
        _navMesh.stoppingDistance = GetComponentInChildren<MeshRenderer>().bounds.size.x + GetComponentInChildren<MeshRenderer>().bounds.size.z;
    }
    public bool isStillOnTrajet()
    {
        return !_navMesh.pathPending && !_navMesh.hasPath || !notAtLocation();
    }

    public void GetNewPath(Vector3 point)
    {
        if(!_navMesh.SetDestination(point))
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
        gameObject.GetComponent<NavMeshAgent>().ResetPath();
    }

    public bool notAtLocation()
    {
        return _navMesh.remainingDistance > _navMesh.stoppingDistance;
    }
}
