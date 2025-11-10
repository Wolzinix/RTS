using UnityEngine;

public class MoveToDistanceState : MoveState
{
    private float range;
    public MoveToDistanceState(NavMeshController navmesh, Vector3 des, EntityController entity): base(navmesh, des, entity)
    {
         range = controller.GetComponent<AggressifEntityManager>().Range - navMeshController.HaveStoppingDistance() > 0
            ? controller.GetComponent<AggressifEntityManager>().Range - navMeshController.HaveStoppingDistance()
            : 0;
    }

    public MoveToDistanceState(NavMeshController navmesh, Vector3 des, EntityController entity,float range) : base(navmesh, des, entity)
    {
        this.range = range;
    }
    public override void Update()
    {
        if (controller.IsMoving() == false)
        {
            controller.PlayAnimation((int)AnimationController.AnimType.Move);
        }
        if (navMeshController != null)
        {
            if (navMeshController.NotOnTraject())
            {
                navMeshController.GetNewPath(destination);
            }

            if (Vector3.Distance(controller.gameObject.transform.position, destination) <= navMeshController.HaveStoppingDistance() + range) { End(); }
        }
        else { End(); }

    }
}
