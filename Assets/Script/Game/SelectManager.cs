using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    
    private List<EntityController> _selectedObject;
    
    private bool _addingMoreThanOne;
    
    void Start()
    {
        if (FindObjectOfType<SelectManager>()) { Destroy(gameObject); }
        
        _selectedObject = new List<EntityController>();
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
                i.OnDeselected();
            }
        }
        _selectedObject.Clear();
    }

    public void AddSelect(EntityController toAdd)
    {
        if (toAdd.gameObject.CompareTag("Allie"))
        {
            _selectedObject.Add(toAdd);
            toAdd.OnSelected();
        }
    }

    public void ActionGroup(RaycastHit hit)
    {
        if (hit.transform)
        {
            if (hit.transform.gameObject.CompareTag("ennemie")) { AttackSelected(hit); }

            if (hit.transform.gameObject.CompareTag("Allie")) { FollowSelected(hit); }
            else { MooveSelected(hit); }
        }
    }

    private void MooveSelected(RaycastHit hit)
    {
        if(!SelectedObjectIsEmpty())
        {
            VerifyIfEveryBodyIsAlive();
            foreach (EntityController i in _selectedObject)
            {
                i.GetComponent<EntityController>().AddPath(hit.point);
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
            foreach (var i in _selectedObject)
            {
                if (i)
                {
                    i.ClearAllOrder();
                    i.StopPath();
                    i.Stay = false;
                }
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
        return _selectedObject.Count < 0;
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
}

