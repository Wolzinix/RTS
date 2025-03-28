using UnityEngine;

public class ProjectilManager : MonoBehaviour
{
    private GameObject _target;
    private AggressifEntityManager _invoker;

    [SerializeField] private Sprite _sprite;

    [SerializeField] private float _damage = 0;
    public bool fixeDamage;

    private Rigidbody _rb;

    public StateEffect _effect;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (!_target) { Destroy(gameObject); return; }
        transform.LookAt(_target.gameObject.transform);


        _rb.AddForce(new Vector3(
            _target.transform.position.x - transform.position.x,
            _target.transform.position.y - transform.position.y,
            _target.transform.position.z - transform.position.z), ForceMode.Impulse);
    }

    public void SetDamage(float damage) { _damage = damage; }

    public void SetTarget(GameObject target)
    {
        _target = target;

        transform.LookAt(_target.gameObject.transform);
    }
    public void SetInvoker(AggressifEntityManager invoker) 
    { 
        _invoker = invoker; 
        if (!fixeDamage) { SetDamage(invoker.Attack); } 
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject == _target)
        {
            _target.GetComponent<SelectableManager>().TakeDamage(_invoker, _damage);
            if (_invoker) 
            {
                _target.GetComponent<SelectableManager>().TakingDamageFromEntity.Invoke(_invoker.GetComponent<AggressifEntityManager>());
                if(_effect)
                {
                    _effect.AddEffectToTarget(_target.GetComponent<SelectableManager>());
                }
            }

            Destroy(gameObject);
        }
    }
}
