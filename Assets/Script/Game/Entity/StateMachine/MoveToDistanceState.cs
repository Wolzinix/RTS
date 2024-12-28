using UnityEngine;

public class MoveToDistanceState : MoveState
{
    private float range;
    public MoveToDistanceState(NavMeshController navmesh, Vector3 des, EntityController entity): base(navmesh, des, entity)
    {
        range = controller.GetComponent<AggressifEntityManager>().Range;
    }

    public MoveToDistanceState(NavMeshController navmesh, Vector3 des, EntityController entity,float range) : base(navmesh, des, entity)
    {
        this.range = range;
    }

    public override void Update()
    {
        if (navMeshController != null)
        {
            if (navMeshController.notOnTraject())
            {
                navMeshController.GetNewPath(destination);
                controller.moving = true;
            }

            if (Vector3.Distance(controller.gameObject.transform.position, destination) <= navMeshController.HaveStoppingDistance() + 0.5 + range) { End(); }
        }
        else { End(); }

    }
}
