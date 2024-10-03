using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupeUiManager : MonoBehaviour
{
    public List<TroupeManager> _listOfEntity;
    [SerializeField] private GameObject cadre;

    private List<GameObject> _listOfCadreControllers;
    [SerializeField] private GameObject image;
    
    void Awake()
    {
        _listOfEntity = new List<TroupeManager>();
        _listOfCadreControllers = new List<GameObject>();
        
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ClearList();
    }

    public void AddEntity(TroupeManager entity)
    {
        int index = _listOfEntity.IndexOf(entity);
        if (index == -1)
        {
            _listOfEntity.Add(entity);
            GameObject newCadre = Instantiate(cadre,image.transform);
            newCadre.GetComponent<CadreController>().SetEntity(entity);
        
            _listOfCadreControllers.Add(newCadre);
        }
        else { RemoveCadre(_listOfCadreControllers[index]); }
        SortAffichage();
    }

    private void SortAffichage()
    {
        if (_listOfCadreControllers.Count == 0) { gameObject.SetActive(false); }
        else
        {
            foreach (GameObject i in _listOfCadreControllers)
            {
                Rect rect = i.GetComponent<RectTransform>().rect;
                Rect rectParent = i.transform.parent.GetComponent<RectTransform>().rect;
                int index = _listOfCadreControllers.IndexOf(i);
                
                i.transform.position = new Vector3(
                    (150 * index + rect.width/2) - (rectParent.width * ((int)((150 * index + rect.width) / rectParent.width))) ,
                    rect.height * (0.5f + (int)((150 * index + rect.width) / rectParent.width)),
                    0);
            }
        }
    }

    private void ClearListOfEntity()
    {
        if (_listOfEntity.Count > 0)  {_listOfEntity.Clear();}
    }

    private void ClearListOfCadre()
    {
        foreach (var i in _listOfCadreControllers)
        {
            Destroy(i.gameObject);
        }
        _listOfCadreControllers.Clear();
    }

    public void ClearList()
    {
        ClearListOfEntity();
        ClearListOfCadre();
    }

    public void RemoveCadre(GameObject cadreToRemove)
    {
        int index = _listOfCadreControllers.IndexOf(cadreToRemove);
        Destroy(cadreToRemove);
        _listOfCadreControllers.RemoveAt(index);
        _listOfEntity.RemoveAt(index);
        SortAffichage();
    }
}
