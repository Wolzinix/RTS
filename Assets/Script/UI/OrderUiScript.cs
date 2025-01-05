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

    private void Start()
    {
        RightClick.action.started += DoRightClick;
    }
    public void SetEntity(GameObject entity)
    {
        _entity = entity;
        ActualiseUi();
    }

    private void ActualiseUi()
    {
        if (_entity && _entity.GetComponent<BuilderController>()) { _button.gameObject.SetActive(true); }
        else { _button.gameObject.SetActive(false); }

        if(_entity.GetComponent<TroupeManager>())
        {
            List<CapacityController> listOfCapacaity = _entity.GetComponentsInChildren<CapacityController>().ToList();
            foreach (Button button in _ListOfButton)
            {
                if (_ListOfButton.IndexOf(button) < listOfCapacaity.Count)
                {
                    CapacityController capacity = listOfCapacaity[_ListOfButton.IndexOf(button)];

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
                        button.onClick.AddListener(delegate { FindAnyObjectByType<ControlManager>().CapacityOrder(_entity.GetComponent<TroupeManager>(), capacity); });
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

    public void GoToBuildUi()
    {
        _buildUi.gameObject.SetActive(true);
        _buildUi.SetBuilder(_entity.GetComponent<BuilderController>());
        gameObject.SetActive(false);
    }

    private List<RaycastResult> DoUiRayCast()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        List<RaycastResult> results = new List<RaycastResult>();
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
                CapacityController capacity = listOfCapacaity[_ListOfButton.IndexOf(_button)];
                if (capacity.GetType().IsSubclassOf(typeof(ActivableCapacity)))
                {
                    ActivableCapacity activable = (ActivableCapacity)capacity;
                    FindAnyObjectByType<ControlManager>().ChangeCapacityActif(activable);
                }
            }
        }
    }
}
