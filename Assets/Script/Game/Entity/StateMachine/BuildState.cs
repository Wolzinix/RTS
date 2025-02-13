using UnityEngine;
public class BuildState : StateClassEntity
{
    BuilderController builder;
    Vector3 position;
    SelectableManager defenseManager;
    float size;

    public BuildState(BuilderController builder, Vector3 position, SelectableManager defenseManager)
    {
        this.builder = builder;
        this.position = position;
        this.defenseManager = defenseManager;
        size = defenseManager.GetComponentInChildren<Renderer>().bounds.size.x + defenseManager.GetComponentInChildren<Renderer>().bounds.size.y;
    }
    public override void Start() { }

    public override void Update()
    {
        float a = Vector3.Distance(builder.transform.position, position);
        if (Vector3.Distance(builder.transform.position, position)
            <= size + builder.GetComponent<NavMeshController>().HaveStoppingDistance()
           )
        {
            if (defenseManager.GetComponent<EntityManager>().CanDoIt(builder.GetComponent<AggressifEntityManager>().ressources))
            {
                Collider[] colliders = builder.DoAOverlap(position);

                if (colliders.Length == 0 || colliders.Length == 1 && colliders[0].gameObject.GetComponent<EntityManager>() == null || (colliders.Length == 2 && (colliders[1] == builder || colliders[0] == builder)))
                {
                    SelectableManager gm = BuilderController.Instantiate(defenseManager, new Vector3(position.x, builder.transform.position.y, position.z), builder.transform.rotation, builder.transform.parent).GetComponent<SelectableManager>();
                    builder.GetComponent<AggressifEntityManager>().ressources.AddGold(-defenseManager.GetComponent<EntityManager>().GoldAmount);
                    builder.GetComponent<AggressifEntityManager>().ressources.AddWood(-defenseManager.GetComponent<EntityManager>().WoodAmount);
                    gm.gameObject.tag = builder.tag;
                    if(gm.GetComponent<DefenseManager>())
                    {
                        builder.TowerIsBuild.Invoke(builder, gm.GetComponent<DefenseManager>());
                    }
                    gm.ActualiseSprite();

                    builder.PayCostOfBuilding(defenseManager);
                    End();
                }
                else
                {
                    foreach (Collider i in colliders)
                    {
                        if (i.GetComponent<EntityController>() != null)
                        {
                            if (i.GetComponent<DefenseManager>() != null) { End(); }
                            else
                            {
                                Vector3 iPosition = i.GetComponent<EntityController>().transform.localPosition;
                                i.GetComponent<EntityController>().AddPath((iPosition - (iPosition - position) - defenseManager.GetComponentInChildren<Renderer>().bounds.size * 2));
                            }
                        }
                    }
                }
            }
            else
            {
                End();
            }
        }
        else
        {
            builder.AddPathWithRange(position, size);
        }
    }

    public override void End()
    {
        builder.RemoveFirstOrder();
    }
}