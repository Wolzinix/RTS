using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonForEntityChoice : Button
{
    public ChooseUi chooseUi;
    public EntityManager entityManager;

    bool EntityIsTaken;

    [SerializeField] TMP_Text _Text;
    [SerializeField] Image ImageEntity;
    [SerializeField] Image ImageOver;

    bool onlyOneByFrame;

    protected override void Start()
    {
        onClick.AddListener(ClickGestion) ;
    }
    protected override void OnDestroy()
    {
        onClick.RemoveListener(ClickGestion) ;
    }

    private void LateUpdate()
    {
        if(onlyOneByFrame) { onlyOneByFrame = false; }
    }
    public void SetEntity(EntityManager entity)
    {
        if(entity)
        {
            TroupeManager troupe = (TroupeManager)entity;
            _Text.text = entity.name + " - " + troupe.level;
            ImageEntity.sprite = entity.GetSprit();

            entityManager = entity.GetComponent<EntityManager>();

            ImageOver.enabled = false;
            GetComponentInChildren<RessourceInfo>().SetRessource(
                Mathf.RoundToInt(troupe.GoldCost * (1 + troupe.level * 0.1f))
                , Mathf.RoundToInt(troupe.WoodCost * (1 + troupe.level * 0.1f)));

            if (chooseUi.FindEntityInSave(entity.GetComponent<EntityController>()))
            {
                EntityIsTaken = true;
                ImageOver.enabled = true;
            }
        }
       
    }

    private void ClickGestion()
    {
        if(!onlyOneByFrame)
        {
            onlyOneByFrame = true;
            EntityController entity = entityManager.GetComponent<EntityController>();

            if (!EntityIsTaken && entity)
            {

                EntityIsTaken = chooseUi.AddEntityToList(entity);
                if (EntityIsTaken)
                {
                    ImageOver.enabled = true;
                }
            }
            else
            {
                EntityIsTaken = false;
                chooseUi.RemoveEntityOfList(entity);

                ImageOver.enabled = false;
            }
        }
        
    }
}
