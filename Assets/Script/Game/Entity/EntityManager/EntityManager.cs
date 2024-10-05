using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EntityManager :  MonoBehaviour
{

    [SerializeField] protected float hp = 10;
    protected float _maxHp;

    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Sprite spriteImage;

    [SerializeField] public Sprite Allisprite;
    [SerializeField] public Sprite Ennemisprite;
    [SerializeField] public Sprite Neutralprite;

    [SerializeField] protected float defense = 1;

    public UnityEvent changeStats = new UnityEvent();

    protected Animator _animator;
    public Sprite GetSprit()
    {
        return spriteImage;
    }
  
    virtual  protected void Awake()
    {
        ActualiseSprite();

        sprite.gameObject.SetActive(false);

        _maxHp = hp;

    }

    public void ActualiseSprite()
    {
        sprite.gameObject.SetActive(true);
        if (CompareTag("Allie")) {sprite.sprite = Allisprite; }
        else if (CompareTag("ennemie")) { sprite.sprite = Ennemisprite;}
        else { sprite.sprite = Neutralprite; }

        sprite.gameObject.SetActive(false);
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


    virtual public void AddHp(float nb)
    {
        hp += nb;
        Death();
    }
    virtual public void TakeDamage(TroupeManager entity, float nb)
    {
        hp -= nb;

        Death();
    }



    virtual protected void Death()
    {
    }

    public void OnSelected() { sprite.gameObject.SetActive(true); }
    public void OnDeselected() { sprite.gameObject.SetActive(false); }

    public void AddDefense(float nb)
    {
        defense += nb;
    }
    public float Defense
    {
        get => defense;
        set => defense = value;
    }

}
