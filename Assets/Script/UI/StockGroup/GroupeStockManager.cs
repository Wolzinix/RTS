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

    private GroupeStockUi _groupeStockUi;
    private UiGestioneur _uiGestioneur;
    private SelectManager _selectManager;

    void Start()
    {
        multiSelectionInput.action.performed += SetAddMore;
        multiSelectionInput.action.canceled += SetAddMore;

        _listOfEntityManager = new List<EntityController>();

        _groupeStockUi = FindObjectOfType<GroupeStockUi>();
        _uiGestioneur = FindObjectOfType<UiGestioneur>();
        _selectManager = FindObjectOfType<SelectManager>();
    }

    private void OnDestroy()
    {
        multiSelectionInput.action.performed -= SetAddMore;
        multiSelectionInput.action.canceled -= SetAddMore;
        if (_groupeStockUi)
        {
            _groupeStockUi.RemoveCadre(gameObject);
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
        _groupeStockUi.AddEntity();
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

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            List<EntityController> list = _selectManager.GetSelectList();
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
            _selectManager.ClearList();

            if (_listOfEntityManager.Count > 0)
            {
                _uiGestioneur.ActualiseUi(_listOfEntityManager[0].gameObject.GetComponent<AggressifEntityManager>());
                foreach (EntityController entityController in _listOfEntityManager)
                {
                    AggressifEntityManager entityManager = entityController.gameObject.GetComponent<AggressifEntityManager>();
                    _selectManager.AddSelect(entityManager);

                    _uiGestioneur.AddOnGroupUi(entityManager);
                }
            }
        }
    }

    public List<EntityController> GetList() { return _listOfEntityManager; }

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
