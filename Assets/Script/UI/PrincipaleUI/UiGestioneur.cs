using Assets.Script.Game;
using UnityEngine;
using UnityEngine.UI;

public class UiGestioneur : MonoBehaviour
{
    [Header("EntityUI")]
    [SerializeField] public GroupeUiManager groupUi;
    [SerializeField] private EntityUiManager entityUi;
    [SerializeField] private OrderUiScript orderUi;

    [Header("BuildUI")]
    [SerializeField] private BuildingUiManager buildingUi;
    [SerializeField] private BuildUi buildUI;

    [Header("OtherUI")]
    [SerializeField] private GameObject NoUi;

    private bool _multiSelectionIsActive;

    public void DesactiveUi()
    {
        entityUi.gameObject.SetActive(false);
        groupUi.gameObject.SetActive(false);
        orderUi.gameObject.SetActive(false);
        NoUi.SetActive(true);
        buildingUi.gameObject.SetActive(false);
        buildUI.gameObject.SetActive(false);
    }
    public void SetMulitSelection(bool multi) { _multiSelectionIsActive = multi; }

    private void SetGroupUI(SelectableManager entity)
    {
        groupUi.gameObject.SetActive(true);
        entityUi.gameObject.SetActive(true);

        if (!_multiSelectionIsActive || groupUi._listOfEntity.Count <= 1)
        {
            entityUi.SetEntity(entity);
            groupUi.gameObject.SetActive(false);
        }
        else
        {
            groupUi.AddEntity(entityUi.GetEntity());
            groupUi.AddEntity(entity);
            entityUi.gameObject.SetActive(false);
        }
    }

    private void ActualiseColorUi(SelectableManager entity)
    {
        if (entity.entityType == EntityType.Building)
        {
            if (entity.CompareTag("Allie")) { entityUi.backgroundImage.color = Color.blue; }
            else if (entity.CompareTag("ennemie")) { entityUi.backgroundImage.color = Color.black; }
            else { entityUi.backgroundImage.color = Color.gray; }
        }
        else if(entity)
        {
            if (entity.CompareTag("Allie")) { entityUi.backgroundImage.color = Color.green; }
            else if (entity.CompareTag("ennemie")) { entityUi.backgroundImage.color = Color.red; }
            else { entityUi.backgroundImage.color = Color.grey; }
        }
    }

    public void ActualiseUi(SelectableManager entity)
    {
        NoUi.SetActive(false);
        SetGroupUI(entity);

        orderUi.gameObject.SetActive(false);
        buildingUi.gameObject.SetActive(false);
        buildUI.gameObject.SetActive(false);

        if (entity.IsAggressifEntity())
        {
            if (entity.CompareTag("Allie"))
            {
                orderUi.gameObject.SetActive(true);
                orderUi.SetEntity(entity.gameObject);
            }
        }

        else if (entity.entityType == EntityType.Building)
        {
            if (!entity.CompareTag("ennemie"))
            {
                buildingUi.gameObject.SetActive(true);
                buildingUi.SetBuilding(entity.gameObject.GetComponent<ProductBuildingController>());
            }
        }

        ActualiseColorUi(entity);
    }

    public void AddOnGroupUI(SelectableManager entity)
    {
        if (entity.entityType != EntityType.Building)
        {
            groupUi.gameObject.SetActive(true);
            orderUi.gameObject.SetActive(true);
            groupUi.AddEntity(entity);
            orderUi.SetEntity(entity.gameObject);
        }
        else
        {
            DesactiveUi();
            ActualiseUi(entity);
        }
    }

    public void RemoveOnGroupUI(SelectableManager entity)
    {
        if (entity.entityType != EntityType.Building)
        {
            groupUi.gameObject.SetActive(true);
            orderUi.gameObject.SetActive(true);
            groupUi.RemoveEntity(entity);
        }
    }
}
