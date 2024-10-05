using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class TroupeManager : SelectableManager
{

    [SerializeField] private float attack = 1;
    [SerializeField] private float attackSpeed = 1;
    [SerializeField] private float speed = 2;
    [SerializeField] private float range = 1;


    private static readonly int WalkSpeed = Animator.StringToHash("WalkSpeed");
    private static readonly int AttackSpeedAnim = Animator.StringToHash("AttackSpeed");


    private NavMeshAgent _navMeshAgent;
    public RessourceController ressources;



    protected override void Awake()
    {
        base.Awake();

        if (GetComponent<NavMeshAgent>())
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            SetNavMeshSpeed(speed);
        }

        if (GetComponentInChildren<Animator>())
        {
            _animator.SetFloat(WalkSpeed, speed);
            _animator.SetFloat(AttackSpeedAnim, attackSpeed);
        }

        foreach (RessourceController i in Resources.FindObjectsOfTypeAll<RessourceController>())
        {
            if (i.gameObject.CompareTag(gameObject.tag))
            {
                ressources = i;
            }
        }
    }

    public float Range
    {
        get => range;
        set => range = value;
    }

    public float Attack
    {
        get => attack;
        set => attack = value;
    }


    public float AttackSpeed
    {
        get => attackSpeed;
    }

    public void SetAttackSpeed(float nb)
    {
        attackSpeed = nb;
        if (_animator)
        {
            _animator.SetFloat(AttackSpeedAnim, attackSpeed);
        }
    }
    public float Speed
    {
        get => speed;
    }

    public void SetSpeed(float nb)
    {
        speed = nb;
        SetNavMeshSpeed(nb);
        if (_animator)
        {
            _animator.SetFloat(WalkSpeed, speed);
        }
    }

    public void AddAttack(float nb)
    {
        attack += nb;
    }

   

    public void AddAttackSpeed(float nb)
    {
        attackSpeed += nb;
        if (_animator)
        {
            _animator.SetFloat(AttackSpeedAnim, attackSpeed);
        }
    }

    public void AddSpeed(float nb)
    {
        speed += nb;
        SetNavMeshSpeed(nb);
        if (_animator)
        {
            _animator.SetFloat(WalkSpeed, speed);
        }
    }

    private void SetNavMeshSpeed(float speed)
    {
        if (_navMeshAgent)
        {
            _navMeshAgent.speed = speed;

        }
    }

    public void AddRange(float nb)
    {
        range += nb;
    }

    public void DoAttack(SelectableManager entityToAttack)
    {
        entityToAttack.TakeDamage(this, attack);
        entityToAttack.TakingDamageFromEntity.Invoke(this);
    }

    public void AddToRessourcesKilledEntity(int gold)
    {
        ressources.AddGold(gold);
    }

}
