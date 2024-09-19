using UnityEngine;
using UnityEngine.UI;

public class OrderUiScript : MonoBehaviour
{

    GameObject _entity;

    [SerializeField] Button _button;

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
}
