using UnityEngine;

public class MoveState : StateClassEntity
{
    NavMeshController navMeshController;
    Vector3 destination;
    EntityController controller;
    public MoveState(NavMeshController navmesh,Vector3 des, EntityController entity) 
    {
        navMeshController = navmesh;
        destination = des;
        controller = entity;
    }

    public override void Update() 
    {
        if (navMeshController.notOnTraject())
        {
            navMeshController.GetNewPath(destination);
            if (Vector3.Distance(controller.gameObject.transform.position, destination) <= navMeshController.HaveStoppingDistance() + 0.5)
            {
                end();
            }
        }
    }

    public override void end() 
    {
        controller.RemoveFirstOrder();
        controller.EntityIsArrive.Invoke();
    }
}
