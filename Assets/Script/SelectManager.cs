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
        if (toAdd.gameObject.CompareTag("Allie"))
        {
            _selectedObject.Add(toAdd);
            toAdd.OnSelected();
        }
    }

    public void ActionGroup(RaycastHit hit)
    {
        Debug.Log(hit.transform.gameObject);
        if (hit.transform.gameObject.CompareTag("ennemie"))
        {
            AttackSelected(hit);
        }
        else
        {
            MooveSelected(hit);
        }
    }
    public void MooveSelected(RaycastHit hit)
    {
        foreach (EntityController i in _selectedObject)
        {
            i.GetComponent<EntityController>().AddPath(hit.point);
        }
    }

    public void AttackSelected(RaycastHit hit)
    {
        foreach (EntityController i in _selectedObject)
        {
            i.GetComponent<EntityController>().AddTarget(hit.transform.gameObject.GetComponent<EntityManager>());
        }
    }

    public void ResetOrder()
    {
        foreach (var i in _selectedObject)
        {
            i.ClearAllFile();
            i.StopPath();
        }
    }
}

