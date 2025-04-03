using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ProjectileActiveCapacity : ActiveCapacity
{
    [SerializeField] ProjectilManager projectile;
    private AggressifEntityManager entity;

    protected override void Start()
    {
        base.Start();
        entity = GetComponentInParent<AggressifEntityManager>();
    }
    override protected void DoEffect()
    {
        ProjectilManager pj = Instantiate(projectile);
        pj.SetTarget(entityAffected.gameObject);
        pj.SetInvoker(entity);

        Vector3 spawnPosition = new(transform.position.x, transform.position.y + 1, transform.position.z);

        pj.gameObject.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z);
    }
}
