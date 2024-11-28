using UnityEngine;

public class AggressifEntityManager : SelectableManager
{
    [HideInInspector] public RessourceController ressources;

    [SerializeField] private float attack = 1;
    [SerializeField] private float attackSpeed = 1;
    [SerializeField] private float range = 1;

    private static readonly int AttackSpeedAnim = Animator.StringToHash("AttackSpeed");
    [SerializeField] public StateEffect effect;
    protected override void Awake()
    {
        base.Awake();

        foreach (RessourceController i in FindObjectsOfType(typeof(RessourceController), false))
        {
            if (i.gameObject.CompareTag(gameObject.tag))
            {
                ressources = i;
            }
        }

        if (GetComponentInChildren<Animator>())
        {
            _animator.SetFloat(AttackSpeedAnim, attackSpeed);
        }
    }

    public void AddToRessourcesKilledEntity(int gold, int wood)
    {
        ressources.AddGold(gold);
        ressources.AddWood(wood);
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

    public void AddRange(float nb)
    {
        range += nb;
    }

    public void DoAttack(EntityManager entityToAttack)
    {
        entityToAttack.TakeDamage(this, attack);
        if (entityToAttack.GetType() == typeof(SelectableManager))
        {
            SelectableManager entityToAttack2 = (SelectableManager)entityToAttack;
            entityToAttack2.TakingDamageFromEntity.Invoke(this);
        }
    }
}
