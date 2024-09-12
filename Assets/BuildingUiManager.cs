using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BuildingController;
using static UnityEngine.EventSystems.EventTrigger;

public class BuildingUiManager : MonoBehaviour
{

    private BuildingController _building;

    [SerializeField] private List<Button> _ListOfButton;

    private int _numberOfbutton;

    void Start()
    {
      gameObject.SetActive(false);
    }

    public void SetBuilding(BuildingController building) 
    {
        _building = building;
        ActualiseButtons();
        _building.entitySpawnNow.AddListener(ActualiseText);
    }

    public void ActualiseText()
    {
        GameObject[] listOfGameobject = _building.GetEntityDictionary().Keys.ToArray();
        foreach (Button button in _ListOfButton)
        {
            if (_ListOfButton.IndexOf(button) + _numberOfbutton < listOfGameobject.Count())
            {
                if (button.IsActive())
                {
                    button.GetComponentInChildren<TMP_Text>().text =
                    _building.GetEntityDictionary()[listOfGameobject[_ListOfButton.IndexOf(button) + _numberOfbutton]].actualStock
                            + " / " +
                    _building.GetEntityDictionary()[listOfGameobject[_ListOfButton.IndexOf(button) + _numberOfbutton]].totalStock;
                }
            }
        }
    }


    private void ActualiseButtons()
    {
        GameObject[] listOfGameobject = _building.GetEntityDictionary().Keys.ToArray();

        foreach (Button button in _ListOfButton) 
        {
            if (_ListOfButton.IndexOf(button)+ _numberOfbutton < listOfGameobject.Count())
            {
                button.gameObject.SetActive(true);
                GameObject entity = listOfGameobject[_ListOfButton.IndexOf(button) + _numberOfbutton];
                button.image.sprite = listOfGameobject[_ListOfButton.IndexOf(button) + _numberOfbutton].GetComponent<EntityManager>().GetSprit();

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate { _building.AllySpawnEntity(entity); });

            }
            else
            {
                button.gameObject.SetActive(false);
            }
        }
        ActualiseText();
    }


    public void goToLeft()
    {
        _numberOfbutton -= _ListOfButton.Count();
        if( _numberOfbutton < 0 )
        {
            if(_building.GetEntityDictionary().Keys.ToArray().Length >= _ListOfButton.Count())
            {
                _numberOfbutton =  _ListOfButton.Count() *( _building.GetEntityDictionary().Keys.ToArray().Length / _ListOfButton.Count());
            }
            else
            {
                _numberOfbutton = 0;
            }
        }
        ActualiseButtons();
    }

    public void goToRight()
    {
        _numberOfbutton += _ListOfButton.Count();

        if (_numberOfbutton >= _building.GetEntityDictionary().Keys.ToArray().Length)
        {
             _numberOfbutton = 0;
        }
        ActualiseButtons();
    }
}
