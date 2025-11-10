using UnityEngine;

public class FollowState : StateClassEntity
{
    SelectableManager target;
    EntityController controller;
    NavMeshController navMeshController;
    public FollowState(SelectableManager target, NavMeshController navMeshController, EntityController entityController)
    {
        this.target = target;
        this.controller = entityController;
        this.navMeshController = navMeshController;
    }
    public override void Start()
    {
        controller.PlayAnimation((int)AnimationController.AnimType.Move);
        controller.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
    }
    public override void Update()
    {
        if (target && navMeshController)
        {
            if (navMeshController.NotOnTraject())
            {
                if (Vector3.Distance(controller.gameObject.transform.position, target.transform.localPosition) >= navMeshController.HaveStoppingDistance() + 0.5)
                {
                    navMeshController.GetNewPath(target.transform.localPosition);
                }
            }
        }
        else { End(); }
    }

    public override void End()
    {
        if (navMeshController)
        { 
            controller.CancelAnimation();
            controller.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
            navMeshController.StopPath();

            controller.EntityIsArrive.Invoke();
        }
        controller.RemoveFirstOrder();
    }
}
