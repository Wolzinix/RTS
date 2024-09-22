using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    
    private List<EntityController> _selectedObject;
    
    private bool _addingMoreThanOne;

    private EntityManager _selected;

    private Vector3 _CenterOfGroup;

    private string _ennemieTag;
    private string _allieTag;

    
    void Start()
    {
        _selectedObject = new List<EntityController>();
        _CenterOfGroup = new Vector3();
    }

    public int getNumberOnGroup() { return _selectedObject.Count; }

    public void SetEnnemieTag(string tag){  _ennemieTag = tag; }

    public void SetAllieTag(string tag)
    {
        _allieTag = tag;
    }

    private void getCenterofGroup()
    {
        _CenterOfGroup = new Vector3();
        foreach(EntityController controller in _selectedObject)
        {
            _CenterOfGroup += controller.gameObject.transform.position;
        }
        _CenterOfGroup /= _selectedObject.Count;
    }

    public bool getAddingMoreThanOne()
    {
        return _addingMoreThanOne;
    }

    public void setAddingMoreThanOne(bool val)
    {
        _addingMoreThanOne = val;
    }
    public List<EntityController> GetSelectedObject()
    {
        return _selectedObject;
    }

    public void ClearList()
    {
        if (!SelectedObjectIsEmpty())
        {
            VerifyIfEveryBodyIsAlive();
            foreach (var i in _selectedObject)
            {
                i.gameObject.GetComponent<EntityManager>().OnDeselected();;
            }
            _selectedObject.Clear();
        }
        
        if (_selected)
        {
            _selected.OnDeselected();;
        }
    }

    public void AddSelect(EntityManager toAdd)
    {
        if (toAdd.gameObject.CompareTag(_allieTag) && toAdd.gameObject.GetComponent<EntityController>())
        {
            if (_selectedObject.IndexOf(toAdd.gameObject.GetComponent<EntityController>()) > -1)
            {
                _selectedObject.RemoveAt(_selectedObject.IndexOf(toAdd.gameObject.GetComponent<EntityController>()));
                toAdd.gameObject.GetComponent<EntityManager>().OnDeselected();
            }
            else
            {
                _selectedObject.Add(toAdd.gameObject.GetComponent<EntityController>());
                toAdd.gameObject.GetComponent<EntityManager>().OnSelected();
            }
        }
        else
        {
            ClearList();
            if (_selected && _selected != toAdd)
            {
                _selected.OnDeselected();
            }
            _selected = toAdd;
            toAdd.OnSelected();
        }
    }

    public void ActionGroup(RaycastHit hit)
    {
        if (hit.transform)
        {
            if (hit.transform.gameObject.CompareTag(_ennemieTag)) { AttackSelected(hit); }

            else if (hit.transform.gameObject.CompareTag(_allieTag)) { FollowSelected(hit); }
            else { MooveSelected(hit); }
        }
    }

    private void MooveSelected(RaycastHit hit)
    {
        if(!SelectedObjectIsEmpty())
        {
            VerifyIfEveryBodyIsAlive();
            getCenterofGroup();
            foreach (EntityController i in _selectedObject)
            {
                Vector3 _PointToReach = _CenterOfGroup - i.transform.position;
                i.GetComponent<EntityController>().AddPath(hit.point - _PointToReach);
                i.Stay = false;
            }
        }
    }

    private void VerifyIfEveryBodyIsAlive()
    {

        List<int> indexToRemove = new List<int>();
        foreach (EntityController i in _selectedObject)
        {
            if (!i)
            {
                indexToRemove.Add(_selectedObject.IndexOf(i));
            }
        }

        indexToRemove.Reverse();
        foreach (int i in indexToRemove)
        {
            _selectedObject.RemoveAt(i);
        }

    }

    private void AttackSelected(RaycastHit hit)
    {
        if (!SelectedObjectIsEmpty())
        {
            VerifyIfEveryBodyIsAlive();
            foreach (EntityController i in _selectedObject)
            {
                i.GetComponent<EntityController>().AddTarget(hit.transform.gameObject.GetComponent<EntityManager>());
                i.Stay = false;
            }
        }
    }

    public void AddTarget(EntityManager controller)
    {
        if (!SelectedObjectIsEmpty())
        {
            VerifyIfEveryBodyIsAlive();
            foreach (EntityController i in _selectedObject)
            {
                if(i.gameObject.GetComponent<EntityManager>() != controller)
                {
                    i.GetComponent<EntityController>().AddTarget(controller);
                    i.Stay = false;
                }
            }
        }
    }

    public void DoABuild(int nb, RaycastHit hit)
    {
        if (!SelectedObjectIsEmpty())
        {
            VerifyIfEveryBodyIsAlive();
            foreach (EntityController i in _selectedObject)
            {
                if (i.gameObject.GetComponent<BuilderManager>())
                {
                    i.gameObject.GetComponent<BuilderManager>().DoAbuild(nb, hit);
                }
            }
        }
    }

    private void FollowSelected(RaycastHit hit)
    {
        if (!SelectedObjectIsEmpty())
        {
            VerifyIfEveryBodyIsAlive();
            foreach (EntityController i in _selectedObject)
            {
                i.GetComponent<EntityController>().AddAllie(hit.transform.gameObject.GetComponent<EntityManager>());
                i.Stay = false;
            }
        }
    }

    public void ResetOrder()
    {
        if (!SelectedObjectIsEmpty())
        {
            VerifyIfEveryBodyIsAlive();
            foreach (EntityController i in _selectedObject)
            {
                if (i){ i.ClearAllOrder();}
            }
        }
    }

    public void PatrouilleOrder(Vector3 point)
    {
        if (!SelectedObjectIsEmpty())
        {
            VerifyIfEveryBodyIsAlive();
            foreach (EntityController i in _selectedObject)
            {
                if (!_addingMoreThanOne)
                {
                    i.GetComponent<EntityController>().AddPatrol(i.gameObject.transform.position);
                }
                i.GetComponent<EntityController>().AddPatrol(point);
                i.Stay = false;
            }
        }
    }

    private bool SelectedObjectIsEmpty()
    {
        return _selectedObject.Count <= 0;
    }

    public void AttackingOnTravel(Vector3 point)
    {
        if (!SelectedObjectIsEmpty())
        {
            VerifyIfEveryBodyIsAlive();
            foreach (EntityController i in _selectedObject)
            {
                i.GetComponent<EntityController>().AddAggressivePath(point);
                i.Stay = false;
            }
        }
    }

    public void TenirPositionOrder()
    {
        if (!SelectedObjectIsEmpty())
        {
            VerifyIfEveryBodyIsAlive();
            ResetOrder();
            foreach (var i in _selectedObject)
            {
                i.Stay = true;
            }
        }
    }

    public List<EntityController> getSelectList()
    {
        return _selectedObject;
    }
}

