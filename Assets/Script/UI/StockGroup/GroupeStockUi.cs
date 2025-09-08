using System.Collections.Generic;
using UnityEngine;

public class GroupeStockUI : MonoBehaviour
{
    [SerializeField] private GameObject _groupUIPrefab;
    [SerializeField] private GameObject _Canvas;

    private List<GameObject> _listOfGroups;
    private RectTransform _rectTransform;
    void Start()
    {
        _listOfGroups = new List<GameObject>();
        _rectTransform = _groupUIPrefab.GetComponent<RectTransform>();
        AddEntity();
    }
    public void AddEntity()
    {
        if (_listOfGroups.Count < 6)
        {
            GameObject newGroup = Instantiate(_groupUIPrefab, _Canvas.transform);

            _listOfGroups.Add(newGroup);
            SortAffichage();
        }
    }

    private void SortAffichage()
    {
        if(!_rectTransform) { _rectTransform = _groupUIPrefab.GetComponent<RectTransform>(); }
        foreach (GameObject i in _listOfGroups)
        {
            i.GetComponent<RectTransform>().anchoredPosition = new Vector3(_rectTransform.sizeDelta[0] * _listOfGroups.IndexOf(i) + (_rectTransform.sizeDelta[0]/2), 0, 0);
        }
    }
    public void RemoveCadre(GameObject cadreToRemove)
    {
        if(_listOfGroups.IndexOf(cadreToRemove) >= 0)
        {
            _listOfGroups.RemoveAt(_listOfGroups.IndexOf(cadreToRemove));
        }
        SortAffichage();
    }
}
