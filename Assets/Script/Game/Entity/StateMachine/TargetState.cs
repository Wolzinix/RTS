using UnityEngine;

public class TargetState : StateClassEntity
{
    public SelectableManager target;
    EntityController controller;
    NavMeshController navMeshController;

    public TargetState(SelectableManager target, EntityController controller, NavMeshController navMeshController)
    {
        this.target = target;
        this.controller = controller;
        this.navMeshController = navMeshController;
    }
    public override void Start()
    {
        controller._animator.SetBool(EntityController.Moving, true);
        if(controller.GetComponent<Rigidbody>())
        {
            controller.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        }
    }
    public override void Update()
    {
        if (target)
        {
            controller.SortTarget();
            if (Vector3.Distance(controller.gameObject.transform.position, target.transform.position) <= controller._entityManager.Range + target.size)
            {
                Stop();
                controller.AddAttackState(target);
            }
            else
            {
                if (navMeshController) { navMeshController.GetNewPath(target.transform.position); controller.moving = true; }
            }
        }
        else { End(); }
    }

    public override void End()
    {
        controller.moving = false;
        Stop();
        controller.RemoveFirstOrder();
    }

    private void Stop()
    {
        if (navMeshController != null)
        {
            controller._animator.SetBool(EntityController.Moving, false);
            controller.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
            navMeshController.StopPath();
        }
    }
}
