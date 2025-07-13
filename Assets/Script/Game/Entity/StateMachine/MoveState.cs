using UnityEngine;
using UnityEngine.Events;

public class MoveState : StateClassEntity
{
    protected NavMeshController navMeshController;
    protected Vector3 destination;
    protected EntityController controller;
    public UnityEvent Arrived = new UnityEvent();
    public MoveState(NavMeshController navmesh, Vector3 des, EntityController entity)
    {
        navMeshController = navmesh;
        controller = entity;
        destination = new Vector3(des.x, controller.transform.position.y, des.z);
    }
    public override void Start()
    {
        controller.CancelAnimation();
        controller._animator.SetBool(AnimationController.Moving, true);
        controller.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
    }
    public override void Update()
    {
        if(controller._animator.GetBool(AnimationController.Moving) == false) 
        {
            controller.CancelAnimation();
            controller._animator.SetBool(AnimationController.Moving, true);
        }
        if (navMeshController != null)
        {
            if (navMeshController.NotOnTraject())
            {
                navMeshController.GetNewPath(destination);
                controller.moving = true;
            }
            destination.y = controller.transform.position.y;
            if (Vector3.Distance(controller.transform.position, destination) <= navMeshController.HaveStoppingDistance() ) { End(); }
        }
        else { End(); }
    }

    public override void End()
    {
        controller.moving = false;
        controller.CancelAnimation();
        controller.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        if(navMeshController)
        {
            navMeshController.StopPath();
        }
        

        controller.RemoveFirstOrder();
        controller.EntityIsArrive.Invoke();
        Arrived.Invoke();
    }
}
