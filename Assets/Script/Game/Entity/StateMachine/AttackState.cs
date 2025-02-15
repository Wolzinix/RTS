using UnityEngine;

public class AttackState : StateClassEntity
{

    EntityController controller;
    SelectableManager target;
    ProjectilManager _projectile;
    bool _attacking = false;

    public AttackState(EntityController controller, ProjectilManager projectile, SelectableManager target)
    {
        this.controller = controller;
        _projectile = projectile;
        this.target = target;
    }
    public AttackState(EntityController controller, SelectableManager target)
    {
        this.controller = controller;
        this.target = target;
    }

    public override void Start() { }

    void DoAttack()
    {
        if (_projectile)
        {
            ProjectilManager pj = EntityController.Instantiate(_projectile);
            pj.SetTarget(target.gameObject);
            pj.SetInvoker(controller.GetComponent<AggressifEntityManager>());

            Vector3 spawnPosition = new Vector3 (controller.transform.position.x,controller.transform.position.y + 1 , controller.transform.position.z);

            if(controller.pointOfSpawn) { spawnPosition = controller.pointOfSpawn.position; }

            pj.gameObject.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z);

            if(controller.GetComponent<AggressifEntityManager>().effect)
            {
                _projectile._effect = controller.GetComponent<AggressifEntityManager>().effect;
            }
        }
        else 
        { 
            controller._entityManager.DoAttack(target);
            if (controller.GetComponent<AggressifEntityManager>().effect)
            {
                controller.GetComponent<AggressifEntityManager>().effect.AddEffectToTarget(target);
            }
        }
        controller._entityManager.DoAnAttack.Invoke();
    }

    public override void Update()
    {
        if (target)
        {
            if (Vector3.Distance(controller.gameObject.transform.position, target.transform.position) <= controller._entityManager.Range + target.size)
            {
                Animator _animator = controller._animator;
                _animator.SetBool(EntityController.Moving, false);

                if (_animator.IsInTransition(0) == false &&
                    _animator.GetInteger(EntityController.Attacking) >= 1 &&
                    _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5 &&
                    _attacking)
                {
                    DoAttack();
                    _attacking = false;
                }

                if (_animator.IsInTransition(0) == false &&
                   _animator.GetInteger(EntityController.Attacking) >= 1 &&
                    _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9)

                { _animator.SetInteger(EntityController.Attacking, 0); }

                else
                {
                    controller.gameObject.transform.LookAt(new Vector3(target.transform.position.x, controller.transform.localPosition.y, target.transform.position.z));
                    controller.gameObject.transform.rotation = new Quaternion(0, controller.gameObject.transform.rotation.y, 0, controller.gameObject.transform.rotation.w);

                    if (_animator.GetInteger(EntityController.Attacking) == 0)
                    {

                        _animator.SetInteger(EntityController.Attacking, Random.Range(1, 3));
                    }
                }

                if (_animator.IsInTransition(0) && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5) { 
                    _attacking = true; }
            }
            else { End(); }
        }
        else 
        { 
            if(controller._EnnemieList.Count <= 0)
            {
                controller.GetComponent<FogWarManager>().ActualiseFog(controller,true);
            }
            End(); 
        }
    }
    public override void End()
    {
        controller.RemoveFirstOrder();
        controller._animator.SetInteger(EntityController.Attacking, 0);
    }
}
