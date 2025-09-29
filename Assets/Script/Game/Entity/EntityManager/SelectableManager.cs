using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectableManager : EntityManager
{
    [Header("Shape")]
    public GameObject CurrentShape;
    public Transform CurrentShapeTransform;

    [Header("Attribute")]
    [SerializeField] private float seeRange = 3;

    [HideInInspector] public UnityEvent<StateEffect> AddEffectEvent;
    [HideInInspector] public UnityEvent<StateEffect> RemoveEffectEvent;
    [HideInInspector] public UnityEvent<SelectableManager> deathEvent = new UnityEvent<SelectableManager>();
    [HideInInspector] public UnityEvent<AggressifEntityManager> TakingDamageFromEntity = new UnityEvent<AggressifEntityManager>();

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
        RemoveEffectEvent.Invoke(effect);
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
            base.Death();
        }
    }
}
