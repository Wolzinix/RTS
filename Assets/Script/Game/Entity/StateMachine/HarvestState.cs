using TMPro;
using UnityEngine;

public class HarvestState : StateClassEntity
{
    RessourceManager target;
    BuilderController builder;
    bool _attacking = false;


    public HarvestState(BuilderController builderController,RessourceManager target)
    {
        this.builder = builderController;
        this.target = target;
    }

    public override void Update()
    {
        if(target)
        {
            if (Vector3.Distance(builder.transform.position, target.transform.position) <= builder._entityManager.Range + target.size)
            {
                Animator _animator = builder.GetComponentInChildren<Animator>();


                _animator.SetBool(EntityController.Moving, false);


                if (!_animator.IsInTransition(0) &&
                    _animator.GetInteger(EntityController.Attacking) == 1 &&
                    _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5 &&
                    _attacking)
                {
                    DoAnAttackOnRessource(target);
                    _attacking = false;
                }

                if (!_animator.IsInTransition(0) &&
                    _animator.GetInteger(EntityController.Attacking) == 1 &&
                    _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)

                { _animator.SetInteger(EntityController.Attacking, 0); }

                else
                {
                    builder.transform.LookAt(target.transform);
                    _animator.SetInteger(EntityController.Attacking, 1);
                }
                if (_animator.IsInTransition(0) && _animator.GetInteger(EntityController.Attacking) == 1) { _attacking = true; }
            }
            else
            {
                builder.AddPathInFirst(target.transform.localPosition);
            }
        }
        else
        {
            end();
        }
        
    }

    void DoAnAttackOnRessource(RessourceManager target)
    {
        builder._entityManager.DoAttack(target);
    }

    public override void end()
    {
        builder.RemoveFirstOrder();
        builder._animator.SetInteger(EntityController.Attacking, 0);
        builder.SearchClosetHarvestTarget();
    }
}
