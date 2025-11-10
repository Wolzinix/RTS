using UnityEngine;

public class AttackState : StateClassEntity
{
    private EntityController _controller;
    private SelectableManager _target;
    private ProjectilManager _projectile;
    private AggressifEntityManager _controllerAggressifManager;
    private bool _attackCheckOnce = false;

    public AttackState(EntityController controller, ProjectilManager projectile, SelectableManager target)
    {
        _controller = controller;
        _projectile = projectile;
        _target = target;
        _controllerAggressifManager = controller.GetComponent<AggressifEntityManager>();
    }
    public AttackState(EntityController controller, SelectableManager target)
    {
        _controller = controller;
        _target = target;
        _controllerAggressifManager = controller.GetComponent<AggressifEntityManager>();
    }

    public override void Start() { PrepareAttack(); }

    private void PrepareAttack()
    {
        _controller.PlayAnimation((int)AnimationController.AnimType.Attack);
    }

    void DoAttack()
    {
        if (_projectile)
        {
            ProjectilManager pj = EntityController.Instantiate(_projectile);
            pj.SetTarget(_target.gameObject);
            pj.SetInvoker(_controllerAggressifManager);

            Vector3 spawnPosition = new (_controller.transform.position.x,_controller.transform.position.y + 1 , _controller.transform.position.z);
            if(_controller.pointOfSpawn) { spawnPosition = _controller.pointOfSpawn.position; }

            pj.gameObject.transform.position = spawnPosition;

            if(_controllerAggressifManager.effect)
            {
                _projectile._effect = _controllerAggressifManager.effect;
            }
        }

        else 
        {
            _controllerAggressifManager.DoAttack(_target);
        }
        _controllerAggressifManager.DoAnAttack.Invoke();
    }

    public override void Update()
    {
        if (_target)
        {
            if (Vector3.Distance(_controller.gameObject.transform.position, _target.transform.position) <= _controllerAggressifManager.Range + _target.size)
            {
                float AnimatorStateInfo = _controller.GetAnimationInfo();

                if (AnimatorStateInfo > 0.5 &&
                    !_attackCheckOnce)
                {
                    DoAttack();
                    _attackCheckOnce = true;
                }

                if ( AnimatorStateInfo > 1)
                {
                    _attackCheckOnce = false;
                    PrepareAttack();
                }

                else
                {
                    _controller.gameObject.transform.LookAt(new Vector3(_target.transform.position.x, _controller.transform.localPosition.y, _target.transform.position.z));
                    _controller.gameObject.transform.rotation = new Quaternion(0, _controller.gameObject.transform.rotation.y, 0, _controller.gameObject.transform.rotation.w);
                }
            }
            else { End(); }
        }
        else 
        { 
            if(_controller._EnnemieList.Count <= 0)
            {
                FogWarManager fogManager = _controller.GetComponent<FogWarManager>();
                if (fogManager) { fogManager.ActualiseFog(_controller, true); }
            }
            End(); 
        }
    }
    public override void End()
    {
        _controller.RemoveFirstOrder();
    }
}
