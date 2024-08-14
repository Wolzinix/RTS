using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    
    private List<EntityManager> _selectedObject;
    
    void Start()
    {
        if (FindObjectOfType<SelectManager>())
        {
            Destroy(gameObject);
        }
        _selectedObject = new List<EntityManager>();
    }

    public void ClearList()
    {
        foreach (var i in _selectedObject)
        {
            i.OnDeselected();
        }
        _selectedObject.Clear();
    }

    public void AddSelect(EntityManager toAdd)
    {
        _selectedObject.Add(toAdd);
        toAdd.OnSelected();
    }
    
    public void MooveSelected(RaycastHit hit)
    {
        foreach (EntityManager i in _selectedObject)
        {
            i.GetComponent<EntityManager>().AddPath(hit.point);
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

