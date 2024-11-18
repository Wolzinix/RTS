﻿using UnityEngine;
public class BuildState : StateClassEntity
{
    BuilderController builder;
    Vector3 position;
    DefenseManager defenseManager;

    public BuildState(BuilderController builder, Vector3 position, DefenseManager defenseManager)
    {
        this.builder = builder;
        this.position = position;
        this.defenseManager = defenseManager;
    }
    public override void Start() { }

    public override void Update()
    {
        if (Vector3.Distance(builder.transform.position, position)
            <=
            builder.GetComponent<NavMeshController>().HaveStoppingDistance() + defenseManager.GetComponentInChildren<Renderer>().bounds.size.x + defenseManager.GetComponentInChildren<Renderer>().bounds.size.y)
        {
            if (defenseManager.GetComponent<EntityManager>().CanDoIt(builder.GetComponent<AggressifEntityManager>().ressources))
            {
                Collider[] colliders = builder.DoAOverlap(position);

                if (colliders.Length == 0 || colliders.Length == 1 && colliders[0].gameObject.GetComponent<EntityManager>() == null || (colliders.Length == 2 && (colliders[1] == builder || colliders[0] == builder)))
                {
                    DefenseManager gm = BuilderController.Instantiate(defenseManager, new Vector3(position.x, builder.transform.position.y, position.z), builder.transform.rotation, builder.transform.parent).GetComponent<DefenseManager>();
                    builder.GetComponent<AggressifEntityManager>().ressources.AddGold(-defenseManager.GetComponent<EntityManager>().GoldAmount);
                    builder.GetComponent<AggressifEntityManager>().ressources.AddWood(-defenseManager.GetComponent<EntityManager>().WoodAmount);
                    gm.gameObject.tag = builder.tag;
                    builder.TowerIsBuild.Invoke(builder, gm);
                    gm.ActualiseSprite();

                    builder.PayCostOfBuilding(defenseManager);
                    end();
                }
                else
                {
                    foreach (Collider i in colliders)
                    {
                        if (i.GetComponent<EntityController>() != null)
                        {
                            if (i.GetComponent<DefenseManager>() != null) { end(); }
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
                end();
            }
        }
        else
        {
            builder.AddPathInFirst(position);
        }
    }

    public override void end()
    {
        builder.RemoveFirstOrder();
    }
}