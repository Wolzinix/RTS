using System.Collections;
using UnityEngine;

public class RessourceManager : EntityManager
{
    override protected void Awake()
    {
        base.Awake();
    }


    override public void TakeDamage(TroupeManager entity, float nb)
    {
        base.TakeDamage(entity, nb);

        changeStats.Invoke();
        _animator.SetBool("Harvest", true);

        if (hp <= 0)
        {
            Death();
            entity.AddToRessourcesKilledEntity(GoldAmount, WoodAmount);
        }

        
    }

    override protected void Death()
    {
        base.Death();
        if (hp <= 0)
        {
            _animator.SetBool("Harvest", false);
            _animator.SetBool("IsDead", true);
            _animator.Play("Base Layer.TreeFall");
            StartCoroutine(DoDeathAnimation());
        }
    }
    IEnumerator DoDeathAnimation()
    {
         yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }

}
