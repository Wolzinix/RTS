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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetEntity(EntityManager entity)
    {
        _entity = entity;
        _entity.changeStats.AddListener(ActualiseHp);
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
    
}
