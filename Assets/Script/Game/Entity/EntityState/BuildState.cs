using Assets.Script.Tools;
using UnityEngine;
public class BuildState : StateClassEntity
{
    BuilderController _builder;
    Vector3 _position;
    SelectableManager _defenseManager;
    float _size;
    RessourceController _ressources;

    public BuildState(BuilderController builder, Vector3 position, SelectableManager defenseManager)
    {
        _builder = builder;
        _position = position;
        _defenseManager = defenseManager;
        _size = defenseManager.GetComponentInChildren<Renderer>().bounds.size.x + defenseManager.GetComponentInChildren<Renderer>().bounds.size.y;
        _ressources = _builder.GetComponent<AggressifEntityManager>().ressources;
    }
    public override void Start() { }

    public override void Update()
    {
        if (Vector3.Distance(_builder.transform.position, _position)
            <= _size + _builder.GetComponent<NavMeshController>().HaveStoppingDistance() + 0.2
           )
        {
            if (_defenseManager.GetComponent<EntityManager>().CanDoIt(_ressources))
            {
                Collider[] colliders = _builder.DoAOverlap(_position);

                if (colliders.Length == 0 || colliders.Length == 1 && colliders[0].gameObject.GetComponent<EntityManager>() == null || (colliders.Length == 2 && (colliders[1] == _builder.gameObject || colliders[0] == _builder.gameObject)))
                {

                    SelectableManager gm = BuilderController.Instantiate(_defenseManager, RayCast.RaycastForGround(new Vector3(_position.x,_builder.transform.position.y,_position.z)), _builder.transform.rotation, _builder.transform.parent).GetComponent<SelectableManager>();
                    
                    gm.gameObject.tag = _builder.tag;
                    DefenseManager defenseManager = gm.GetComponent<DefenseManager>();
                    if (defenseManager)
                    {
                        _builder.TowerIsBuild.Invoke(_builder, defenseManager);
                    }
                    gm.ActualiseSprite();

                    _builder.PayCostOfBuilding(gm);
                    End();
                }
                else
                {
                    foreach (Collider i in colliders)
                    {
                        if (i.GetComponent<EntityController>() && i.GetComponent<DefenseManager>() == null)
                        {
                            Vector3 iPosition = i.GetComponent<EntityController>().transform.position;
                            i.GetComponent<EntityController>().AddPath((iPosition - (iPosition - _position) - _defenseManager.GetComponentInChildren<Renderer>().bounds.size ));
                        }
                        else{ End(); }
                    }
                }
            }
            else  { End(); }
        }
        else  { _builder.AddPathWithRange(_position, _size); }
    }

    public override void End()
    {
        _builder.RemoveFirstOrder();
    }

    
}