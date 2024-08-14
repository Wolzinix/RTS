using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    
    private List<EntityController> _selectedObject;
    
    void Start()
    {
        if (FindObjectOfType<SelectManager>())
        {
            Destroy(gameObject);
        }
        _selectedObject = new List<EntityController>();
    }

    public void ClearList()
    {
        foreach (var i in _selectedObject)
        {
            i.OnDeselected();
        }
        _selectedObject.Clear();
    }

    public void AddSelect(EntityController toAdd)
    {
        _selectedObject.Add(toAdd);
        toAdd.OnSelected();
    }
    
    public void MooveSelected(RaycastHit hit)
    {
        foreach (EntityController i in _selectedObject)
        {
            i.GetComponent<EntityController>().AddPath(hit.point);
        }
    }

    public void ResetPath()
    {
        foreach (var i in _selectedObject)
        {
            i.ClearAllPath();
            i.StopPath();
        }
    }
}

