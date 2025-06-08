using Assets.Script.Game;
using UnityEngine;
using UnityEngine.Events;

public class EntityManager : MonoBehaviour
{

    [Header("Attribute")]
    [SerializeField] protected float hp = 10;
    [SerializeField] protected float defense = 0;
    protected float _maxHp;
    public EntityType entityType;
    [HideInInspector] public float size;

    [Header("Cost")]
    public int GoldAmount = 1;
    public int WoodAmount = 1;

    [Header("Drop")]
    public int GoldCost = 1;
    public int WoodCost = 1;
    [SerializeField] protected float xpToGive = 0.0f;

    [Header("Sprite")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Sprite spriteImage;
    [SerializeField] public Sprite Allisprite;
    [SerializeField] public Sprite Ennemisprite;
    [SerializeField] public Sprite Neutralprite;
    protected Animator _animator;


    [HideInInspector] public UnityEvent changeStats = new UnityEvent();

    public Sprite GetSprit()
    {
        return spriteImage;
    }

    virtual protected void Awake()
    {
        ActualiseSprite();

        sprite.gameObject.SetActive(false);
        _animator = GetComponent<Animator>();

        _maxHp = hp;
        size = GetSize();

    }

    public void ActualiseSprite()
    {
        sprite.gameObject.SetActive(true);
        if (CompareTag("Allie")) { sprite.sprite = Allisprite; }
        else if (CompareTag("ennemie")) { sprite.sprite = Ennemisprite; }
        else { sprite.sprite = Neutralprite; }

        sprite.gameObject.SetActive(false);
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
            entity.AddToRessourcesKilledEntity(GoldAmount, WoodAmount);
            if (entity.GetType() == typeof(TroupeManager)) { TroupeManager c = (TroupeManager)entity; c.AddXp(xpToGive); }
        }

        Death();
    }

    virtual public void TakeDamage(float nb)
    {
        AddHp(-((nb - defense <= 0) ? 1 : (nb - defense)));

        if (hp <= 0)
        {
            Death();
        }
    }

    virtual protected void Death() 
    {
        if(hp <=0)
        {
            Destroy(this);
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
        return ressource.CompareGold(GoldCost) && ressource.CompareWood(WoodCost);
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
}
