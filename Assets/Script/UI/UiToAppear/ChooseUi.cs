using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChooseUi : UiAppeirBase
{
    [SerializeField] RessourceUi ressourceUi;
    SaveForNextlevel save;
    RessourceController playerRessource;
    [SerializeField] GameObject Ui;

    List<EntityController> _EntitiesAlive = new();
    public List<EntityController> _EntitiesSave = new();
    ControlManager controlManager;
    public List<ButtonForEntityChoice> _ListOfButton = new();

    [SerializeField] GameObject GameobjectOfPlayerStock;
    int currentNumber = 0;
    List<IABrain> ia;

    void Start()
    {

        ia = FindObjectsOfType<IABrain>().ToList();
        save = FindAnyObjectByType<SaveForNextlevel>();
        controlManager = FindAnyObjectByType<ControlManager>();
        playerRessource = controlManager.GetComponent<RessourceController>();

        _ListOfButton = Ui.GetComponentsInChildren<ButtonForEntityChoice>(true).ToList();
        foreach (ButtonForEntityChoice i in _ListOfButton) { i.chooseUi = this; }

        Ui.SetActive(false);
    }

    public override void AppearUI()
    {
        Time.timeScale = 0;
        Ui.SetActive(true);
        FindEveryEntityOfPlayer();
        ActualiseButton();
        ressourceUi.ActualiseData();
        controlManager.ResetUiOrder();
        controlManager.DesactiveController();
        foreach(IABrain i in ia)
        {
            if(i)
            {
                i.gameObject.SetActive(false);
            }
        }
        //FindAnyObjectByType<UiGestioneur>().gameObject.SetActive(false);
        //FindAnyObjectByType<ControlManager>().gameObject.SetActive(false);

    }
    private void ActualiseButton()
    {
        for (int w = currentNumber; w < _ListOfButton.Count + currentNumber; w++)
        {
            ButtonForEntityChoice i = _ListOfButton[w - currentNumber];
            if (w < _EntitiesAlive.Count)
            {
                i.gameObject.SetActive(true);
                i.SetEntity(_EntitiesAlive[w].GetComponent<EntityManager>());
            }
            else
            {
                i.gameObject.SetActive(false);
            }
        }
    }

    public void GoLeft()
    {
        if(_ListOfButton.Count< _EntitiesAlive.Count)
        {
            currentNumber -= _ListOfButton.Count;
            if (currentNumber < 0)
            {
                currentNumber = _EntitiesAlive.Count - _ListOfButton.Count;
            }
        }
        
        ActualiseButton();
    }

    public void GoRight()
    {
        currentNumber += _ListOfButton.Count;
        if (currentNumber > _EntitiesAlive.Count)
        {
            currentNumber = 0;
        }
        ActualiseButton();
    }
    public override void AppearUI(bool IsPlayer) => throw new System.NotImplementedException();
    public override void DisappearUI()
    {
        Time.timeScale = 1;
        Ui.SetActive(false);
    }

    public void AddEntityToSave(EntityController entity)
    {
        EntityManager entityManger = entity.GetComponent<EntityManager>();
        if (playerRessource.CompareRessource(entityManger.WoodCost, entityManger.GoldCost))
        {
            save.SaveEntity(entity);
        }
    }

    public void AddEntitiesToSave()
    {
        save.SaveEntity(_EntitiesSave);
    }

    public bool AddEntityToList(EntityController entity)
    {
        EntityManager entityManger = entity.GetComponent<EntityManager>();
        if (playerRessource.CompareRessource(entityManger.WoodCost, entityManger.GoldCost))
        {
            _EntitiesSave.Add(entity);
            playerRessource.AddGold(-entityManger.GoldCost);
            playerRessource.AddWood(-entityManger.WoodCost);
            return true;
        }
        return false;
    }

    public void RemoveEntityOfList(EntityController entity)
    {
        EntityManager entityManger = entity.GetComponent<EntityManager>();
        _EntitiesSave.Remove(entity);
        playerRessource.AddGold(entityManger.GoldCost);
        playerRessource.AddWood(entityManger.WoodCost);
    }

    public void FindEveryEntityOfPlayer()
    {
        foreach (TroupeManager i in GameobjectOfPlayerStock.GetComponentsInChildren<TroupeManager>())
        {
            if (i && i.CompareTag(controlManager.tag) && i.GetComponent<EntityController>())
            {
                _EntitiesAlive.Add(i.GetComponent<EntityController>());
            }
        }
    }

    public bool FindEntityInSave(EntityController entity)
    {
        return _EntitiesSave.Contains(entity); ;
    }
}
