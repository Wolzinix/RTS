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

    [SerializeField] List<ButtonOverlap> _ListOfAbilityButton;
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
        if (_entity && _entity.GetComponent<BuilderController>()) { _buttonForBuilding.gameObject.SetActive(true); }
        else { _buttonForBuilding.gameObject.SetActive(false); }

        if(_entity.GetComponent<TroupeManager>())
        {
            StopAllCoroutines();
            foreach (ButtonOverlap button in _ListOfAbilityButton)
            {
                button.GetComponent<Image>().enabled = true;
                if (_ListOfAbilityButton.IndexOf(button) < listOfCapacity.Count)
                {
                    CapacityController capacity = listOfCapacity[_ListOfAbilityButton.IndexOf(button)];

                    button.gameObject.SetActive(true);
                    button.SetCapacity(capacity);

                    if (capacity.GetType().IsSubclassOf(typeof(PassifCapacity)))
                    {
                        button.GetComponent<ButtonOverlap>().enabled = false;
                    }
                    else
                    {
                        button.GetComponent<ButtonOverlap>().enabled = true;
                        button.onClick.RemoveAllListeners();
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
                    else
                    {
                        button.isActivable = false;
                    }
                }
                else
                {
                    button.onClick.RemoveAllListeners();
                    button.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            foreach (ButtonOverlap button in _ListOfAbilityButton)
            {
                button.onClick.RemoveAllListeners();
                button.gameObject.SetActive(false);
            }
        }
    }

    private void ActualiseACapacity(CapacityController capacity)
    {
        ButtonOverlap button = _ListOfAbilityButton[listOfCapacity.IndexOf(capacity)];
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
            if (raycastResult.gameObject.GetComponent<ButtonOverlap>())
            {
                List<CapacityController> listOfCapacaity = _entity.GetComponentsInChildren<CapacityController>().ToList();
                ButtonOverlap _button = raycastResult.gameObject.GetComponent<ButtonOverlap>();
                if(_ListOfAbilityButton.Contains(_button))
                {
                    CapacityController capacity = listOfCapacaity[_ListOfAbilityButton.IndexOf(_button)];
                    if (capacity.GetType().IsSubclassOf(typeof(ActivableCapacity)))
                    {
                        ActivableCapacity activable = (ActivableCapacity)capacity;

                        controlManager.ChangeCapacityActif(activable);
                    }
                }
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
