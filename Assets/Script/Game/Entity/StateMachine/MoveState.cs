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
        destination = new Vector3(des.x, controller.gameObject.transform.localPosition.y, des.z);
    }
    public override void Start()
    {
        controller._animator.SetBool(EntityController.Moving, true);
        controller.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
    }
    public override void Update()
    {
        if(controller._animator.GetBool(EntityController.Moving) == false) 
        {
            controller._animator.SetBool(EntityController.Moving, true);
        }
        if (navMeshController != null)
        {
            if (navMeshController.notOnTraject())
            {
                navMeshController.GetNewPath(destination);
                controller.moving = true;
            }
            float a = Vector3.Distance(controller.gameObject.transform.localPosition, destination);
            double b = navMeshController.HaveStoppingDistance();
            if (Vector3.Distance(controller.gameObject.transform.localPosition, destination) <= navMeshController.HaveStoppingDistance() ) { End(); }
        }
        else { End(); }
    }

    public override void End()
    {
        controller.moving = false;
        controller._animator.SetBool(EntityController.Moving, false);
        controller.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        navMeshController.StopPath();

        controller.RemoveFirstOrder();
        controller.EntityIsArrive.Invoke();
        Arrived.Invoke();
    }
}
