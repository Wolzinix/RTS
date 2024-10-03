using TMPro;
using UnityEngine;

public class EntityUiManager : MonoBehaviour
{
    private TroupeManager _entity;

    [SerializeField] private TMP_Text entityName;
    [SerializeField] private TMP_Text hp;
    [SerializeField] private TMP_Text attack;
    [SerializeField] private TMP_Text defense;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        UpdateUI();
        if (_entity)
        {
            _entity.changeStats.AddListener(UpdateUI);
            _entity.deathEvent.AddListener(DisableUI);
        }
    }

    public void SetEntity(TroupeManager em)
    {
        _entity = em;
        _entity.changeStats.AddListener(UpdateUI);
        _entity.deathEvent.AddListener(DisableUI);
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (_entity)
        {
            entityName.text = _entity.gameObject.name;
            hp.text = "HP:" + _entity.Hp +" / " + _entity.MaxHp;
            attack.text = "Attack:" + _entity.Attack;
            defense.text = "Defense:" + _entity.Defense;
        }
    }

    private void DisableUI(TroupeManager entity)
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (_entity)
        {
            _entity.changeStats.RemoveListener(UpdateUI);
            _entity.deathEvent.RemoveListener(DisableUI);
        }
    }

    public TroupeManager GetEntity()
    {
        return _entity;
    }
}
