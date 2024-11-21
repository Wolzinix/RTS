using System.Collections.Generic;
using UnityEngine;

public class PatrolState : StateClassEntity
{
    List<Vector3> _ListOfDestination;
    NavMeshController navMeshController;
    EntityController controller;
    int _patrolIteration = 0;

    public PatrolState(List<Vector3> listOfDestination, NavMeshController navMeshController, EntityController controller)
    {
        _ListOfDestination = listOfDestination;
        this.navMeshController = navMeshController;
        this.controller = controller;
    }
    public override void Start()
    {
        controller._animator.SetBool(EntityController.Moving, true);
        controller.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
    }
    public override void Update()
    {
        if (controller.groupManager != null && !controller.groupManager.EveryOneIsStop())
        {
            if (navMeshController.notOnTraject())
            {
                if (_patrolIteration == _ListOfDestination.Count)
                {
                    _patrolIteration = 0;
                }
                controller.AddPathInFirst(_ListOfDestination[_patrolIteration]);
                _patrolIteration += 1;
            }
        }
    }

    public void AddDestination(Vector3 destination)
    {
        _ListOfDestination.Add(destination);
    }
    public override void end()
    {
        controller.moving = false;
        controller.RemoveFirstOrder();
    }
}
