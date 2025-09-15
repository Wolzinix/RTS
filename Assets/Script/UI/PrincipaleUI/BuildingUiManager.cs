using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BuildingUiManager : MonoBehaviour
{

    [SerializeField] private List<ButtonForEntity> _ListOfButton;

    private int _numberOfbutton;
    private ProductBuildingController _building;
    private RessourceController _controlManagerRessourceController;

    void Start()
    {
        _controlManagerRessourceController = FindAnyObjectByType<ControlManager>().GetComponent<RessourceController>();
        gameObject.SetActive(false);
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
            int indexOfButton = _ListOfButton.IndexOf(button);
            if (indexOfButton + _numberOfbutton < listOfGameobject.Count())
            {
                if (button.IsActive())
                {
                    button.GetComponentInChildren<TMP_Text>().SetText("{0} / {1}",
                        _building.GetEntityDictionary()[listOfGameobject[indexOfButton + _numberOfbutton]].actualStock,
                        _building.GetEntityDictionary()[listOfGameobject[indexOfButton + _numberOfbutton]].totalStock);
                }
            }
        }
    }


    private void ActualiseButtons()
    {
        GameObject[] listOfGameobject = _building.GetEntityDictionary().Keys.ToArray();

        foreach (ButtonForEntity button in _ListOfButton)
        {
            int IndexOfButtonWithIsPlace = _ListOfButton.IndexOf(button) + _numberOfbutton;
            if (IndexOfButtonWithIsPlace < listOfGameobject.Count())
            {
                if(listOfGameobject[IndexOfButtonWithIsPlace])
                {
                    button.gameObject.SetActive(true);
                    GameObject entity = listOfGameobject[IndexOfButtonWithIsPlace];
                    button.SetEntity(entity.GetComponent<TroupeManager>());
                    
                    
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(delegate { _building.AllySpawnEntity(entity, _controlManagerRessourceController); });
                }
                else{ button.gameObject.SetActive(false);}
                
            }
            else{ button.gameObject.SetActive(false); }
        }
        ActualiseText();
    }


    public void GoToLeft()
    {
        _numberOfbutton -= _ListOfButton.Count();
        if (_numberOfbutton < 0)
        {
            int LenghtOfEntityBuilding = _building.GetEntityDictionary().Keys.Count;
            if (LenghtOfEntityBuilding > _ListOfButton.Count())
            {
                _numberOfbutton = _ListOfButton.Count() * (LenghtOfEntityBuilding / _ListOfButton.Count());
            }
            else { _numberOfbutton = 0; }
        }
        ActualiseButtons();
    }

    public void GoToRight()
    {
        _numberOfbutton += _ListOfButton.Count();

        if (_numberOfbutton >= _building.GetEntityDictionary().Keys.Count)
        {
            _numberOfbutton = 0;
        }
        ActualiseButtons();
    }
}
