using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonForEntityChoice : Button
{
    public ChooseUi chooseUi;

    public EntityManager entityManager;

    bool EntityIsTaken;

    [SerializeField] TMP_Text _Text;
    [SerializeField] Image Imageentity;
    [SerializeField] Image ImageOver;

    protected override void Start()
    {
        onClick.AddListener(ClickGestion) ;
    }
    public void SetEntity(EntityManager entity)
    {
        entityManager = entity;
        TroupeManager troupe = (TroupeManager)entity;
        _Text.text = entityManager.name + " - " + troupe.level;
        Imageentity.sprite = entityManager.GetSprit();

        ImageOver.enabled = false;
        GetComponentInChildren<RessourceInfo>().SetRessource(
            Mathf.RoundToInt(troupe.GoldCost * (1+ troupe.level * 0.1f))
            , Mathf.RoundToInt(troupe.WoodCost * (1 + troupe.level * 0.1f)));

        if (chooseUi.FindEntityInSave(entity.GetComponent<EntityController>())) 
        {
            EntityIsTaken = true;
            ImageOver.enabled = true;
        }
    }

    private void ClickGestion()
    {
        EntityController entity = entityManager.GetComponent<EntityController>(); 
        if (!EntityIsTaken && entity)
        {
            EntityIsTaken = chooseUi.AddEntityToList(entity);
            if(EntityIsTaken)
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
