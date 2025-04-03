using UnityEngine;

public class StuntState : StateClassEntity
{
    protected NavMeshController navMeshController;
    protected EntityController controller;
    Rigidbody rb;
    public StuntState(NavMeshController navmesh, EntityController entity)
    {
        navMeshController = navmesh;
        controller = entity;
        rb = controller.GetComponent<Rigidbody>();
    }
    public override void Start()
    {
        if (navMeshController)
        {
            controller._animator.SetBool(EntityController.Moving, false);
            controller._animator.SetInteger(EntityController.Attacking, 0);
            rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
            navMeshController.StopPath();
        }
    }
    public override void Update()
    {
        if (navMeshController)
        {
            controller._animator.SetBool(EntityController.Moving, false);
            controller._animator.SetInteger(EntityController.Attacking, 0);
            rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
            navMeshController.StopPath();
        }
    }

    public override void End()
    {
        controller.RemoveFirstOrder();
    }
}

