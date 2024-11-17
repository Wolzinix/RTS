using UnityEngine;

public class TroupeManager : AggressifEntityManager
{

    [SerializeField] public float StartSpeed = 2;
    [SerializeField]  private float speed = 2;
    private static readonly int WalkSpeed = Animator.StringToHash("WalkSpeed");

    private NavMeshController _navMeshAgent;

    [SerializeField] private float xp;
    [SerializeField] private float level;

    [SerializeField] private int HpToAdd = 1;
    [SerializeField] private int AttackToAdd = 1;
    [SerializeField] private int DefenseToAdd = 1;

    protected override void Awake()
    {
        base.Awake();

        if (GetComponent<NavMeshController>())
        {
            _navMeshAgent = GetComponent<NavMeshController>();
        }

        if (GetComponentInChildren<Animator>())
        {
            _animator.SetFloat(WalkSpeed, speed);
        }

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
        if (_animator)
        {
            _animator.SetFloat(WalkSpeed, speed);
        }
    }

 
    public void AddSpeed(float nb)
    {
        speed += nb;
        SetNavMeshSpeed(nb);
        if (_animator)
        {
            _animator.SetFloat(WalkSpeed, speed);
        }
    }

    private void SetNavMeshSpeed(float speed)
    {
        if (_navMeshAgent) {  _navMeshAgent.SetSpeedWithNavMesh(speed); }
    }

    public void AddXp(float xpGive)
    {
        xp += xpGive;
        if(xp >= 100)
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
