using UnityEngine;
using UnityEngine.Events;

public class SelectableManager : EntityManager
{
    [SerializeField] private float seeRange = 3;
  
    public UnityEvent<SelectableManager> deathEvent = new UnityEvent<SelectableManager>();
    public UnityEvent<AggressifEntityManager> TakingDamageFromEntity = new UnityEvent<AggressifEntityManager>();

    public float SeeRange
    {
        get => seeRange;
        set => seeRange = value;
    }

    override protected void Awake()
    {
        base.Awake();
        if (GetComponentInChildren<Animator>())
        {
            _animator = GetComponentInChildren<Animator>();
        }
    }

    public override void TakeDamage(AggressifEntityManager entity, float nb)
    {
        base.TakeDamage(entity, nb);

        changeStats.Invoke();

        if (hp <= 0)
        {
            entity.AddToRessourcesKilledEntity(GoldAmount, WoodAmount);
            Death();
            
        }


    }


    public override void AddHp(float hp)
    {
        base.AddHp(hp);
        changeStats.Invoke();
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
