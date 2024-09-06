using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.UI;

public class CadreController : MonoBehaviour
{
    private EntityManager _entity;

    [SerializeField] private Image image;

    [SerializeField] private TMP_Text text;

    public void SetEntity(EntityManager entity)
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

    private void DestroyHimSelf()
    {
        FindObjectOfType<GroupUiManager>().RemoveCadre(transform.gameObject);
        _entity.changeStats.RemoveListener(ActualiseHp);
        _entity.deathEvent.RemoveListener(DestroyHimSelf);
        Destroy(gameObject);
    }
    
}
