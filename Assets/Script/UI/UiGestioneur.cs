using UnityEngine;
using UnityEngine.UI;

public class UiGestioneur : MonoBehaviour
{

    [SerializeField] private EntityUiManager entityUi;
    [SerializeField] private GroupeUiManager groupUi;
    [SerializeField] private OrderUiScript orderUi;

    [SerializeField] private BuildingUiManager buildingUi;
    [SerializeField] private BuildUi buildUI;

    [SerializeField] private GameObject NoUi;

    private bool _multiSelectionIsActive;
    public void SetMulitSelection(bool multi) {  _multiSelectionIsActive = multi; }

    private void ActualiseEntityUI(SelectableManager entity)
    {
        if (!_multiSelectionIsActive)
        {
            if (groupUi.gameObject.activeSelf) { groupUi.gameObject.SetActive(false); }

            if (!entityUi.gameObject.activeSelf)
            {
                groupUi.gameObject.SetActive(false);
                entityUi.gameObject.SetActive(true);
                entityUi.SetEntity(entity);
            }
            if (entityUi.gameObject.activeSelf) { entityUi.SetEntity(entity); }
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

    private void ActualiseColorUi(SelectableManager entity)
    {
        if (entity.gameObject.GetComponent<SelectableManager>())
        {
            if (entity.CompareTag("Allie")) { entityUi.GetComponentInChildren<Image>().color = Color.green;  }
            else if (entity.CompareTag("ennemie")) { entityUi.GetComponentInChildren<Image>().color = Color.red; }
            else {  entityUi.GetComponentInChildren<Image>().color = Color.grey; }
        }
        else if (entity.gameObject.GetComponent<BuildingController>())
        {
            if (entity.CompareTag("Allie")) { entityUi.GetComponentInChildren<Image>().color = Color.blue;  }
            else if (entity.CompareTag("ennemie")) { entityUi.GetComponentInChildren<Image>().color = Color.black;  }
            else { entityUi.GetComponentInChildren<Image>().color = Color.grey; }
        }
    }

    public void ActualiseUi(SelectableManager entity)
    {
        NoUi.gameObject.SetActive(false);
        ActualiseEntityUI(entity);

        if (entity.gameObject.GetComponent<AggressifEntityManager>())
        {
            if ( entity.CompareTag("Allie"))
            {
                orderUi.gameObject.SetActive(true);
                orderUi.SetEntity(entity.gameObject);
            }
    
            buildingUi.gameObject.SetActive(false);
            buildUI.gameObject.SetActive(false);
        }

        else if (entity.gameObject.GetComponent<BuildingController>())
        {
            if (!entity.CompareTag("ennemie"))
            {
                buildingUi.gameObject.SetActive(true);
                buildingUi.SetBuilding(entity.gameObject.GetComponent<BuildingController>());
            }
            buildUI.gameObject.SetActive(false);
            orderUi.gameObject.SetActive(false);
        }

        ActualiseColorUi(entity);
    }

    public void AddOnGroupUi(SelectableManager entity)
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
