using UnityEngine;

public class StuntState : StateClassEntity
{
    protected NavMeshController navMeshController;
    protected EntityController controller;
    public StuntState(NavMeshController navmesh, EntityController entity)
    {
        navMeshController = navmesh;
        controller = entity;
    }
    public override void Start()
    {
        if (navMeshController)
        {
            controller._animator.SetBool(EntityController.Moving, false);
            controller._animator.SetBool(EntityController.Attacking, false);
            controller.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
            navMeshController.StopPath();
        }
    }
    public override void Update()
    {
        controller._animator.SetBool(EntityController.Moving, false);
        controller._animator.SetBool(EntityController.Attacking, false);
        controller.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        navMeshController.StopPath();
    }

    public override void end()
    {
        controller.RemoveFirstOrder();
    }
}

