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
        if (_entity && _entity.GetComponent<EntityController>())
        {
            _button.enabled = true;
        }
        else
        {
            _button.enabled = false;
        }
    }
}
