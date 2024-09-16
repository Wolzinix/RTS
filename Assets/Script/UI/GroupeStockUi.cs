using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroupeStockUi : MonoBehaviour
{
    [SerializeField] private GameObject groupUi;
    [SerializeField] private GameObject image;

    private List<GameObject> _listOfGroup;
    void Start()
    {
        _listOfGroup = new List<GameObject>();
        AddEntity();
    }
    public void AddEntity()
    {
        if(_listOfGroup.Count < 6 ) 
        {
            GameObject newGroup = Instantiate(groupUi, image.transform);

            _listOfGroup.Add(newGroup);
            SortAffichage();
        }
    }

    private void SortAffichage()
    {
        foreach (GameObject i in _listOfGroup)
        {
            i.GetComponent<RectTransform>().anchoredPosition = new Vector3(55 * _listOfGroup.IndexOf(i) + 25, 0, 0);
        }
        
    }
    public void RemoveCadre(GameObject cadreToRemove)
    {
        _listOfGroup.RemoveAt(_listOfGroup.IndexOf(cadreToRemove));
        SortAffichage();
    }
}
