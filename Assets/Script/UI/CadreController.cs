using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CadreController : MonoBehaviour
{
    private SelectableManager _entity;

    [SerializeField] private Image image;
    [SerializeField] private TMP_Text text;

    private GroupeUiManager _groupeUiManager;

    public void SetEntity(SelectableManager entity)
    {
        _entity = entity;
        if(_entity != null )
        {
            _entity.changeStats.AddListener(ActualiseHp);
            _entity.deathEvent.AddListener(DestroyHimSelf);
            SetCadre();
        }
        
    }
    public void SetGroupUiManager(GroupeUiManager groupUi)
    {
        _groupeUiManager = groupUi;
    }
    public void SetCadre()
    {
        image.sprite = _entity.GetSprit();
        text.text = _entity.Hp + "/" + _entity.MaxHp;
    }

    private void ActualiseHp()
    {
        text.text = _entity.Hp + "/" + _entity.MaxHp;
    }

    private void DestroyHimSelf(SelectableManager entity)
    {
        if (_groupeUiManager)
        {
            _groupeUiManager.RemoveCadre(transform.gameObject);
        }
        if (_entity)
        {
            _entity.changeStats.RemoveListener(ActualiseHp);
            _entity.deathEvent.RemoveListener(DestroyHimSelf);
        }

        Destroy(gameObject);
    }

    public SelectableManager GetEntity()
    {
        return _entity;
    }
}
