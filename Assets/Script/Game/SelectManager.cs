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

    public bool getAddinMoreThanOne()
    {
        return _addingMoreThanOne;
    }

    public void setAddingMoreThanOne(bool reverse)
    {
        _addingMoreThanOne = reverse;
    }
    public List<EntityController> getSelectedObject()
    {
        return _selectedObject;
    }

    public void ClearList()
    {
        if (!SelectedObjectIsEmpty())
        {
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
            foreach (EntityController i in _selectedObject)
            {
                i.GetComponent<EntityController>().AddPath(hit.point);
            }
        }
        
    }

    private void AttackSelected(RaycastHit hit)
    {
        if (!SelectedObjectIsEmpty())
        {
            foreach (EntityController i in _selectedObject)
            {
                i.GetComponent<EntityController>().AddTarget(hit.transform.gameObject.GetComponent<EntityManager>());
            }
        }
    }

    private void FollowSelected(RaycastHit hit)
    {
        if (!SelectedObjectIsEmpty())
        {
            foreach (EntityController i in _selectedObject)
            {
                i.GetComponent<EntityController>().AddAllie(hit.transform.gameObject.GetComponent<EntityManager>());
            }
        }
    }

    public void ResetOrder()
    {
        if (!SelectedObjectIsEmpty())
        {
            foreach (var i in _selectedObject)
            {
                i.ClearAllFile();
                i.StopPath();
            }
        }
    }

    public void PatrouilleOrder(Vector3 point)
    {
        if (!SelectedObjectIsEmpty())
        {
            foreach (EntityController i in _selectedObject)
            {
                if (!_addingMoreThanOne)
                {
                    i.GetComponent<EntityController>().AddPatrouille(i.gameObject.transform.position);
                }
                i.GetComponent<EntityController>().AddPatrouille(point);
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
            foreach (EntityController i in _selectedObject)
            {
                i.GetComponent<EntityController>().AddAggresifPath(point);
            }
        }
    }
}

