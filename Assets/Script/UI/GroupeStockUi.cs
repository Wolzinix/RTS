using System.Collections.Generic;
using UnityEngine;

public class GroupeStockUi : MonoBehaviour
{
    [SerializeField] private GameObject groupUi;
    [SerializeField] private GameObject image;

    private List<GameObject> _listOfGroup;
    private RectTransform _rectTransform;
    void Start()
    {
        _listOfGroup = new List<GameObject>();
        AddEntity();
        _rectTransform = groupUi.GetComponent<RectTransform>();
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
        if(!_rectTransform) { _rectTransform = groupUi.GetComponent<RectTransform>(); }
        foreach (GameObject i in _listOfGroup)
        {
            i.GetComponent<RectTransform>().anchoredPosition = new Vector3(_rectTransform.sizeDelta[0] * _listOfGroup.IndexOf(i) + (_rectTransform.sizeDelta[0]/2), 0, 0);
        }
    }
    public void RemoveCadre(GameObject cadreToRemove)
    {
        _listOfGroup.RemoveAt(_listOfGroup.IndexOf(cadreToRemove));
        SortAffichage();
    }
}
