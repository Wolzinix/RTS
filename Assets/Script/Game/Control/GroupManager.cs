using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GroupManager
{

    private List<EntityController> _selectedObject;

    private Vector3 _CenterOfGroup;

    private bool _addingMoreThanOne;

    private string _ennemieTag;

    private string _alliTag;

    public UnityEvent<GroupManager> GroupIsDeadevent = new UnityEvent<GroupManager>();

    public GroupManager()
    {
        _selectedObject = new List<EntityController>();
        _CenterOfGroup = new Vector3();
    }

    public bool getAddingMoreThanOne()  { return _addingMoreThanOne; }
    public void setAddingMoreThanOne(bool val) { _addingMoreThanOne = val;}
    public int getNumberOnGroup() { return _selectedObject.Count; }

    public void SetEnnemieTag(string tag) { _ennemieTag = tag;  }

    public void SetAllieTag(string tag) { _alliTag = tag;}


    public string GetAllieTag() { return _alliTag;}
    public Vector3 getCenterofGroup()
    {
        _CenterOfGroup = new Vector3();
        foreach (EntityController controller in _selectedObject)
        {
            _CenterOfGroup += controller.gameObject.transform.position;
        }
        _CenterOfGroup /= _selectedObject.Count;

        return _CenterOfGroup;
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
                i.gameObject.GetComponent<EntityManager>().OnDeselected(); ;
            }
            _selectedObject.Clear();
        }
    }


    public void AddSelect(EntityManager toAdd)
    {
        if (toAdd.gameObject.CompareTag(_alliTag) && toAdd.gameObject.GetComponent<EntityController>())
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
            toAdd.OnSelected();
        }
    }


    public void RemoveSelect(EntityManager toAdd)
    {
        
        _selectedObject.RemoveAt(_selectedObject.IndexOf(toAdd.gameObject.GetComponent<EntityController>()));
        toAdd.gameObject.GetComponent<EntityManager>().OnDeselected();
          
    }

    public void ActionGroup(RaycastHit hit)
    {
        if (hit.transform)
        {
            if (hit.transform.gameObject.CompareTag(_ennemieTag)) { AttackSelected(hit); }

            else if (hit.transform.gameObject.CompareTag(_alliTag)) { FollowSelected(hit); }
            else { MooveSelected(hit.point); }
        }
    }

    void OnDestroy()
    {
        GroupIsDeadevent.Invoke(this);
    }

    public void MooveSelected(Vector3 point, bool dontGoOnPoint = true)
    {
        if (!SelectedObjectIsEmpty())
        {
            VerifyIfEveryBodyIsAlive();
            getCenterofGroup();
            foreach (EntityController i in _selectedObject)
            {
                Vector3 _PointToReach = _CenterOfGroup - i.transform.position;
                if(dontGoOnPoint) {   i.GetComponent<EntityController>().AddPath(point - _PointToReach);   }
                else { i.GetComponent<EntityController>().AddPath(point); }
                
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

    public void AttackSelected(RaycastHit hit)
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
                if (i.gameObject.GetComponent<EntityManager>() != controller)
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

    public void FollowSelected(RaycastHit hit)
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
                if (i) { i.ClearAllOrder(); }
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

    public bool GroupContainUnity(EntityController entity)
    {
        return _selectedObject.Contains(entity);
    }
}
