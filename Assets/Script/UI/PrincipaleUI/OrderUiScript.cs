using Assets.Script.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OrderUiScript : MonoBehaviour
{
    [SerializeField] private Button _buttonForBuilding;
    [SerializeField] private BuildUi _buildUi;
    [SerializeField] private List<ButtonForCapacity> _ListOfAbilityButton;
    [SerializeField] private InputActionReference RightClick;

    private List<CapacityController> listOfCapacity = new();
    private GameObject _entity;
    private ControlManager controlManager;

    private void DoRightClick(InputAction.CallbackContext obj)
    {
        List<RaycastResult> listOfUIRay = RayCast.DoUiRayCastFromMouse();
        foreach (RaycastResult raycastResult in listOfUIRay)
        {
            ButtonForCapacity _button = raycastResult.gameObject.GetComponent<ButtonForCapacity>();

            if (_button &&
                _entity &&
                _ListOfAbilityButton.Contains(_button) &&
                (_button.GetCapacity().GetType() == typeof(ActivableCapacity) ||
                _button.GetCapacity().GetType().IsSubclassOf(typeof(ActivableCapacity)))
                )
            {
                CapacityController capacity = listOfCapacity[_ListOfAbilityButton.IndexOf(_button)];
                ActivableCapacity activable = (ActivableCapacity)capacity;
                controlManager.ChangeCapacityActif(activable);
                break;
            }
        }
    }
    private void Start()
    {
        RightClick.action.started += DoRightClick;
        controlManager = FindAnyObjectByType<ControlManager>();
    }
    IEnumerator ChargeBarOfAbility(Image Imagebutton, CapacityController capacity)
    {
        float progressionValue = capacity.actualTime / capacity.GetCooldown();
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
    private void UpdateProgressBarOfACapacity(CapacityController capacity)
    {
        ButtonForCapacity button = _ListOfAbilityButton[listOfCapacity.IndexOf(capacity)];
        StartCoroutine(ChargeBarOfAbility(button.GetComponentsInChildren<Image>()[1], capacity));
    }
    private void RemoveListenerFromCapacities()
    {
        foreach(CapacityController capacity in listOfCapacity)
        {
            capacity.ActivateEvent.RemoveListener(UpdateProgressBarOfACapacity);
            if (capacity.GetType().IsSubclassOf(typeof(ActivableCapacity)))
            {
                ActivableCapacity activable = (ActivableCapacity)capacity;
                activable.changeActif.RemoveListener(_ListOfAbilityButton[listOfCapacity.IndexOf(capacity)].Clignote);
            }
        }
    }
    public void OnDisable()
    {
        RemoveListenerFromCapacities();
    }
    private void ActualiseUi()
    {
        _buttonForBuilding.gameObject.SetActive(_entity.GetComponent<BuilderController>());

        StopAllCoroutines();
        foreach (ButtonForCapacity button in _ListOfAbilityButton)
        {
            button.gameObject.SetActive(true);
            button.onClick.RemoveAllListeners();
            int indexOfAbilityButton = _ListOfAbilityButton.IndexOf(button);
            if (indexOfAbilityButton < listOfCapacity.Count)
            {
                CapacityController capacity = listOfCapacity[indexOfAbilityButton];

                button.SetCapacity(capacity);
                button.isActivable = false;

                if (capacity.GetType() != typeof(PassifCapacity))
                {
                    button.onClick.AddListener(delegate { controlManager.CapacityOrder(capacity); });
                    StartCoroutine(ChargeBarOfAbility(button.GetComponentsInChildren<Image>()[1], capacity));
                    capacity.ActivateEvent.AddListener(UpdateProgressBarOfACapacity);
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
    public void SetEntity(GameObject entity)
    {
        _entity = entity;
        RemoveListenerFromCapacities();
        listOfCapacity = _entity.GetComponentsInChildren<CapacityController>().ToList();
        ActualiseUi();
    }
    public void GoToBuildUi()
    {
        _buildUi.gameObject.SetActive(true);
        _buildUi.SetBuilder(_entity.GetComponent<BuilderController>());
        gameObject.SetActive(false);
    }
}
