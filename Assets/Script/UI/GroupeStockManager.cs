using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GroupeStockManager : MonoBehaviour
{
    private List<EntityController> _listOfEntityManager;
    [SerializeField] private InputActionReference multiSelectionInput;

    private bool AddMore;

    private int _nbOfEntity;

    void Start()
    {
        multiSelectionInput.action.performed += SetAddMore;
        multiSelectionInput.action.canceled += SetAddMore;
        _listOfEntityManager = new List<EntityController>();
    }

    private void OnDestroy()
    {
        multiSelectionInput.action.performed -= SetAddMore;
        multiSelectionInput.action.canceled -= SetAddMore;
        if (FindObjectOfType<GroupeStockUi>())
        {
            FindObjectOfType<GroupeStockUi>().RemoveCadre(gameObject);
        }

    }
    private void SetAddMore(InputAction.CallbackContext context)
    {
        AddMore = !AddMore;
    }
    private void AddList(List<EntityController> listOfEntityManager)
    {
        _listOfEntityManager = new List<EntityController>(listOfEntityManager);
        _nbOfEntity = _listOfEntityManager.Count;
        ActualiseAffichage();
        foreach (EntityController entityManager in _listOfEntityManager)
        {
            entityManager.gameObject.GetComponent<SelectableManager>().deathEvent.AddListener(RemoveEntity);
        }
        FindObjectOfType<GroupeStockUi>().AddEntity();
    }

    private void AddToList(List<EntityController> listOfEntityManager)
    {
        foreach (EntityController entityManager in listOfEntityManager)
        {
            if (!_listOfEntityManager.Contains(entityManager))
            {
                _listOfEntityManager.Add(entityManager);
                entityManager.gameObject.GetComponent<SelectableManager>().deathEvent.AddListener(RemoveEntity);
                _nbOfEntity += 1;
            }
        }
        ActualiseAffichage();
    }


    public virtual void OnPointerClick(BaseEventData data)
    {

        PointerEventData eventData = data as PointerEventData;

        SelectManager selectManager = FindObjectOfType<SelectManager>();

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            List<EntityController> list = selectManager.getSelectList();
            if (list.Count > 0)
            {
                if (AddMore) { AddToList(list); }
                else
                {
                    ResetList();
                    AddList(list);
                }
            }

        }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UiGestioneur uiGestioneur = FindObjectOfType<UiGestioneur>();
            selectManager.ClearList();

            uiGestioneur.ActualiseUi(_listOfEntityManager[0].gameObject.GetComponent<AggressifEntityManager>());
            foreach (EntityController entityController in _listOfEntityManager)
            {
                AggressifEntityManager entityManager = entityController.gameObject.GetComponent<AggressifEntityManager>();
                selectManager.AddSelect(entityManager);

                uiGestioneur.AddOnGroupUi(entityManager);
            }
        }

    }

    public List<EntityController> GetList()
    {
        return _listOfEntityManager;
    }

    public void ResetList() { _listOfEntityManager.Clear(); }

    private void RemoveEntity(SelectableManager entityManager)
    {
        _listOfEntityManager.Remove(entityManager.gameObject.GetComponent<EntityController>());
        entityManager.deathEvent.RemoveListener(RemoveEntity);
        _nbOfEntity -= 1;
        if (_nbOfEntity == 0)
        {
            Destroy(gameObject); return;
        }
        ActualiseAffichage();
    }

    private void ActualiseAffichage()
    {
        GetComponentInChildren<TMP_Text>().text = _nbOfEntity.ToString();
        GetComponentInChildren<Image>().sprite = _listOfEntityManager[0].gameObject.GetComponent<AggressifEntityManager>().GetSprit();

    }


}
