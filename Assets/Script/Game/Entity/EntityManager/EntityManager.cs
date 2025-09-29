using Assets.Script.Game;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EntityManager : MonoBehaviour
{

    [Header("Attribute")]
    public EntityType entityType;
    [SerializeField] protected float defense = 0;
    [SerializeField] protected float _maxHp = 10;
    
    [Header("Cost")]
    public int GoldCost = 1;
    public int WoodCost = 1;

    [Header("Drop")]
    public int GoldLoot = 1;
    public int WoodLoot = 1;
    [SerializeField] protected float xpToGive = 0.0f;

    [Header("Sprite")]
    public Sprite Allisprite;
    public Sprite Ennemisprite;
    public Sprite Neutralprite;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Sprite spriteImage;

    [HideInInspector] public UnityEvent changeStats = new UnityEvent(); 
    [HideInInspector] public float size;

    protected Animator _animator;
    protected float hp;

    public Sprite GetSprit()
    {
        return spriteImage;
    }

    public void ActualiseSprite()
    {
        sprite.gameObject.SetActive(true);
        if (CompareTag("Allie")) { sprite.sprite = Allisprite; }
        else if (CompareTag("ennemie")) { sprite.sprite = Ennemisprite; }
        else { sprite.sprite = Neutralprite; }

        sprite.gameObject.SetActive(false);
    }
    private float GetSize()
    {
        float taille = 0;
        float nb = 0;
        foreach (Renderer i in GetComponentsInChildren<Renderer>())
        {
            taille += i.bounds.size.x / 2;
            taille += i.bounds.size.z / 2;
            nb += 2;
        }
        return taille / nb;
    }

    virtual protected void Awake()
    {
        ActualiseSprite();

        sprite.gameObject.SetActive(false);
        _animator = GetComponentInChildren<Animator>();

        hp = _maxHp;
        size = GetSize();
    }

    public float Hp => hp;

    public void SetHp(float nb)
    {
        hp = nb;
        changeStats.Invoke();
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
    }
    virtual public void TakeDamage(AggressifEntityManager entity, float nb)
    {
        AddHp(-((nb - defense <= 0) ? 1 : (nb - defense)));

        if (hp <= 0)
        {
            entity.AddToRessourcesKilledEntity(GoldCost, WoodCost);
            if (EntityTypeCalcul.IsATroupe(entity.entityType)) { TroupeManager c = (TroupeManager)entity; c.AddXp(xpToGive); }
        }

        Death();
    }

    virtual public void TakeDamage(float nb)
    {
        AddHp(-((nb - defense <= 0) ? 1 : (nb - defense)));

        if (hp <= 0){ Death();}
    }

    virtual protected void Death() 
    {
        if(hp <=0) 
        {
            if (_animator)
            {
                _animator.SetBool("IsDead", true);
            }
            StartCoroutine(DoDeathAnimation()); 
        }
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


    public bool CanDoIt(RessourceController ressource)
    {
        return ressource.CompareGold(GoldLoot) && ressource.CompareWood(WoodLoot);
    }

    IEnumerator DoDeathAnimation()
    {
        if (_animator)
        {
            yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        }
        else
        {
            yield return new WaitForSeconds(0);
        }
        Destroy(gameObject);
    }

}
