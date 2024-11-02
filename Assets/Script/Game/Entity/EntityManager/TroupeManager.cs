using UnityEngine;
using UnityEngine.AI;

public class TroupeManager : AggressifEntityManager
{

    [SerializeField] public float StartSpeed = 2;
    [SerializeField]  private float speed = 2;


    private static readonly int WalkSpeed = Animator.StringToHash("WalkSpeed");


    private NavMeshAgent _navMeshAgent;



    protected override void Awake()
    {
        base.Awake();

        if (GetComponent<NavMeshAgent>())
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            SetNavMeshSpeed(speed);
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
        _navMeshAgent.enabled = true;
        if (_navMeshAgent)
        {
            _navMeshAgent.speed = speed;
        }

        _navMeshAgent.enabled = false;
    }


}
