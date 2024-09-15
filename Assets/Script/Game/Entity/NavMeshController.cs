using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshController : MonoBehaviour
{
    private NavMeshAgent _navMesh;

    void Start()
    {
        _navMesh = GetComponent<NavMeshAgent>();
    }
    public bool isStillOnTrajet()
    {
        return !_navMesh.pathPending && !_navMesh.hasPath || !notAtLocation();
    }

    public void GetNewPath(Vector3 point)
    {
        _navMesh.SetDestination(point);
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
