using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    
    private List<EntityController> _selectedObject;
    
    void Start()
    {
        if (FindObjectOfType<SelectManager>()) { Destroy(gameObject); }
        
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
        if (hit.transform)
        {
            if (hit.transform.gameObject.CompareTag("ennemie")) { AttackSelected(hit); }

            if (hit.transform.gameObject.CompareTag("Allie")) { FollowSelected(hit); }
            else { MooveSelected(hit); }
        }
    }

    private void MooveSelected(RaycastHit hit)
    {
        foreach (EntityController i in _selectedObject)
        {
            i.GetComponent<EntityController>().AddPath(hit.point);
        }
    }

    private void AttackSelected(RaycastHit hit)
    {
        foreach (EntityController i in _selectedObject)
        {
            i.GetComponent<EntityController>().AddTarget(hit.transform.gameObject.GetComponent<EntityManager>());
        }
    }

    private void FollowSelected(RaycastHit hit)
    {
        foreach (EntityController i in _selectedObject)
        {
            i.GetComponent<EntityController>().AddAllie(hit.transform.gameObject.GetComponent<EntityManager>());
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

    public void PatrouilleOrder(Vector3 point)
    {
        foreach (EntityController i in _selectedObject)
        {
            i.GetComponent<EntityController>().addPatrouille(point);
        }
    }
}

