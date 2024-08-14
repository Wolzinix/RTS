using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    [SerializeField] private float hp = 10;
    [SerializeField] private float attack = 1;
    [SerializeField] private float defense = 1;
    [SerializeField] private float attackSpeed = 1;
    [SerializeField] private float speed = 2;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public float Hp => hp;

    public void setHp(float nb)
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
        set => speed = value;
    }

    public void addHP(float nb)
    {
        hp += nb;
        Death();
    }
    
    public void addAttack(float nb)
    {
        attack += nb;
    }
    
    public void addDefense(float nb)
    {
        defense += nb;
    }
    
    public void addAttackSpeed(float nb)
    {
        attackSpeed += nb;
    }
    
    public void addSpeed(float nb)
    {
        speed += nb;
    }

    private void Death()
    {
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
