using UnityEngine;
using UnityEngine.UI;

public class UiGestioneur : MonoBehaviour
{

    [SerializeField] private EntityUiManager entityUi;

    [SerializeField] private GroupeUiManager groupUi;


    [SerializeField] private OrderUiScript orderUi;

    [SerializeField] private BuildingUiManager buildingUi;


    [SerializeField] private GameObject NoUi;

    [SerializeField] private BuildUi buildUI;



    private bool _multiSelectionIsActive;


    public void SetMulitSelection(bool multi) {  _multiSelectionIsActive = multi; }

    public void ActualiseUi(EntityManager entity)
    {
        NoUi.gameObject.SetActive(false);
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

        if(entity.gameObject.GetComponent<EntityController>())
        {
            if ( entity.CompareTag("Allie"))
            {
                orderUi.gameObject.SetActive(true);
                orderUi.SetEntity(entity.gameObject);
                entityUi.GetComponentInChildren<Image>().color = Color.green;
            }
            else if (entity.CompareTag("ennemie"))
            {
                entityUi.GetComponentInChildren<Image>().color = Color.red;
            }
            else
            {
                entityUi.GetComponentInChildren<Image>().color = Color.grey;
            }
            buildingUi.gameObject.SetActive(false);
        }
        else if (entity.gameObject.GetComponent<BuildingController>())
        {
            if (entity.CompareTag("Allie"))
            {
                orderUi.gameObject.SetActive(true);
                orderUi.SetEntity(entity.gameObject);
                entityUi.GetComponentInChildren<Image>().color = Color.blue;
                buildingUi.gameObject.SetActive(true);
                buildingUi.SetBuilding(entity.gameObject.GetComponent<BuildingController>());
            }
            else if (entity.CompareTag("ennemie"))
            {
                entityUi.GetComponentInChildren<Image>().color = Color.black;
            }
            else
            {
                entityUi.GetComponentInChildren<Image>().color = Color.grey;
                buildingUi.gameObject.SetActive(true);
                buildingUi.SetBuilding(entity.gameObject.GetComponent<BuildingController>());
            }

            orderUi.gameObject.SetActive(false);
            
        }
    }

    public void AddOnGroupUi(EntityManager entity)
    {
        NoUi.gameObject.SetActive(false);
        groupUi.gameObject.SetActive(true);
        orderUi.gameObject.SetActive(true);
        orderUi.SetEntity(entity.gameObject);
        groupUi.AddEntity(entity);
    }



    public void DesactiveUi()
    {
        entityUi.gameObject.SetActive(false);
        groupUi.gameObject.SetActive(false);
        orderUi.gameObject.SetActive(false);
        NoUi.gameObject.SetActive(true);
        buildingUi.gameObject.SetActive(false);
        buildUI.gameObject.SetActive(false);
    }
}
