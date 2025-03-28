﻿using UnityEngine;

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
            controller._animator.SetInteger(EntityController.Attacking, 0);
            controller.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
            navMeshController.StopPath();
        }
    }
    public override void Update()
    {
        if (navMeshController)
        {
            controller._animator.SetBool(EntityController.Moving, false);
            controller._animator.SetInteger(EntityController.Attacking, 0);
            controller.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
            navMeshController.StopPath();
        }
    }

    public override void End()
    {
        controller.RemoveFirstOrder();
    }
}

