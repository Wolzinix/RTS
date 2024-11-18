using UnityEngine;
using UnityEngine.AI;

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
    public override void Start()
    {
        controller._animator.SetBool(EntityController.Moving, true);
        controller.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
    }
    public override void Update()
    {
        if(target && navMeshController)
        {
            if (navMeshController.notOnTraject())
            {
                if (Vector3.Distance(controller.gameObject.transform.position, target.transform.localPosition) >= navMeshController.HaveStoppingDistance() + 0.5)
                {
                    navMeshController.GetNewPath(target.transform.localPosition);
                    controller.moving = true;
                }
            }
        }
        else{ end(); }
    }

    public override void end()
    {
        if(navMeshController)
        {
            controller._animator.SetBool(EntityController.Moving, false);
            controller.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
            navMeshController.StopPath();

            controller.EntityIsArrive.Invoke();
        }    
    }
}
