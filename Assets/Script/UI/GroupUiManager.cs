using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupUiManager : MonoBehaviour
{
    private List<EntityManager> _listOfEntity;
    [SerializeField] private GameObject cadre;

    private List<CadreController> _listOfCadreControllers;
    [SerializeField] private GameObject _image;
    
    void Start()
    {
        _listOfEntity = new List<EntityManager>();
        _listOfCadreControllers = new List<CadreController>();
        
        gameObject.SetActive(false);
    }

    public void AddEntity(EntityManager entity)
    {
        _listOfEntity.Add(entity);
        GameObject newCadre = Instantiate(cadre,new Vector3(250 * _listOfCadreControllers.Count + 35,125,0),transform.rotation,_image.transform);
        CadreController newCadreController = newCadre.GetComponent<CadreController>();
        newCadreController.SetEntity(entity);
        
        _listOfCadreControllers.Add(newCadreController);
    }

    public void ClearListOfEntity()
    {
        _listOfEntity.Clear();
        ClearListOfCadre();
    }

    private void ClearListOfCadre()
    {
        foreach (var i in _listOfCadreControllers)
        {
            Destroy(i.gameObject);
        }
        _listOfCadreControllers.Clear();
    }
}
