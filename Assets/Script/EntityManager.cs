using UnityEngine;
using UnityEngine.AI;

public class EntityManager : MonoBehaviour
{
    [SerializeField] private float hp = 10;
    [SerializeField] private float attack = 1;
    [SerializeField] private float defense = 1;
    [SerializeField] private float attackSpeed = 1;
    [SerializeField] private float speed = 2;
    [SerializeField] private float range = 1;

    public float Range
    {
        get => range;
        set => range = value;
    }

    private NavMeshAgent _navMeshAgent; 
    
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.speed = speed;
    }

    public float Hp => hp;

    public void SetHp(float nb)
    {
        hp = nb;
        Death();
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
        set => attackSpeed = value;
    }

    public float Speed
    {
        get => speed;
    }

    public void SetSpeed(float nb)
    {
        speed = nb;
        _navMeshAgent.speed = speed;
    }

    public void AddHp(float nb)
    {
        hp += nb;
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
    }
    
    public void AddSpeed(float nb)
    {
        speed += nb;
        _navMeshAgent.speed = speed;
    }
    
    public void AddRange(float nb)
    {
        range += nb;
    }

    private void Death()
    {
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
