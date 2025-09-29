using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle))]
public class ProductionBuildingManager : SelectableManager
{
    protected override void Awake()
    {
        base.Awake();
        GetComponent<NavMeshObstacle>().enabled = true;
    }
}
