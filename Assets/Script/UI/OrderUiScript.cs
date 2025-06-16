using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OrderUiScript : MonoBehaviour
{
    GameObject _entity;

    [SerializeField] Button _button;
    [SerializeField] BuildUi _buildUi;

    [SerializeField] List<Button> _ListOfButton;
    [SerializeField] private InputActionReference RightClick;
    private List<CapacityController> listOfCapacity = new();
    private Dictionary<int,bool> DicOfCapacityClignote = new();

    ControlManager controlManager;

    private void Start()
    {
        RightClick.action.started += DoRightClick;
        controlManager = FindAnyObjectByType<ControlManager>();
    }
    private void LateUpdate()
    {
        if (DicOfCapacityClignote.Keys.Count > 0)
        {
            foreach (int i in DicOfCapacityClignote.Keys)
            {
                if (DicOfCapacityClignote[i])
                {
                    _ListOfButton[i].GetComponent<Image>().enabled = !_ListOfButton[i].GetComponent<Image>().enabled;
                }
            }
        }
    }
    private void RemoveListenerFromCapacity()
    {
        foreach(CapacityController capacity in listOfCapacity)
        {
            capacity.ActivateEvent.RemoveListener(ActualiseACapacity);
            if (capacity.GetType().IsSubclassOf(typeof(ActivableCapacity)))
            {
                ActivableCapacity activable = (ActivableCapacity)capacity;
                activable.changeActif.RemoveListener(Clignote);
            }
        }
        DicOfCapacityClignote.Clear();
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
        if (_entity && _entity.GetComponent<BuilderController>()) { _button.gameObject.SetActive(true); }
        else { _button.gameObject.SetActive(false); }

        if(_entity.GetComponent<TroupeManager>())
        {
            StopAllCoroutines();
            foreach (Button button in _ListOfButton)
            {
                button.GetComponent<Image>().enabled = true;
                if (_ListOfButton.IndexOf(button) < listOfCapacity.Count)
                {
                    CapacityController capacity = listOfCapacity[_ListOfButton.IndexOf(button)];

                    button.gameObject.SetActive(true);
                    button.GetComponentInChildren<TMP_Text>().text = capacity.Name;
                    button.GetComponentsInChildren<Image>()[1].sprite = capacity.sprite;
                    if (capacity.GetType().IsSubclassOf(typeof(PassifCapacity)))
                    {
                        button.GetComponent<Button>().enabled = false;
                    }
                    else
                    {
                        button.GetComponent<Button>().enabled = true;
                        button.onClick.RemoveAllListeners();
                        button.onClick.AddListener(delegate { controlManager.CapacityOrder(capacity); });
                        StartCoroutine(ChargeBarOfAbility(button.GetComponentsInChildren<Image>()[1], capacity));
                        capacity.ActivateEvent.AddListener(ActualiseACapacity);
                    }
                    if (capacity.GetType().IsSubclassOf(typeof(ActivableCapacity)))
                    {
                        ActivableCapacity activable = (ActivableCapacity)capacity;
                        DicOfCapacityClignote[_ListOfButton.IndexOf(_button)] = false;
                        activable.changeActif.AddListener(Clignote);
                        Clignote(activable);
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
            foreach (Button button in _ListOfButton)
            {
                button.onClick.RemoveAllListeners();
                button.gameObject.SetActive(false);
            }
        }
    }

    private void ActualiseACapacity(CapacityController capacity)
    {
        Button button = _ListOfButton[listOfCapacity.IndexOf(capacity)];
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
            if (raycastResult.gameObject.GetComponent<Button>())
            {
                List<CapacityController> listOfCapacaity = _entity.GetComponentsInChildren<CapacityController>().ToList();
                Button _button = raycastResult.gameObject.GetComponent<Button>();
                if(_ListOfButton.Contains(_button))
                {
                    CapacityController capacity = listOfCapacaity[_ListOfButton.IndexOf(_button)];
                    if (capacity.GetType().IsSubclassOf(typeof(ActivableCapacity)))
                    {
                        ActivableCapacity activable = (ActivableCapacity)capacity;

                        controlManager.ChangeCapacityActif(activable);
                    }
                }
            }
        }
    }

    private void Clignote(ActivableCapacity activable)
    {
        DicOfCapacityClignote[listOfCapacity.IndexOf(activable)] = activable.actif;
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
