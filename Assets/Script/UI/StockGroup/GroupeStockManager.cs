using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GroupeStockManager : MonoBehaviour
{
    private List<EntityController> _listOfEntityController = new();
    [SerializeField] private InputActionReference multiSelectionInput;

    private bool AddMore;
    private int _nbOfEntity;

    private GroupeStockUI _groupeStockUI;
    private UiGestioneur _UIGestioneur;
    private SelectManager _selectManager;

    private TMP_Text _text;
    private Image _image;

    void Start()
    {
        multiSelectionInput.action.performed += SetAddMore;
        multiSelectionInput.action.canceled += SetAddMore;

        _groupeStockUI = FindObjectOfType<GroupeStockUI>();
        _UIGestioneur = FindObjectOfType<UiGestioneur>();
        _selectManager = FindObjectOfType<SelectManager>();
        _text = GetComponentInChildren<TMP_Text>();
        _image = GetComponentInChildren<Image>();
    }

    private void OnDestroy()
    {
        multiSelectionInput.action.performed -= SetAddMore;
        multiSelectionInput.action.canceled -= SetAddMore;
        if (_groupeStockUI)
        {
            _groupeStockUI.RemoveCadre(gameObject);
        }

    }
    private void SetAddMore(InputAction.CallbackContext context)
    {
        AddMore = !AddMore;
    }
    private void AddList(List<EntityController> listOfEntityManager)
    {
        _listOfEntityController = new List<EntityController>(listOfEntityManager);
        _nbOfEntity = _listOfEntityController.Count;
        ActualiseAffichage();
        foreach (EntityController entityManager in _listOfEntityController)
        {
            entityManager.GetComponent<SelectableManager>().deathEvent.AddListener(RemoveEntity);
        }
        _groupeStockUI.AddEntity();
    }

    private void AddToList(List<EntityController> listOfEntityManager)
    {
        foreach (EntityController entityManager in listOfEntityManager)
        {
            if (!_listOfEntityController.Contains(entityManager))
            {
                _listOfEntityController.Add(entityManager);
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
                return;
            }
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!AddMore) { _selectManager.ClearList(); }

            if (_listOfEntityController.Count > 0)
            {
                _UIGestioneur.ActualiseUi(_listOfEntityController[0].GetComponent<AggressifEntityManager>());
                foreach (EntityController entityController in _listOfEntityController)
                {
                    AggressifEntityManager entityManager = entityController.GetComponent<AggressifEntityManager>();
                    _UIGestioneur.AddOnGroupUI(entityManager);
                    if (!_selectManager.GetSelectList().Contains(entityController))
                    {
                        _selectManager.AddSelect(entityManager);
                    }
                }
            }
        }
    }

    public List<EntityController> GetList() { return _listOfEntityController; }

    public void ResetList() { _listOfEntityController.Clear(); }

    private void RemoveEntity(SelectableManager entityManager)
    {
        _listOfEntityController.Remove(entityManager.gameObject.GetComponent<EntityController>());
        entityManager.deathEvent.RemoveListener(RemoveEntity);
        _nbOfEntity -= 1;
        if (_nbOfEntity == 0){ Destroy(gameObject); return; }
        ActualiseAffichage();
    }

    private void ActualiseAffichage()
    {
        _text.text = _nbOfEntity.ToString();
        _image.sprite = _listOfEntityController[0].GetComponent<EntityManager>().GetSprit();
    }
}
