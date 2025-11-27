using UnityEngine;

[RequireComponent(typeof(NavMeshController))]
public class TroupeManager : AggressifEntityManager
{
    [Header("Attribute")]
    public float StartSpeed = 2;
    public float level;
    [SerializeField] private float xp;
    [SerializeField] private int HpToAdd = 1;
    [SerializeField] private int AttackToAdd = 1;
    [SerializeField] private int DefenseToAdd = 1;

    private float speed = 2;
    private static readonly int WalkSpeed = Animator.StringToHash("WalkSpeed");
    private NavMeshController _navMeshAgent;

    protected override void Awake()
    {
        base.Awake();
        _navMeshAgent = GetComponent<NavMeshController>();

        _animator.SetFloat(WalkSpeed, speed);
        SetSpeed(StartSpeed);
    }
    public float Speed
    {
        get => speed;
    }
    public void SetSpeed(float nb)
    {
        speed = nb;
        SetNavMeshSpeed(nb);
        if(_animator != null) { _animator.SetFloat(WalkSpeed, speed); }
        
        
    }
    public void SetSpeedWithoutAnimation(float nb)
    {
        speed = nb;
        SetNavMeshSpeed(nb);

    }
    public void AddSpeed(float nb)
    {
        speed += nb;
        SetNavMeshSpeed(nb);
        if (_animator != null){ _animator.SetFloat(WalkSpeed, speed);}
    }

    private void SetNavMeshSpeed(float speed)
    {
        _navMeshAgent.SetSpeedWithNavMesh(speed);
    }

    public void AddXp(float xpGive)
    {
        xp += xpGive;
        if (xp >= 100)
        {
            xp -= 100;
            AddLevel();
        }
    }

    public void AddLevel()
    {
        level += 1;
        defense += DefenseToAdd;
        MaxHp += HpToAdd;
        Attack += AttackToAdd;
    }
}
