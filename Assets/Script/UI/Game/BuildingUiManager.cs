using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BuildingUiManager : MonoBehaviour
{
    private ProductBuildingController _building;

    [SerializeField] private List<ButtonForEntity> _ListOfButton;

    private int _numberOfbutton;

    private RessourceController _controlManagerRessourceController;

    void Start()
    {
        gameObject.SetActive(false);
        _controlManagerRessourceController = FindAnyObjectByType<ControlManager>().GetComponent<RessourceController>();
    }

    public void SetBuilding(ProductBuildingController building)
    {
        _building = building;
        ActualiseButtons();
        _building.entitySpawnNow.AddListener(ActualiseText);
    }

    public void ActualiseText()
    {
        GameObject[] listOfGameobject = _building.GetEntityDictionary().Keys.ToArray();
        foreach (ButtonForEntity button in _ListOfButton)
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

        foreach (ButtonForEntity button in _ListOfButton)
        {
            if (_ListOfButton.IndexOf(button) + _numberOfbutton < listOfGameobject.Count())
            {
                if(listOfGameobject[_ListOfButton.IndexOf(button) + _numberOfbutton])
                {
                    button.gameObject.SetActive(true);
                    button.SetEntity(listOfGameobject[_ListOfButton.IndexOf(button) + _numberOfbutton].GetComponent<TroupeManager>());
                    GameObject entity = listOfGameobject[_ListOfButton.IndexOf(button) + _numberOfbutton];
                    button.image.sprite = listOfGameobject[_ListOfButton.IndexOf(button) + _numberOfbutton].GetComponent<TroupeManager>().GetSprit();

                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(delegate { _building.AllySpawnEntity(entity, _controlManagerRessourceController); });
                }
                else
                {
                    button.gameObject.SetActive(false);
                }
                
            }
            else
            {
                button.gameObject.SetActive(false);
            }
        }
        ActualiseText();
    }


    public void GoToLeft()
    {
        _numberOfbutton -= _ListOfButton.Count();
        if (_numberOfbutton < 0)
        {
            if (_building.GetEntityDictionary().Keys.ToArray().Length > _ListOfButton.Count())
            {
                _numberOfbutton = _ListOfButton.Count() * (_building.GetEntityDictionary().Keys.ToArray().Length / _ListOfButton.Count());
            }
            else { _numberOfbutton = 0; }
        }
        ActualiseButtons();
    }

    public void GoToRight()
    {
        _numberOfbutton += _ListOfButton.Count();

        if (_numberOfbutton >= _building.GetEntityDictionary().Keys.ToArray().Length)
        {
            _numberOfbutton = 0;
        }
        ActualiseButtons();
    }
}
