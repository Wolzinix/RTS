using UnityEngine;
using UnityEngine.Events;

public class EntityManager : MonoBehaviour
{

    [SerializeField] protected float hp = 10;
    protected float _maxHp;

    public int GoldAmount = 1;

    public int WoodAmount = 1;

    public int GoldCost = 1;

    public int WoodCost = 1;

    [HideInInspector] public float size;

    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Sprite spriteImage;

    [SerializeField] public Sprite Allisprite;
    [SerializeField] public Sprite Ennemisprite;
    [SerializeField] public Sprite Neutralprite;

    [SerializeField] protected float defense = 0;

    [HideInInspector] public UnityEvent changeStats = new UnityEvent();

    protected Animator _animator;
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
        size = getSize();

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
    virtual public void TakeDamage(AggressifEntityManager entity, float nb)
    {
        hp -= (nb - defense <= 0) ? 1 : (nb - defense);
    }

    virtual public void TakeDamage(float nb)
    {
        hp -= (nb - defense <= 0) ? 1 : (nb - defense);
    }

    virtual protected void Death() { }

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

    private float getSize()
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
