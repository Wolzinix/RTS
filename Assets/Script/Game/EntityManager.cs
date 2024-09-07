using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;

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

    [SerializeField] private Sprite sprite;

    public UnityEvent changeStats = new UnityEvent();
    
    public UnityEvent deathEvent = new UnityEvent();
    
    private Animator _animator;

    public Sprite GetSprit()
    {
        return sprite;
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

    private NavMeshAgent _navMeshAgent; 
    
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.speed = speed;
        
        _animator = GetComponentInChildren<Animator>();
        _animator.SetFloat(WalkSpeed,speed);
        _animator.SetFloat(AttackSpeedAnim, attackSpeed);

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
        _animator.SetFloat(AttackSpeedAnim, attackSpeed);
    }
    public float Speed
    {
        get => speed;
    }

    public void SetSpeed(float nb)
    {
        speed = nb;
        _navMeshAgent.speed = speed;
        _animator.SetFloat(WalkSpeed,speed);
    }

    public void AddHp(float nb)
    {
        hp += nb;
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
        _animator.SetFloat(AttackSpeedAnim, attackSpeed);
    }
    
    public void AddSpeed(float nb)
    {
        speed += nb;
        _navMeshAgent.speed = speed;
        _animator.SetFloat(WalkSpeed,speed);
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
}
