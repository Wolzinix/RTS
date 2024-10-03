using UnityEngine;

public class ProjectilManager : MonoBehaviour
{
    private GameObject _target;
    private TroupeManager _invoker;

    [SerializeField] private Sprite _sprite;

    private float _damage;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (!_target) { Destroy(gameObject); return; }
        transform.LookAt(_target.gameObject.transform);


        _rb.AddForce( new Vector3(
            _target.transform.position.x - transform.position.x, 
            _target.transform.position.y - transform.position.y, 
            _target.transform.position.z - transform.position.z),ForceMode.Impulse);
    }

    public void SetDamage(float damage){_damage = damage;}

    public void SetTarget(GameObject target)
    {
        _target = target;

        transform.LookAt(_target.gameObject.transform);
    }
    public void SetInvoker(TroupeManager invoker) { _invoker = invoker; }


    private void OnTriggerEnter(Collider other)
    {
        if(other != null && other.gameObject == _target)
        { 
            _target.GetComponent<TroupeManager>().TakeDamage(_invoker,_damage);
            if( _invoker ) {_target.GetComponent<TroupeManager>().TakingDamageFromEntity.Invoke(_invoker.GetComponent<TroupeManager>());}
            
            Destroy(gameObject);
        }
    }
}
