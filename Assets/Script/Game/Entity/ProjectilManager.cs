using UnityEngine;
public class ProjectilManager : MonoBehaviour
{
    public StateEffect _effect;
    public bool fixeDamage;

    [SerializeField] private Sprite _sprite;
    [SerializeField] private float _damage = 0;
    
    private Rigidbody _rb;
    private SelectableManager _target;
    private AggressifEntityManager _invoker;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (!_target) { Destroy(gameObject); return; }
        transform.LookAt(_target.transform);

        _rb.AddForce(
            new Vector3(
                _target.transform.position.x - transform.position.x,
                _target.transform.position.y - transform.position.y,
                _target.transform.position.z - transform.position.z), 
            ForceMode.Impulse);
    }

    public void SetDamage(float damage) { _damage = damage; }

    public void SetTarget(GameObject target)
    {
        _target = target.GetComponent<SelectableManager>();

        transform.LookAt(_target.transform);
    }
    public void SetInvoker(AggressifEntityManager invoker) 
    { 
        _invoker = invoker; 
        if (!fixeDamage) { SetDamage(invoker.Attack); } 
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other && _target && other.gameObject == _target.gameObject)
        {
            _target.TakeDamage(_invoker, _damage);
            if (_invoker) 
            {
                _target.TakingDamageFromEntity.Invoke(_invoker);
                if(_effect) { _effect.AddEffectToTarget(_target);}
            }
            Destroy(gameObject);
        }
    }
}
