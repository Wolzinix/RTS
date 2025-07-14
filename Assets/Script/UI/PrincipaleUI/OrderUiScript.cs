using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OrderUiScript : MonoBehaviour
{
    GameObject _entity;

    [SerializeField] Button _buttonForBuilding;
    [SerializeField] BuildUi _buildUi;

    [SerializeField] List<ButtonForCapacity> _ListOfAbilityButton;
    [SerializeField] private InputActionReference RightClick;
    private List<CapacityController> listOfCapacity = new();

    ControlManager controlManager;

    private void Start()
    {
        RightClick.action.started += DoRightClick;
        controlManager = FindAnyObjectByType<ControlManager>();
    }
    private void RemoveListenerFromCapacity()
    {
        foreach(CapacityController capacity in listOfCapacity)
        {
            capacity.ActivateEvent.RemoveListener(ActualiseACapacity);
            if (capacity.GetType().IsSubclassOf(typeof(ActivableCapacity)))
            {
                ActivableCapacity activable = (ActivableCapacity)capacity;
                activable.changeActif.RemoveListener(_ListOfAbilityButton[listOfCapacity.IndexOf(capacity)].Clignote);
            }
        }
    }
    public void SetEntity(GameObject entity)
    {
        _entity = entity;
        RemoveListenerFromCapacity();
        listOfCapacity = _entity.GetComponentsInChildren<CapacityController>().ToList();
        ActualiseUi();
    }

    private void ActualiseUi()
    {
        _buttonForBuilding.gameObject.SetActive(_entity.GetComponent<BuilderController>()); 

        if(_entity.GetComponent<TroupeManager>())
        {
            StopAllCoroutines();
            foreach (ButtonForCapacity button in _ListOfAbilityButton)
            {
                button.gameObject.SetActive(true);
                button.onClick.RemoveAllListeners();
                if (_ListOfAbilityButton.IndexOf(button) < listOfCapacity.Count)
                {
                    CapacityController capacity = listOfCapacity[_ListOfAbilityButton.IndexOf(button)];

                    button.SetCapacity(capacity);
                    button.isActivable = false;

                    if (capacity.GetType() != typeof(PassifCapacity))
                    {
                        button.onClick.AddListener(delegate { controlManager.CapacityOrder(capacity); });
                        StartCoroutine(ChargeBarOfAbility(button.GetComponentsInChildren<Image>()[1], capacity));
                        capacity.ActivateEvent.AddListener(ActualiseACapacity);
                    }
                    if (capacity.GetType().IsSubclassOf(typeof(ActivableCapacity)))
                    {
                        ActivableCapacity activable = (ActivableCapacity)capacity;
                        activable.changeActif.AddListener(button.Clignote);
                        button.Clignote(activable);
                    }
                }
                else { button.gameObject.SetActive(false); }
            }
        }
        else
        {
            foreach (ButtonForCapacity button in _ListOfAbilityButton)
            {
                button.onClick.RemoveAllListeners();
                button.gameObject.SetActive(false);
            }
        }
    }

    private void ActualiseACapacity(CapacityController capacity)
    {
        ButtonForCapacity button = _ListOfAbilityButton[listOfCapacity.IndexOf(capacity)];
        StartCoroutine(ChargeBarOfAbility(button.GetComponentsInChildren<Image>()[1], capacity));
    }

    public void GoToBuildUi()
    {
        _buildUi.gameObject.SetActive(true);
        _buildUi.SetBuilder(_entity.GetComponent<BuilderController>());
        gameObject.SetActive(false);
    }

    private List<RaycastResult> DoUiRayCast()
    {
        PointerEventData eventData = new (EventSystem.current);
        List<RaycastResult> results = new ();
        eventData.position = Input.mousePosition;
        EventSystem.current.RaycastAll(eventData, results);
        return results;
    }

    private void DoRightClick(InputAction.CallbackContext obj)
    {
        List<RaycastResult> listOfUIRay = DoUiRayCast();
        foreach (RaycastResult raycastResult in listOfUIRay)
        {
            ButtonForCapacity _button = raycastResult.gameObject.GetComponent<ButtonForCapacity>();
            
            if (_button &&
                _ListOfAbilityButton.Contains(_button) &&
                (_button.GetCapacity().GetType() == typeof(ActivableCapacity) ||
                _button.GetCapacity().GetType().IsSubclassOf(typeof(ActivableCapacity)))
                )
            {
                List<CapacityController> listOfCapacaity = _entity.GetComponentsInChildren<CapacityController>().ToList();
                
                CapacityController capacity = listOfCapacaity[_ListOfAbilityButton.IndexOf(_button)];
                ActivableCapacity activable = (ActivableCapacity)capacity;
                controlManager.ChangeCapacityActif(activable);
                break;
            }
        }
    }
    IEnumerator ChargeBarOfAbility(Image Imagebutton, CapacityController capacity)
    {
        float progressionValue =  capacity.actualTime / capacity.GetCooldown();
        yield return new WaitForFixedUpdate();

        while (progressionValue < 1 && capacity.actualTime != 0)
        {
            Imagebutton.fillAmount = progressionValue;
            progressionValue = capacity.actualTime / capacity.GetCooldown();
            yield return new WaitForEndOfFrame();
        }

        Imagebutton.fillAmount = 1;
        yield return null;
    }
}
