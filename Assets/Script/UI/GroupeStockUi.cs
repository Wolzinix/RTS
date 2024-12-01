using System.Collections.Generic;
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
        if (_listOfGroup.Count < 6)
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
            i.GetComponent<RectTransform>().anchoredPosition = new Vector3(groupUi.GetComponent<RectTransform>().sizeDelta[0] * _listOfGroup.IndexOf(i) + (groupUi.GetComponent<RectTransform>().sizeDelta[0]/2), 0, 0);
        }

    }
    public void RemoveCadre(GameObject cadreToRemove)
    {
        _listOfGroup.RemoveAt(_listOfGroup.IndexOf(cadreToRemove));
        SortAffichage();
    }
}
