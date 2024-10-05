using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XR;

public class GroupManager
{

    private List<EntityController> _selectedObject;

    private Vector3 _CenterOfGroup;

    private bool _addingMoreThanOne;

    private string _ennemieTag;

    private string _alliTag;

    public bool IsPlayer;

    public UnityEvent<GroupManager> GroupIsDeadevent = new UnityEvent<GroupManager>();

    public UnityEvent<GroupManager> SomeoneIsImmobile = new UnityEvent<GroupManager>();
     
    private SelectableManager _OneSelected; 

    public GroupManager()
    {
        _selectedObject = new List<EntityController>();
        _CenterOfGroup = new Vector3();
    }

    public bool IsMoving()
    {
        bool mooving = false;
        foreach (EntityController controller in _selectedObject)
        {
            if(!mooving )
            {
                mooving = controller.GetComponent<NavMeshController>().notAtLocation();
                break;
            }
        }
        return mooving;
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
            if(controller)
            {
                _CenterOfGroup += controller.gameObject.transform.position;
            }
            
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
                i.gameObject.GetComponent<TroupeManager>().OnDeselected(); ;
            }
            _selectedObject.Clear();
           
        }
        else if (_OneSelected)
        {
            _OneSelected.OnDeselected();
            _OneSelected = null;
        }
    }

    private void SomeOneIsImmobile()
    {
        SomeoneIsImmobile.Invoke(this);
    }


    public void AddSelect(SelectableManager toAdd)
    {
        if (toAdd.gameObject.CompareTag(_alliTag) && toAdd.gameObject.GetComponent<EntityController>())
        {
            if (_selectedObject.IndexOf(toAdd.gameObject.GetComponent<EntityController>()) > -1)
            {
                _selectedObject.RemoveAt(_selectedObject.IndexOf(toAdd.gameObject.GetComponent<EntityController>()));
                toAdd.gameObject.GetComponent<TroupeManager>().OnDeselected();
            }
            else
            {
                _selectedObject.Add(toAdd.gameObject.GetComponent<EntityController>());
                toAdd.GetComponent<EntityController>().EntityIsArrive.AddListener(SomeOneIsImmobile);
                if(IsPlayer)
                {
                    toAdd.gameObject.GetComponent<TroupeManager>().OnSelected();
                }
            }
        }
        else
        {
            ClearList();
            if(IsPlayer)
            {

                toAdd.OnSelected();
                _OneSelected = toAdd;
            }
        }
    }

    public void RemoveSelect(TroupeManager toAdd)
    {
        
        _selectedObject.RemoveAt(_selectedObject.IndexOf(toAdd.gameObject.GetComponent<EntityController>()));
        toAdd.gameObject.GetComponent<TroupeManager>().OnDeselected();
          
    }

    public void ActionGroup(RaycastHit hit)
    {
        if (hit.transform)
        {
            if (hit.transform.gameObject.CompareTag(_ennemieTag)) { AttackSelected(hit); }

            else if (hit.transform.gameObject.CompareTag(_alliTag)) { FollowSelected(hit); }
            else if (hit.transform.gameObject.GetComponent<RessourceManager>()) { GoHarvest(hit.transform.gameObject); }
            else { MooveSelected(hit.point); }
        }
    }

    public void GoHarvest(GameObject hit)
    {
        if (!SelectedObjectIsEmpty())
        {
            VerifyIfEveryBodyIsAlive();
            foreach (EntityController i in _selectedObject)
            {
                if (i.GetComponent<BuilderController>())
                {
                    i.GetComponent<BuilderController>().AddHarvestTarget(hit);
                    i.Stay = false;
                }
            }
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
                i.GetComponent<EntityController>().AddTarget(hit.transform.gameObject.GetComponent<TroupeManager>());
                i.Stay = false;
            }
        }
    }

    public void AddTarget(SelectableManager controller)
    {
        if (!SelectedObjectIsEmpty())
        {
            VerifyIfEveryBodyIsAlive();
            foreach (EntityController i in _selectedObject)
            {
                if (i.gameObject.GetComponent<TroupeManager>() != controller)
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
                if (i.gameObject.GetComponent<BuilderController>())
                {
                    i.gameObject.GetComponent<BuilderController>().DoAbuild(nb, hit);
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
                i.GetComponent<EntityController>().AddAllie(hit.transform.gameObject.GetComponent<TroupeManager>());
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
