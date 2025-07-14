using Assets.Script.Game;
using System.Collections.Generic;
using UnityEngine;

public class GroupeUiManager : MonoBehaviour
{
    public List<SelectableManager> _listOfEntity;
    [SerializeField] private GameObject cadre;

    private List<GameObject> _listOfCadreControllers;
    [SerializeField] private GameObject BackImageZone;

    [SerializeField] private int marge = 150;
    void Awake()
    {
        _listOfEntity = new List<SelectableManager>();
        _listOfCadreControllers = new List<GameObject>();

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        ClearList();
    }

    public void AddEntity(SelectableManager entity)
    {
        int index = _listOfEntity.IndexOf(entity);
        if (index == -1)
        {
            _listOfEntity.Add(entity);
            GameObject newCadre = Instantiate(cadre, BackImageZone.transform);
            newCadre.GetComponent<CadreController>().SetEntity(entity);
            newCadre.GetComponent<CadreController>().SetGroupUiManager(this);
            _listOfCadreControllers.Add(newCadre);
            }
        else { RemoveCadre(_listOfCadreControllers[index]); }
        SortAffichage();
        CleanAffichage();
    }

    public void RemoveEntity(SelectableManager entity)
    {
        int index = _listOfEntity.IndexOf(entity);
        if (index != -1)
        {
            Destroy(_listOfCadreControllers[index]);
            _listOfCadreControllers.RemoveAt(index);
            _listOfEntity.Remove(entity);
        }
        SortAffichage();
        CleanAffichage();
    }

    private void CleanAffichage()
    {
        if (_listOfCadreControllers.Count == 0) { gameObject.SetActive(false); }
        else
        {
            foreach (GameObject i in _listOfCadreControllers)
            {
                i.transform.localScale = cadre.transform.localScale * RecalculeSizeOfCadre();
                float newMarge = marge * RecalculeSizeOfCadre();
                Rect rect = i.GetComponent<RectTransform>().rect;
                Rect rectParent = i.transform.parent.GetComponent<RectTransform>().rect;
                int index = _listOfCadreControllers.IndexOf(i);

                i.transform.position = new Vector3(
                    (newMarge * index + rect.width / 2) - (rectParent.width * ((int)((newMarge * index + rect.width) / rectParent.width))),
                    rect.height * (0.5f + (int)((newMarge * index + rect.width) / rectParent.width)),
                    0);
            }
        }
    }

    private float RecalculeSizeOfCadre()
    {
        Rect rect = cadre.GetComponent<RectTransform>().rect;
        Rect rectParent = BackImageZone.GetComponent<RectTransform>().rect;
        float cadresAir = ((marge + rect.width) / 2) ;
        float ZoneAir = rectParent.width - (marge *2);

        float coeff = ZoneAir / (cadresAir * _listOfCadreControllers.Count);
        if (coeff < 1)
        {
            return coeff;
        }
        else
        {
            return 1;
        }
    }

    private void SortAffichage()
    {
        int i = 0;
        EntityType actualEntity = _listOfEntity[_listOfEntity.Count-1].entityType;
        while (i < _listOfEntity.Count -1)
        {   
            if (_listOfEntity[i].entityType == actualEntity)
            {
                _listOfEntity.Insert(i+1, _listOfEntity[_listOfEntity.Count - 1]);
                _listOfEntity.RemoveAt(_listOfEntity.Count - 1);

                _listOfCadreControllers.Insert(i + 1, _listOfCadreControllers[_listOfEntity.Count - 1]);
                _listOfCadreControllers.RemoveAt(_listOfCadreControllers.Count - 1);
                break;
            }
            i++;
        }
    }

    private void ClearListOfEntity()
    {
        if (_listOfEntity.Count > 0) { _listOfEntity.Clear(); }
    }

    private void ClearListOfCadre()
    {
        foreach (var i in _listOfCadreControllers)
        {
            Destroy(i);
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
        CleanAffichage();
    }
}
