using UnityEngine;

public class ProjectileActiveCapacity : ActiveCapacity
{
    [SerializeField] ProjectilManager projectile;
    override protected void DoEffect()
    {
        ProjectilManager pj = Instantiate(projectile);
        pj.SetTarget(entityAffected.gameObject);
        pj.SetInvoker(GetComponentInParent<AggressifEntityManager>());

        Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

        pj.gameObject.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z);
    }
}
