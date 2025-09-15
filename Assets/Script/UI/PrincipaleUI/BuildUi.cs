using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BuildUi : MonoBehaviour
{
    [SerializeField] private List<ButtonForEntity> _ListOfButton;

    private BuilderController _builder;
    private int _numberOfbutton;
    private ControlManager _controlManager;

    void Start()
    {
        _controlManager = FindAnyObjectByType<ControlManager>();
        gameObject.SetActive(false);
    }

    public void SetBuilder(BuilderController builder)
    {
        _builder = builder;
        ActualiseButtons();
    }

    public void ActualiseText()
    {
        List<GameObject> listOfGameobject = _builder.GetBuildings();
        foreach (ButtonForEntity button in _ListOfButton)
        {
            int indexOfButtonWithNumber = _ListOfButton.IndexOf(button) + _numberOfbutton;
            if (indexOfButtonWithNumber < listOfGameobject.Count())
            {
                if (button.IsActive())
                {
                    button.GetComponentInChildren<TMP_Text>().SetText(_builder.GetBuildings()[indexOfButtonWithNumber - _numberOfbutton].name) ;
                }
            }
        }
    }


    private void ActualiseButtons()
    {
        List<GameObject> listOfGameobject = _builder.GetBuildings();

        foreach (ButtonForEntity button in _ListOfButton)
        {
            if (_ListOfButton.IndexOf(button) + _numberOfbutton < listOfGameobject.Count())
            {
                button.gameObject.SetActive(true);
                button.SetEntity(listOfGameobject[_ListOfButton.IndexOf(button) + _numberOfbutton].GetComponent<SelectableManager>());
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate { _controlManager.DoABuilding(_ListOfButton.IndexOf(button) + _numberOfbutton, _builder.GetBuildings()[_ListOfButton.IndexOf(button) + _numberOfbutton]); });
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
            if (_builder.GetBuildings().Count > _ListOfButton.Count())
            {
                _numberOfbutton = _ListOfButton.Count() * (_builder.GetBuildings().Count / _ListOfButton.Count());
            }
            else
            {
                _numberOfbutton = 0;
            }
        }
        ActualiseButtons();
    }

    public void GoToRight()
    {
        _numberOfbutton += _ListOfButton.Count();

        if (_numberOfbutton >= _builder.GetBuildings().Count)
        {
            _numberOfbutton = 0;
        }
        ActualiseButtons();
    }
}
