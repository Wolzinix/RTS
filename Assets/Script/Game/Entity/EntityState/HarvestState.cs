using UnityEngine;
using UnityEngine.InputSystem.XR;

public class HarvestState : StateClassEntity
{
    RessourceManager target;
    BuilderController builder;
    private bool _attacking = false;
    private bool _attackCheckOnce = false;
    bool _endHere = false;

    float targetSize = 0;
    private readonly Animator _animator;

    public HarvestState(BuilderController builderController, RessourceManager target)
    {
        builder = builderController;
        this.target = target;
        _animator = builder.GetComponentInChildren<Animator>();
        
        CalculeSizeOfTarget();
        builder._navMesh._stoppingDistance += targetSize;
    }
    private void PrepareAttack()
    {
        builder.CancelAnimation();
        _animator.Play(AnimationController.GetAttackAnimRandom());
        _attacking = true;
    }

    private void EndAttack()
    {
        builder.CancelAnimation();
        _attacking = false;
    }
    public override void Update()
    {
        if (target)
        {
            if (Vector3.Distance(builder.transform.position, target.transform.position) <= builder._entityManager.Range + target.size)
            {
                if (!_attacking) { PrepareAttack(); };
                float AnimatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                if (AnimatorStateInfo >= 0.5 &&
                    !_attackCheckOnce &&
                    _attacking)
                {
                    DoAnAttackOnRessource(target);
                    _attackCheckOnce = true;
                }

                if (AnimatorStateInfo > 1) 
                { 
                    _attacking = false;
                    _attackCheckOnce = false;
                }

                else
                {
                    builder.transform.LookAt(new Vector3(target.transform.position.x, builder.transform.position.y, target.transform.position.z));
                }
            }
            else
            {
                builder.AddPathWithRange(target.transform.position);
                _attacking = false;
            }
        }
        else
        {
            _endHere = true;
            End();
        }
    }

    

    public override void End()
    {
        builder._navMesh._stoppingDistance -= targetSize;
        builder.RemoveFirstOrder();

        EndAttack();
        if (_endHere)
        {   
            builder.SearchClosetHarvestTarget();
        }
    }
    void DoAnAttackOnRessource(RessourceManager target)
    {
        builder._entityManager.DoAttack(target);
    }
    private void CalculeSizeOfTarget()
    {
        int i = 0;
        foreach (BoxCollider x in target.GetComponentsInChildren<BoxCollider>())
        {
            i += 1;
            targetSize += (x.bounds.size.x + x.bounds.size.z) / 2;
        }
        targetSize /= i;
    }

}
