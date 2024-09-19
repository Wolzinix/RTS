using UnityEngine;
using UnityEngine.UI;

public class OrderUiScript : MonoBehaviour
{

    GameObject _entity;

    [SerializeField] Button _button;
    [SerializeField] BuildUi _buildUi;

    public void SetEntity(GameObject entity)
    {
        _entity = entity;
        ActualiseUi();
    }

    private void ActualiseUi()
    {
        if (_entity && _entity.GetComponent<BuilderManager>())
        {
            _button.gameObject.SetActive(true);
        }
        else
        {
            _button.gameObject.SetActive(false);
        }
    }

    public void GoToBuildUi()
    {
        _buildUi.gameObject.SetActive(true);
        _buildUi.SetBuilder(_entity.GetComponent<BuilderManager>());
        gameObject.SetActive(false);
    }
}
