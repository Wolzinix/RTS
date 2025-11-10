using UnityEngine;

public class StuntState : StateClassEntity
{
    protected NavMeshController navMeshController;
    protected EntityController controller;
    private readonly Rigidbody rb;
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
            controller.CancelAnimation();
            rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
            navMeshController.StopPath();
        }
    }

    public override void End()
    {
        controller.RemoveFirstOrder();
    }
}

