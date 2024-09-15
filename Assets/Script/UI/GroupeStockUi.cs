using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroupeStockUi : MonoBehaviour
{
    [SerializeField] private GameObject groupUi;
    [SerializeField] private GameObject image;

    private List<GameObject> _listOfGroup;
    // Start is called before the first frame update
    void Start()
    {
        _listOfGroup = new List<GameObject>();
        AddEntity();
    }
    public void AddEntity()
    {
        if(_listOfGroup.Count < 6 ) 
        { 
            GameObject newGroup = Instantiate(groupUi, new Vector3(150 * _listOfGroup.Count + 50, 50, 0), transform.rotation, image.transform);
            _listOfGroup.Add(newGroup);
            SortAffichage();
        }
    }

    private void SortAffichage()
    {
        foreach (GameObject i in _listOfGroup)
        {
            i.transform.position = new Vector3(150 * _listOfGroup.IndexOf(i) + 50, 125, 0);
        }
        
    }
    public void RemoveCadre(GameObject cadreToRemove)
    {
        _listOfGroup.RemoveAt(_listOfGroup.IndexOf(cadreToRemove));
        SortAffichage();
    }
}
