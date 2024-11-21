using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildUi : MonoBehaviour
{

    private BuilderController _builder;

    [SerializeField] private List<Button> _ListOfButton;

    private int _numberOfbutton;

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetBuilder(BuilderController builder)
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
                    button.GetComponentInChildren<TMP_Text>().text = _builder.getBuildings()[_ListOfButton.IndexOf(button)].name;
                }
            }
        }
    }


    private void ActualiseButtons()
    {
        List<GameObject> listOfGameobject = _builder.getBuildings();

        foreach (Button button in _ListOfButton)
        {
            if (_ListOfButton.IndexOf(button) + _numberOfbutton < listOfGameobject.Count())
            {
                button.gameObject.SetActive(true);
                button.image.sprite = listOfGameobject[_ListOfButton.IndexOf(button) + _numberOfbutton].GetComponent<SelectableManager>().GetSprit();
                button.onClick.AddListener(delegate { FindAnyObjectByType<ControlManager>().DoABuilding(_ListOfButton.IndexOf(button) + _numberOfbutton); });
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
        if (_numberOfbutton < 0)
        {
            if (_builder.getBuildings().Count >= _ListOfButton.Count())
            {
                _numberOfbutton = _ListOfButton.Count() * (_builder.getBuildings().Count / _ListOfButton.Count());
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
