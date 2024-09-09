using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiGestioneur : MonoBehaviour
{

    [SerializeField] private EntityUiManager entityUi;

    [SerializeField] private GroupUiManager groupUi;


    [SerializeField] private GameObject orderUi;


    private bool _multiSelectionIsActive;


    public void SetMulitSelection(bool multi) {  _multiSelectionIsActive = multi; }

    public void ActualiseUi(EntityManager entity)
    {
        orderUi.gameObject.SetActive(true);
        if (!_multiSelectionIsActive)
        {
            if (groupUi.gameObject.activeSelf) { groupUi.gameObject.SetActive(false); }

            if (!entityUi.gameObject.activeSelf)
            {
                groupUi.gameObject.SetActive(false);

                entityUi.gameObject.SetActive(true);
                entityUi.SetEntity(entity);
            }
            if (entityUi.gameObject.activeSelf)
            {
                entityUi.SetEntity(entity);
            }
        }
        else
        {

            if (entityUi.gameObject.activeSelf)
            {
                groupUi.gameObject.SetActive(true);
                groupUi.AddEntity(entityUi.GetEntity());
                groupUi.AddEntity(entity);

                entityUi.gameObject.SetActive(false);
            }
            else
            {
                if (groupUi.gameObject.activeSelf) { groupUi.AddEntity(entity); }
                else
                {
                    entityUi.gameObject.SetActive(true);
                    entityUi.SetEntity(entity);
                }
            }
        }
    }

    public void AddOnGroupUi(EntityManager entity)
    {
        groupUi.gameObject.SetActive(true);
        orderUi.gameObject.SetActive(true);
        groupUi.AddEntity(entity);
    }



    public void DesactiveUi()
    {
        entityUi.gameObject.SetActive(false);
        groupUi.gameObject.SetActive(false);
        orderUi.gameObject.SetActive(false);
    }
}
