using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonForEntityChoice : Button
{
    public ChooseUi chooseUi;
    public EntityManager entityManager;


    [SerializeField] TMP_Text _Text;
    [SerializeField] Image ImageEntity;
    [SerializeField] Image ImageOver;

    bool onlyOneByFrame;
    RessourceInfo ressourceInfo;

    private EntityController entityController;
    bool EntityIsTaken;

    protected override void Start()
    {
        onClick.AddListener(ClickGestion) ;
        ressourceInfo = GetComponentInChildren<RessourceInfo>();
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
            _Text.text = string.Concat(entity.name, " - ", troupe.level);
            ImageEntity.sprite = entity.GetSprit();

            entityManager = entity.GetComponent<EntityManager>();
            entityController = entity.GetComponent<EntityController>();

            ImageOver.enabled = false;
            ressourceInfo.SetRessource(
                Mathf.RoundToInt(troupe.GoldCost * (1 + troupe.level * 0.1f))
                , Mathf.RoundToInt(troupe.WoodCost * (1 + troupe.level * 0.1f)));

            if (chooseUi.FindEntityInSave(entityController))
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
            if (!EntityIsTaken && entityController)
            {
                EntityIsTaken = chooseUi.AddEntityToList(entityController);
                if (EntityIsTaken)
                {
                    ImageOver.enabled = true;
                }
            }
            else
            {
                EntityIsTaken = false;
                chooseUi.RemoveEntityOfList(entityController);

                ImageOver.enabled = false;
            }
        }
    }
}
