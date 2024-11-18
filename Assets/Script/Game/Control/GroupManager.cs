using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
            if (!mooving)
            {
                mooving = controller.GetComponent<NavMeshController>().notAtLocation();
                break;
            }
        }
        return mooving;
    }

    public bool getAddingMoreThanOne() { return _addingMoreThanOne; }
    public void setAddingMoreThanOne(bool val) { _addingMoreThanOne = val; }
    public int getNumberOnGroup() { return _selectedObject.Count; }

    public void SetEnnemieTag(string tag) { _ennemieTag = tag; }

    public void SetAllieTag(string tag) { _alliTag = tag; }


    public string GetAllieTag() { return _alliTag; }
    public Vector3 getCenterofGroup()
    {
        _CenterOfGroup = new Vector3();
        foreach (EntityController controller in _selectedObject)
        {
            if (controller)
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
                i.gameObject.GetComponent<AggressifEntityManager>().OnDeselected(); ;
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

    private void ChangeSpeedWhenAdd(EntityController entity)
    {
        if (_selectedObject.Count > 1)
        {
            if (entity.GetStartSpeed() > 0)
            {
                if (entity.GetStartSpeed() > _selectedObject[0].GetSpeed() && _selectedObject[0].GetSpeed() > 0)
                {
                    entity.ChangeSpeed(_selectedObject[0].GetSpeed());
                }
                else
                {
                    _selectedObject.Reverse();
                    foreach (EntityController i in _selectedObject)
                    {
                        i.ChangeSpeed(entity.GetStartSpeed());
                    }
                }
            }
        }
        else
        {
            entity.ChangeSpeed(entity.GetStartSpeed());
        }


    }

    public void AddSelect(SelectableManager toAdd)
    {
        if (toAdd.gameObject.CompareTag(_alliTag) && toAdd.gameObject.GetComponent<EntityController>())
        {
            if (_selectedObject.IndexOf(toAdd.gameObject.GetComponent<EntityController>()) > -1)
            {
                RemoveSelect(toAdd);
            }
            else
            {
                _selectedObject.Add(toAdd.gameObject.GetComponent<EntityController>());
                ChangeSpeedWhenAdd(toAdd.gameObject.GetComponent<EntityController>());
                toAdd.gameObject.GetComponent<EntityController>().groupManager = this;
                toAdd.GetComponent<EntityController>().EntityIsArrive.AddListener(SomeOneIsImmobile);
                toAdd.deathEvent.AddListener(RemoveSelect);
                if (IsPlayer)
                {
                    toAdd.gameObject.GetComponent<AggressifEntityManager>().OnSelected();
                }
            }
        }
        else
        {
            ClearList();
            if (IsPlayer)
            {

                toAdd.OnSelected();
                _OneSelected = toAdd;
            }
        }
    }

    private void ChangeSpeedWhenRemove(EntityController entity)
    {
        if (!SelectedObjectIsEmpty())
        {
            if (entity.GetStartSpeed() < _selectedObject[0].GetSpeed())
            {
                float newSpeed = _selectedObject[0].GetStartSpeed();
                foreach (EntityController i in _selectedObject)
                {
                    if (newSpeed > i.GetStartSpeed())
                    {
                        newSpeed = i.GetStartSpeed();
                    }
                }
                foreach (EntityController i in _selectedObject)
                {
                    i.ChangeSpeed(newSpeed);
                }

            }
        }
    }
    public void RemoveSelect(SelectableManager toAdd)
    {
        if (toAdd)
        {
            ChangeSpeedWhenRemove(toAdd.GetComponent<EntityController>());
            toAdd.gameObject.GetComponent<EntityController>().groupManager = null;
            toAdd.gameObject.GetComponent<AggressifEntityManager>().OnDeselected();
        }
        int i = _selectedObject.IndexOf(toAdd.gameObject.GetComponent<EntityController>());
        if (i < _selectedObject.Count && i >= 0)
        {
            _selectedObject.RemoveAt(i);
        }
        toAdd.deathEvent.RemoveListener(RemoveSelect);
        SelectedObjectIsEmpty();
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
                if (dontGoOnPoint) { i.GetComponent<EntityController>().AddPath(point - _PointToReach); }
                else { i.GetComponent<EntityController>().AddPath(point); }

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
        SelectedObjectIsEmpty();
    }

    public void AttackSelected(RaycastHit hit)
    {
        if (!SelectedObjectIsEmpty())
        {
            VerifyIfEveryBodyIsAlive();
            foreach (EntityController i in _selectedObject)
            {
                i.GetComponent<EntityController>().AddTarget(hit.transform.gameObject.GetComponent<AggressifEntityManager>());
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
                if (i.gameObject.GetComponent<AggressifEntityManager>() != controller)
                {
                    i.GetComponent<EntityController>().AddTarget(controller);
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
                    i.gameObject.GetComponent<BuilderController>().DoAbuildWithRaycast(nb, hit);
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
            }
        }
    }

    public void SpecificPatrouilleOrder(Vector3 start, Vector3 end)
    {
        if (!SelectedObjectIsEmpty())
        {
            VerifyIfEveryBodyIsAlive();
            foreach (EntityController i in _selectedObject)
            {
                i.GetComponent<EntityController>().AddPatrol(start);
                i.GetComponent<EntityController>().AddPatrol(end);
            }
        }
    }

    private bool SelectedObjectIsEmpty()
    {
        if (_selectedObject.Count <= 0)
        {
            OnDestroy();
            return true;
        }
        else { return false; }
    }

    public void AttackingOnTravel(Vector3 point)
    {
        if (!SelectedObjectIsEmpty())
        {
            VerifyIfEveryBodyIsAlive();
            foreach (EntityController i in _selectedObject)
            {
                i.GetComponent<EntityController>().AddAggressivePath(point);
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
                i.AddStayOrder();
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

    public bool EntityIsInGroup(EntityController entity)
    {
        return _selectedObject.Contains(entity);
    }

    public bool EveryOneIsStop()
    {
        bool moving = false;
        foreach (EntityController i in _selectedObject)
        {
            if (i.moving) moving = true;
        }
        return moving;
    }
}
