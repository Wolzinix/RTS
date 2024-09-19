using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BuildingController;
using static UnityEngine.EventSystems.EventTrigger;

public class BuildUi : MonoBehaviour
{

    private BuilderManager _builder;

    [SerializeField] private List<Button> _ListOfButton;

    private int _numberOfbutton;

    void Start()
    {
      gameObject.SetActive(false);
    }

    public void SetBuilder(BuilderManager builder) 
    {
        _builder = builder;
        ActualiseButtons();
    }

    public void ActualiseText()
    {
        List<GameObject> listOfGameobject = _builder.getBuildings();
        foreach (Button button in _ListOfButton)
        {
            if (_ListOfButton.IndexOf(button) + _numberOfbutton < listOfGameobject.Count())
            {
                if (button.IsActive())
                {
                    button.GetComponentInChildren<TMP_Text>().text = "tchoutchou";
                }
            }
        }
    }


    private void ActualiseButtons()
    {
        List<GameObject> listOfGameobject = _builder.getBuildings();

        foreach (Button button in _ListOfButton) 
        {
            if (_ListOfButton.IndexOf(button)+ _numberOfbutton < listOfGameobject.Count())
            {
                button.gameObject.SetActive(true);
                button.image.sprite = listOfGameobject[_ListOfButton.IndexOf(button) + _numberOfbutton].GetComponent<EntityManager>().GetSprit();
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
            if(_builder.getBuildings().Count >= _ListOfButton.Count())
            {
                _numberOfbutton =  _ListOfButton.Count() *( _builder.getBuildings().Count / _ListOfButton.Count());
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

        if (_numberOfbutton >= _builder.getBuildings().Count)
        {
             _numberOfbutton = 0;
        }
        ActualiseButtons();
    }
}
