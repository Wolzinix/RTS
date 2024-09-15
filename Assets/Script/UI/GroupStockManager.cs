using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GroupStockManager : MonoBehaviour
{
    private List<EntityManager> _listOfEntityManager;

    private int _nbOfEntity;

    void Start()
    {
        _listOfEntityManager = new List<EntityManager>();
    }


    public void SetList(List<EntityManager> listOfEntityManager)
    {
        _listOfEntityManager = listOfEntityManager;
        _nbOfEntity = _listOfEntityManager.Count;
        ActualiseAffichage();
        foreach (EntityManager entityManager in _listOfEntityManager)
        {
            entityManager.deathEvent.AddListener(RemoveEntity);
        }
    }

    public void ResetList (){ _listOfEntityManager.Clear(); }

    private void RemoveEntity(EntityManager entityManager)
    {
        _listOfEntityManager.Remove(entityManager);
        entityManager.deathEvent.RemoveListener(RemoveEntity);
        _nbOfEntity -= 1;
        ActualiseAffichage();
    }

    private  void ActualiseAffichage()
    {
        GetComponentInChildren<TMP_Text>().text = _nbOfEntity.ToString();
        GetComponentInChildren<Image>().sprite = _listOfEntityManager[0].GetSprit();

    }
}
