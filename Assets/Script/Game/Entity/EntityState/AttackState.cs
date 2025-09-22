using UnityEngine;

public class AttackState : StateClassEntity
{

    EntityController controller;
    SelectableManager target;
    ProjectilManager _projectile;
    bool _attacking = false;
    Animator _animator;
    private bool _attackCheckOnce = false;
    AggressifEntityManager controlelrAggressifManager;

    public AttackState(EntityController controller, ProjectilManager projectile, SelectableManager target)
    {
        this.controller = controller;
        _projectile = projectile;
        this.target = target;
        controlelrAggressifManager = controller.GetComponent<AggressifEntityManager>();
        _animator = controller._animator;
    }
    public AttackState(EntityController controller, SelectableManager target)
    {
        this.controller = controller;
        this.target = target;
        controlelrAggressifManager = controller.GetComponent<AggressifEntityManager>();
        _animator = controller._animator;
    }

    public override void Start() { }

    private void PrepareAttack()
    {
        controller.CancelAnimation();
        _animator.Play(AnimationController.GetAttackAnimRandom());
        _attacking = true;
    }

    private void EndAttack()
    {
        controller.CancelAnimation();
        _attacking = false;
    }

    void DoAttack()
    {
        if (_projectile)
        {
            ProjectilManager pj = EntityController.Instantiate(_projectile);
            pj.SetTarget(target.gameObject);
            pj.SetInvoker(controlelrAggressifManager);

            Vector3 spawnPosition = new Vector3 (controller.transform.position.x,controller.transform.position.y + 1 , controller.transform.position.z);

            if(controller.pointOfSpawn) { spawnPosition = controller.pointOfSpawn.position; }

            pj.gameObject.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z);

            if(controlelrAggressifManager.effect)
            {
                _projectile._effect = controlelrAggressifManager.effect;
            }
        }
        else 
        { 
            controller._entityManager.DoAttack(target);
            if (controlelrAggressifManager.effect)
            {
                controlelrAggressifManager.effect.AddEffectToTarget(target);
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
                if (!_attacking) { PrepareAttack(); };
                float AnimatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                if (AnimatorStateInfo >= 0.5 &&
                    AnimatorStateInfo <= 1 && 
                    _attacking &&
                    !_attackCheckOnce)
                {
                    DoAttack();
                    _attackCheckOnce = true;
                }

                if ( AnimatorStateInfo > 1 && (_attacking || _attackCheckOnce))
                { 
                    _attacking = false;
                    _attackCheckOnce = false;
                }

                else
                {
                    controller.gameObject.transform.LookAt(new Vector3(target.transform.position.x, controller.transform.localPosition.y, target.transform.position.z));
                    controller.gameObject.transform.rotation = new Quaternion(0, controller.gameObject.transform.rotation.y, 0, controller.gameObject.transform.rotation.w);
                }
            }
            else { End(); }
        }
        else 
        { 
            if(controller._EnnemieList.Count <= 0)
            {
                FogWarManager fogManager = controller.GetComponent<FogWarManager>();
                if (fogManager) { fogManager.ActualiseFog(controller, true); }
            }
            End(); 
        }
    }
    public override void End()
    {
        controller.RemoveFirstOrder();
        EndAttack();
    }
}
