using System.Collections.Generic;
using UnityEngine;

public class ChooseUi : UIAppearBase
{
    [SerializeField] GameObject GameobjectOfPlayerStock;

    SaveForNextlevel save;
    RessourceController playerResource;
    ControlManager controlManager;
    RessourceUi resourceUi;
    int currentNumber = 0;
    Canvas UI;

    ButtonForEntityChoice[] _ListOfButtons;
    List<EntityController> _EntitiesSave = new();
    List<EntityManager> _EntitiesAlive = new();
    AIBrain[] AI;
    void Start()
    {
        AI = FindObjectsOfType<AIBrain>();
        save = FindAnyObjectByType<SaveForNextlevel>();
        controlManager = FindAnyObjectByType<ControlManager>();
        playerResource = controlManager.GetComponent<RessourceController>();
        UI = GetComponentInChildren<Canvas>();
        resourceUi = GetComponentInChildren<RessourceUi>();

        _ListOfButtons = UI.GetComponentsInChildren<ButtonForEntityChoice>(true);
        foreach (ButtonForEntityChoice i in _ListOfButtons) { i.chooseUi = this; }

        UI.enabled = false;
    }
    private void ActualiseButton()
    {
        for (int w = currentNumber; w < _ListOfButtons.Length + currentNumber; w++)
        {
            ButtonForEntityChoice i = _ListOfButtons[w - currentNumber];
            if (w < _EntitiesAlive.Count)
            {
                i.gameObject.SetActive(true);
                i.SetEntity(_EntitiesAlive[w]);
            }
            else { i.gameObject.SetActive(false); }
        }
    }
    public void FindEveryEntityOfPlayer()
    {
        foreach (TroupeManager i in GameobjectOfPlayerStock.GetComponentsInChildren<TroupeManager>())
        {
            if (i)
            {
                _EntitiesAlive.Add(i.GetComponent<EntityManager>());
            }
        }
    }
    public override void AppearUI()
    {
        Time.timeScale = 0;
        UI.enabled = true;
        FindEveryEntityOfPlayer();
        ActualiseButton();
        resourceUi.UpdateData();
        controlManager.ResetUiOrder();
        controlManager.DesactiveController();
        foreach (AIBrain i in AI)
        {
            if (i)
            {
                i.gameObject.SetActive(false);
            }
        }
    }

    public void GoLeft()
    {
        if (_ListOfButtons.Length < _EntitiesAlive.Count)
        {
            currentNumber -= _ListOfButtons.Length;
            if (currentNumber < 0)
            {
                currentNumber = _EntitiesAlive.Count - _ListOfButtons.Length;
            }
        }

        ActualiseButton();
    }

    public void GoRight()
    {
        currentNumber += _ListOfButtons.Length;
        if (currentNumber > _EntitiesAlive.Count)
        {
            currentNumber = 0;
        }
        ActualiseButton();
    }
    public override void AppearUI(bool IsPlayer){ AppearUI(); }
    public override void DisappearUI()
    {
        Time.timeScale = 1;
        UI.enabled = false;
    }

    public void AddEntityToSave(EntityController entity)
    {
        EntityManager entityManger = entity.GetComponent<EntityManager>();
        if (playerResource.CompareRessource(entityManger.WoodCost, entityManger.GoldCost))
        {
            save.SaveEntity(entity);
        }
    }

    public void AddEntitiesToSave()
    {
        save.SaveEntity(_EntitiesSave);
        DisappearUI();
    }

    public bool AddEntityToList(EntityController entity)
    {
        EntityManager entityManger = entity.GetComponent<EntityManager>();
        if (playerResource.CompareRessource(entityManger.WoodCost, entityManger.GoldCost))
        {
            _EntitiesSave.Add(entity);
            playerResource.AddGold(-entityManger.GoldCost);
            playerResource.AddWood(-entityManger.WoodCost);
            return true;
        }
        return false;
    }

    public void RemoveEntityOfList(EntityController entity)
    {
        EntityManager entityManger = entity.GetComponent<EntityManager>();
        _EntitiesSave.Remove(entity);
        playerResource.AddGold(entityManger.GoldCost);
        playerResource.AddWood(entityManger.WoodCost);
    }

    public bool FindEntityInSave(EntityController entity)
    {
        return _EntitiesSave.Contains(entity); ;
    }
}
