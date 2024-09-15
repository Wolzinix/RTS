using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilManager : MonoBehaviour
{
    private GameObject _target;

    [SerializeField] private Sprite _sprite;

    private float _damage;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        _rb.velocity = new Vector3(
            _target.transform.position.x, 
            _target.transform.position.y, 
            _target.transform.position.z);
    }

    public void SetDamage(float damage){_damage = damage;}

    public void SetTarget(GameObject target) { _target = target; }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision != null && collision.gameObject == _target)
        { 
            _target.GetComponent<EntityManager>().TakeDamage(_damage);
            Destroy(gameObject);
        }
    }
}
