using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private EntityManager _entity;

    
    [SerializeField] private TMP_Text name;
    [SerializeField] private TMP_Text hp;
    [SerializeField] private TMP_Text attack;
    [SerializeField] private TMP_Text defense;

    private void OnEnable()
    {
        UpdateUI();
        _entity.changeStats.AddListener(UpdateUI);
        _entity.deathEvent.AddListener(DisableUI);
    }

    public void setEntity(EntityManager _em)
    {
        _entity = _em;
        _entity.changeStats.AddListener(UpdateUI);
        _entity.deathEvent.AddListener(DisableUI);
        UpdateUI();
    }

    public void UpdateUI()
    {
        name.text = _entity.gameObject.name;
        hp.text = "HP:" + _entity.Hp +" / " + _entity.MaxHp;
        attack.text = "Attack:" + _entity.Attack;
        defense.text = "Defense:" + _entity.Defense;
    }

    private void DisableUI()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        _entity.changeStats.RemoveListener(UpdateUI);
        _entity.deathEvent.RemoveListener(DisableUI);
    }
}
