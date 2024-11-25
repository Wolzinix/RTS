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
            pj.gameObject.transform.position = new Vector3(controller.transform.position.x, controller.transform.position.y + 1, controller.transform.position.z);
        }
        else { controller._entityManager.DoAttack(target); new PoisonState(target, 5, 1); }
    }

    public override void Update()
    {
        if (target)
        {
            if (Vector3.Distance(controller.gameObject.transform.position, target.transform.position) <= controller._entityManager.Range + target.size)
            {
                Animator _animator = controller._animator;
                _animator.SetBool(EntityController.Moving, false);

                if (!_animator.IsInTransition(0) &&
                    _animator.GetInteger(EntityController.Attacking) == 1 &&
                    _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5 &&
                    _attacking)
                {
                    DoAttack();
                    _attacking = false;
                }

                if (!_animator.IsInTransition(0) &&
                   _animator.GetInteger(EntityController.Attacking) == 1 &&
                    _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)

                { _animator.SetInteger(EntityController.Attacking, 0); }

                else
                {
                    controller.gameObject.transform.LookAt(target.transform);
                    _animator.SetInteger(EntityController.Attacking, 1);
                }

                if (_animator.IsInTransition(0) && _animator.GetInteger(EntityController.Attacking) == 1) { _attacking = true; }
            }
            else { end(); }
        }
        else { end(); }
    }
    public override void end()
    {
        controller.RemoveFirstOrder();
        controller._animator.SetInteger(EntityController.Attacking, 0);
    }
}
