using System.Collections;
using UnityEngine;

public class RessourceManager : EntityManager
{
    public GPUInstancing GPUInstancing { get; set; }
    [SerializeField] GameObject Mesh;
    override protected void Awake()
    {
        base.Awake();
        if(Mesh && GPUInstancing)
        {
            Mesh.SetActive(false);
        }
    }

    public void AddInMatrice ()
    {
        GPUInstancing.AddInMatrice(transform.position, transform.rotation, transform.localScale *100);
    }
    override public void TakeDamage(AggressifEntityManager entity, float nb)
    {
        if (_animator) { StartCoroutine(DoHarvestAnimation()); }
        base.TakeDamage(entity, nb);
    }
    IEnumerator DoHarvestAnimation()
    {
        if (GPUInstancing) 
        { 
            GPUInstancing.RemoveFromMatrice(transform.position, transform.rotation, transform.localScale);
            if (Mesh)
            {
                Mesh.SetActive(true);
            }
        }
        _animator.SetBool("Harvest", true);
        if (_animator)
        {
            yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        }
        else
        {
            yield return new WaitForSeconds(0);
        }
        _animator.SetBool("Harvest", false);

        if (GPUInstancing) 
        {
            AddInMatrice();
            if (Mesh)
            {
                Mesh.SetActive(false);
            }
        }
    }
    override protected void Death()
    {
        if (hp <= 0)
        {
            if (_animator)
            {
                _animator.SetBool("Harvest", false);
                _animator.SetBool("IsDead", true);
                _animator.Play("Base Layer.TreeFall");
            }
            StartCoroutine(DoDeathAnimation());
        }
    }
    IEnumerator DoDeathAnimation()
    {
        if (_animator)
        {
            yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        }
        else
        {
            yield return new WaitForSeconds(0);
        }
        Destroy(gameObject);
    }
}
