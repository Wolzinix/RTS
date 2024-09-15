using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GroupStockManager : MonoBehaviour
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
        if(FindObjectOfType<GroupeStockUi>())
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
            entityManager.gameObject.GetComponent<EntityManager>().deathEvent.AddListener(RemoveEntity);
        }
        FindObjectOfType<GroupeStockUi>().AddEntity();
    }

    private void AddToList(List<EntityController> listOfEntityManager)
    {
        foreach(EntityController entityManager in listOfEntityManager)
        {
            _listOfEntityManager.Add(entityManager);
            entityManager.gameObject.GetComponent<EntityManager>().deathEvent.AddListener(RemoveEntity);
            _nbOfEntity += 1;
        }
        ActualiseAffichage();
    }


    public virtual void OnPointerClick(BaseEventData data)
    {
        
        PointerEventData eventData = data as PointerEventData;

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            List<EntityController> list = FindObjectOfType<SelectManager>().getSelectList();
            if(list.Count > 0)
            {
                if (AddMore){AddToList(list); }
                else {AddList(list);}
            }
           
        }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            foreach ( EntityController entityController in _listOfEntityManager)
            {
                EntityManager entityManager = entityController.gameObject.GetComponent<EntityManager>();
                FindObjectOfType<SelectManager>().AddSelect(entityManager);
                FindObjectOfType<UiGestioneur>().SetMulitSelection(true);
                FindObjectOfType<UiGestioneur>().ActualiseUi(entityManager);
            }
            FindObjectOfType<UiGestioneur>().SetMulitSelection(false);
        }
        
    }

    public List<EntityController> GetList()
    {
        return _listOfEntityManager;
    }

    public void ResetList (){ _listOfEntityManager.Clear(); }

    private void RemoveEntity(EntityManager entityManager)
    {
        _listOfEntityManager.Remove(entityManager.gameObject.GetComponent<EntityController>());
        entityManager.deathEvent.RemoveListener(RemoveEntity);
        _nbOfEntity -= 1;
        if(_nbOfEntity == 0)
        {
            Destroy(this); return;
        }
        ActualiseAffichage();
    }

    private  void ActualiseAffichage()
    {
        GetComponentInChildren<TMP_Text>().text = _nbOfEntity.ToString();
        GetComponentInChildren<Image>().sprite = _listOfEntityManager[0].gameObject.GetComponent<EntityManager>().GetSprit();

    }


}
