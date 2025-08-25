using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectableManager : EntityManager
{
    [Header("Shape")]
    [SerializeField] public GameObject CurrentShape;
    [SerializeField] public Transform CurrentShapeTransform;

    [HideInInspector] public UnityEvent<SelectableManager> deathEvent = new UnityEvent<SelectableManager>();
    [HideInInspector] public UnityEvent<AggressifEntityManager> TakingDamageFromEntity = new UnityEvent<AggressifEntityManager>();

    [Header("Attribute")]
    [SerializeField] private float seeRange = 3;

    [HideInInspector] public UnityEvent<StateEffect> AddEffectEvent;
    [HideInInspector] public UnityEvent RemoveEffectEvent;

    public List<StateEffect> _listOfEffects;


    public float SeeRange
    {
        get => seeRange;
        set => seeRange = value;
    }

    public void AddEffect(StateEffect effect)
    {
        _listOfEffects.Add(effect);
        AddEffectEvent.Invoke(effect);
    }

    public void RemoveEffect(StateEffect effect)
    {
        RemoveEffectEvent.Invoke();
        _listOfEffects.Remove(effect);
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
    }
    public override void TakeDamage(float nb)
    {
        base.TakeDamage(nb);
    }

    public override void AddHp(float hp)
    {
        base.AddHp(hp);
        changeStats.Invoke();
    }

    override protected void Death()
    {
        if (hp <= 0)
        {
            _listOfEffects.Clear();
            deathEvent.Invoke(this);
            Destroy(gameObject);
        }
    }
}
