using System.Collections.Generic;
using UnityEngine;

public class GroupUiManager : MonoBehaviour
{
    public List<EntityManager> _listOfEntity;
    [SerializeField] private GameObject cadre;

    private List<GameObject> _listOfCadreControllers;
    [SerializeField] private GameObject image;
    
    void Awake()
    {
        _listOfEntity = new List<EntityManager>();
        _listOfCadreControllers = new List<GameObject>();
        
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ClearListOfEntity();
    }

    public void AddEntity(EntityManager entity)
    {
        int index = _listOfEntity.IndexOf(entity);
        if (index == -1)
        {
            _listOfEntity.Add(entity);
            GameObject newCadre = Instantiate(cadre,new Vector3(150 * _listOfCadreControllers.Count + 50,125,0),transform.rotation,image.transform);
            CadreController newCadreController = newCadre.GetComponent<CadreController>();
            newCadreController.SetEntity(entity);
        
            _listOfCadreControllers.Add(newCadre);
        }
        else
        {
            Destroy(_listOfCadreControllers[index]);
            _listOfCadreControllers.RemoveAt(index);
            _listOfEntity.RemoveAt(index);
            SortAffichage();
        }
        
    }

    private void SortAffichage()
    {
        if (_listOfCadreControllers.Count == 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            foreach (GameObject i in _listOfCadreControllers)
            {
                i.transform.position = new Vector3(150 * _listOfCadreControllers.IndexOf(i) + 50,125,0);
            }
        }
    }

    private void ClearListOfEntity()
    {
        if (_listOfEntity.Count > 0)
        {
            _listOfEntity.Clear();
            ClearListOfCadre();
        }
    }

    private void ClearListOfCadre()
    {
        foreach (var i in _listOfCadreControllers)
        {
            Destroy(i.gameObject);
        }
        _listOfCadreControllers.Clear();
    }

    public void RemoveCadre(GameObject cadreToRemove)
    {
        int index = _listOfCadreControllers.IndexOf(cadreToRemove);
        _listOfCadreControllers.RemoveAt(index);
        _listOfEntity.RemoveAt(index);
        SortAffichage();
    }
}
