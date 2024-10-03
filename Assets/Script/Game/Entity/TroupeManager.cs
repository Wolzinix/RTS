using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class TroupeManager : EntityManager
{

    [SerializeField] private float attack = 1;
    [SerializeField] private float defense = 1;
    [SerializeField] private float attackSpeed = 1;
    [SerializeField] private float speed = 2;
    [SerializeField] private float range = 1;

    [SerializeField] private float seeRange = 3;

    private static readonly int WalkSpeed = Animator.StringToHash("WalkSpeed");
    private static readonly int AttackSpeedAnim = Animator.StringToHash("AttackSpeed");

    private NavMeshAgent _navMeshAgent;

    public int PriceWhenDestroy = 1;
    public RessourceManager ressources;


    public UnityEvent<TroupeManager> deathEvent = new UnityEvent<TroupeManager>();

    public UnityEvent<TroupeManager> TakingDamageFromEntity = new UnityEvent<TroupeManager>();



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
            _animator = GetComponentInChildren<Animator>();
            _animator.SetFloat(WalkSpeed, speed);
            _animator.SetFloat(AttackSpeedAnim, attackSpeed);
        }

        foreach (RessourceManager i in Resources.FindObjectsOfTypeAll<RessourceManager>())
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

    public float SeeRange
    {
        get => seeRange;
        set => seeRange = value;
    }


    public float Attack
    {
        get => attack;
        set => attack = value;
    }

    public float Defense
    {
        get => defense;
        set => defense = value;
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

    public void AddDefense(float nb)
    {
        defense += nb;
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

    public void DoAttack(TroupeManager entityToAttack)
    {
        entityToAttack.TakeDamage(this, attack);
        entityToAttack.TakingDamageFromEntity.Invoke(this);
    }

    public override void AddHp(float hp)
    {
        base.AddHp(hp);
        changeStats.Invoke();
    }

    public override void TakeDamage(TroupeManager entity, float nb)
    {
        base.TakeDamage(entity, nb);

        changeStats.Invoke();

        if (hp <= 0)
        {
            entity.AddToRessourcesKilledEntity(PriceWhenDestroy);
        }
    }

    public void AddToRessourcesKilledEntity(int gold)
    {
        ressources.AddGold(gold);
    }

    override protected void Death()
    {
        base.Death();
        if (hp <= 0)
        {
            deathEvent.Invoke(this);
            Destroy(gameObject);
        }
        
    }
}
