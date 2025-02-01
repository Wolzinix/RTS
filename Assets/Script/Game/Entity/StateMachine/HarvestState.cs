using UnityEngine;

public class HarvestState : StateClassEntity
{
    RessourceManager target;
    BuilderController builder;
    bool _attacking = false;
    bool _endHere = false;

    float targetSize = 0;


    public HarvestState(BuilderController builderController, RessourceManager target)
    {
        builder = builderController;
        this.target = target;
    }

    public override void Update()
    {
        if (target)
        {
            if(targetSize == 0 )
            {
                calculeSizeOfTarget();
                builder._navMesh._stoppingDistance += targetSize;
            }
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
                    builder.transform.LookAt(new Vector3(target.transform.localPosition.x, builder.transform.localPosition.y, target.transform.localPosition.z));
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
            _endHere = true;
            End();
        }

    }

    void DoAnAttackOnRessource(RessourceManager target)
    {
        builder._entityManager.DoAttack(target);
    }

    public override void End()
    {
        builder._navMesh._stoppingDistance -= targetSize;
        builder.RemoveFirstOrder();
        builder._animator.SetInteger(EntityController.Attacking, 0);
        if(_endHere)
        {

            builder.SearchClosetHarvestTarget();
        }
    }

    private void calculeSizeOfTarget()
    {
        int i = 0;
        foreach (MeshRenderer x in target.GetComponentsInChildren<MeshRenderer>())
        {
            i += 1;
            targetSize += (x.bounds.size.x + x.bounds.size.z) / 2;
        }
        targetSize /= i;
    }
}
