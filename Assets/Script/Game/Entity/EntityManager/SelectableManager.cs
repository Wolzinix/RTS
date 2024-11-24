using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectableManager : EntityManager
{
    [SerializeField] private float seeRange = 3;

    [HideInInspector] public UnityEvent<SelectableManager> deathEvent = new UnityEvent<SelectableManager>();
    [HideInInspector] public UnityEvent<AggressifEntityManager> TakingDamageFromEntity = new UnityEvent<AggressifEntityManager>();

    [SerializeField] protected float xpToGive = 0.5f;

    private List<StateEffect> effects = new List<StateEffect>();

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
            if (entity.GetType() == typeof(TroupeManager)) { TroupeManager c = (TroupeManager)entity; c.AddXp(xpToGive); }
            Death();
        }
    }
    public override void TakeDamage(float nb)
    {
        base.TakeDamage(nb);

        changeStats.Invoke();

        if (hp <= 0)
        {
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

    public bool gotTheEffect(Type type)
    {
        foreach(StateEffect i in effects)
        {
            if(i.GetType() == type)
            {
                return true;
            }
        }
        return false;
    }
}
