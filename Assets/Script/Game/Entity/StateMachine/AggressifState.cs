using UnityEngine;
public class AggressifState : MoveState
{
    public AggressifState(NavMeshController navmesh, Vector3 des, EntityController entity): base(navmesh, des, entity)
    {
        navMeshController = navmesh;
        destination = des;
        controller = entity;
    }
}
