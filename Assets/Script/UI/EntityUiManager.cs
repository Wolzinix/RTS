using TMPro;
using UnityEngine;

public class EntityUiManager : MonoBehaviour
{
    private SelectableManager _entity;

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

    public void SetEntity(SelectableManager em)
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
            if(typeof(AggressifEntityManager) == _entity.GetType() )
            {
                AggressifEntityManager _entity2 = (AggressifEntityManager) _entity ;
                attack.enabled = true;
                attack.text = "Attack:" + _entity2.Attack;
            }
            else
            {
                attack.enabled = false;
            }
           
            defense.text = "Defense:" + _entity.Defense;
        }
    }

    private void DisableUI(SelectableManager entity)
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

    public SelectableManager GetEntity()
    {
        return _entity;
    }
}
