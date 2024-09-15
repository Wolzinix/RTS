using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EntityManager : MonoBehaviour
{
    [SerializeField] private float hp = 10;
    private float _maxHp;
    [SerializeField] private float attack = 1;
    [SerializeField] private float defense = 1;
    [SerializeField] private float attackSpeed = 1;
    [SerializeField] private float speed = 2;
    [SerializeField] private float range = 1;
    
    [SerializeField] private float seeRange = 3;

    private static readonly int WalkSpeed = Animator.StringToHash("WalkSpeed");
    private static readonly int AttackSpeedAnim = Animator.StringToHash("AttackSpeed");

    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Sprite spriteImage;

    [SerializeField] public Sprite Allisprite;
    [SerializeField] public Sprite Ennemisprite;

    public UnityEvent changeStats = new UnityEvent();
    
    public UnityEvent deathEvent = new UnityEvent();

    public UnityEvent<EntityManager> TakingDamageFromEntity = new UnityEvent<EntityManager>();
    
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;

    public Sprite GetSprit()
    {
        return spriteImage;
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

    
    
    void Awake()
    {
        if(GetComponent<NavMeshAgent>())
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

        if(CompareTag("Allie"))
        {
            sprite.sprite = Allisprite;
        }
        else if(CompareTag("ennemie"))
        {
            sprite.sprite = Ennemisprite;
        }

        sprite.gameObject.SetActive(false);

        _maxHp = hp;
    }

    public float Hp => hp;

    public void SetHp(float nb)
    {
        hp = nb;
        Death();
    }
    
    public float MaxHp
    {
        get => _maxHp;
        set => _maxHp = value;
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
        if ( _animator)
        {
            _animator.SetFloat(WalkSpeed,speed); 
        }
    }

    public void AddHp(float nb)
    {
        hp += nb;
        changeStats.Invoke();
        Death();
    }
    public void TakeDamage(float nb)
    {
        hp -= nb;
        changeStats.Invoke();
        Death();
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

    private void Death()
    {
        if (hp <= 0)
        {
            deathEvent.Invoke();
            Destroy(gameObject); 
        }
    }

    public void OnSelected() { sprite.gameObject.SetActive(true); }
    public void OnDeselected() { sprite.gameObject.SetActive(false); }

    public void DoAttack(EntityManager entityToAttack)
    {
        entityToAttack.TakeDamage(attack);
        entityToAttack.TakingDamageFromEntity.Invoke(this);
    }
}
