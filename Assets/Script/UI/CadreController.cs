using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CadreController : MonoBehaviour
{
    private TroupeManager _entity;

    [SerializeField] private Image image;

    [SerializeField] private TMP_Text text;

    public void SetEntity(TroupeManager entity)
    {
        _entity = entity;
        _entity.changeStats.AddListener(ActualiseHp);
        _entity.deathEvent.AddListener(DestroyHimSelf);
        SetCadre();
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

    private void DestroyHimSelf(TroupeManager entity)
    {
        if(FindObjectOfType<GroupeUiManager>())
        {
            FindObjectOfType<GroupeUiManager>().RemoveCadre(transform.gameObject);
        }
        if(_entity)
        {
            _entity.changeStats.RemoveListener(ActualiseHp);
            _entity.deathEvent.RemoveListener(DestroyHimSelf);
        }
        
        Destroy(gameObject);
    }

    public TroupeManager GetEntity()
    {
        return _entity;
    }
}
