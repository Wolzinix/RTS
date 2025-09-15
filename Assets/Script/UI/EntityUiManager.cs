using Assets.Script.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class EntityUiManager : MonoBehaviour
{
    private SelectableManager _entity;
    [SerializeField] public Image backgroundImage;

    [SerializeField] private TMP_Text entityName;
    [SerializeField] private TMP_Text hp;
    [SerializeField] private TMP_Text attack;
    [SerializeField] private TMP_Text defense;

    [SerializeField] private TMP_Text level;

    IconAffichage iconAffichage;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void DisableUI(SelectableManager entity)
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if(!iconAffichage)
        {
            iconAffichage = GetComponentInChildren<IconAffichage>();
        }
    }

    public void SetEntity(SelectableManager em)
    {
        if(em != null) 
        {
            _entity = em;
            SetUpIconEffect();

            _entity.changeStats.AddListener(UpdateUI);
            _entity.deathEvent.AddListener(DisableUI);
            
            UpdateUI();
        }
    }

    private void SetUpIconEffect()
    {
        iconAffichage.ClearEffect();
        _entity.AddEffectEvent.AddListener(iconAffichage.AddEffect);
        _entity.RemoveEffectEvent.AddListener(iconAffichage.RemoveEffect);
        foreach (StateEffect effect in _entity._listOfEffects)
        {
            iconAffichage.AddEffect(effect);
        }
    }

    public void UpdateUI()
    {
        if (_entity)
        {
            entityName.text = _entity.gameObject.name;
            hp.text = string.Concat("HP:", _entity.Hp, " / ", _entity.MaxHp);
            defense.text = string.Concat("Defense:", _entity.Defense);
            attack.enabled = false;
            level.enabled = false;

            if (_entity.IsAggressifEntity())
            {
                AggressifEntityManager _entity2 = (AggressifEntityManager)_entity;
                attack.enabled = true;
                attack.text = string.Concat("Attack:", _entity2.Attack);
                if (typeof(TroupeManager) == _entity.GetType())
                {
                    TroupeManager _entity3 = (TroupeManager)_entity;
                    level.enabled = true;
                    level.text = string.Concat("Level:", _entity3.level);
                }
            }
        }
    }
    private void OnDisable()
    {
        if (_entity)
        {
            _entity.AddEffectEvent.RemoveAllListeners();
            _entity.RemoveEffectEvent.RemoveAllListeners();
            _entity.changeStats.RemoveListener(UpdateUI);
            _entity.deathEvent.RemoveListener(DisableUI);
        }
    }

    public SelectableManager GetEntity()
    {
        return _entity;
    }
}
