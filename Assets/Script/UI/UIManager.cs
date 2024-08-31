using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private EntityManager _entity;

    [SerializeField] private TMP_Text hp;

    private void OnEnable()
    {
        UpdateUI();
        _entity.changeStats.AddListener(UpdateUI);
        
    }

    public void setEntity(EntityManager _em)
    {
        _entity = _em;
    }

    public void UpdateUI()
    {
        hp.text = "HP:" + _entity.Hp;
    }

    private void OnDisable()
    {
        _entity.changeStats.RemoveListener(UpdateUI);
    }
}
