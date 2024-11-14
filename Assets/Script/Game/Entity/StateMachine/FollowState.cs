using UnityEngine;

public class FollowState : StateClassEntity
{
    SelectableManager target;
    EntityController controller;
    NavMeshController navMeshController;
    public FollowState(SelectableManager target,NavMeshController navMeshController,EntityController entityController)
    {
        this.target = target;
        this.controller = entityController;
        this.navMeshController = navMeshController;
    }

    public override void Update()
    {
        if (navMeshController.notOnTraject())
        {
            if (Vector3.Distance(controller.gameObject.transform.position, target.transform.localPosition) >= navMeshController.HaveStoppingDistance() + 0.5)
            {
                navMeshController.GetNewPath(target.transform.localPosition);
            }
        }
    }

    public override void end()
    {
    }
}
