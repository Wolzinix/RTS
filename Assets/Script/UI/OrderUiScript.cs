using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OrderUiScript : MonoBehaviour
{
    GameObject _entity;

    [SerializeField] Button _button;
    [SerializeField] BuildUi _buildUi;

    [SerializeField] List<Button> _ListOfButton;

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
                    button.gameObject.SetActive(true);
                    button.onClick.RemoveAllListeners();
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
}
