using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    public GroupManager _groupManager = new GroupManager();

    public GroupManager _selected = new GroupManager();


    private string _ennemieTag;

    
    void Start()
    {
    }

    public int getNumberOnGroup() { return _groupManager.getNumberOnGroup(); }

    public void SetEnnemieTag(string tag)
    {
        _groupManager.SetEnnemieTag(tag);
        _selected.SetAllieTag(tag);
        _ennemieTag = tag;
    }

    public void SetAllieTag(string tag)
    {
        _selected.SetEnnemieTag(tag);
        _groupManager.SetAllieTag(tag);
    }

    public bool getAddingMoreThanOne()
    {
        return _groupManager.getAddingMoreThanOne();
    }

    public void setAddingMoreThanOne(bool val)
    {
        _groupManager.setAddingMoreThanOne(val);
    }
    public List<EntityController> GetSelectedObject()
    {
        return _groupManager.GetSelectedObject();
    }

    public void ClearList()
    {
        
            _selected.ClearList();
       
            _groupManager.ClearList();
        
    }

    public void AddSelect(EntityManager toAdd)
    {
        if (toAdd.gameObject.CompareTag(_groupManager.GetAllieTag()))
        {
            _groupManager.AddSelect(toAdd);
        }
        else
        {
            ClearList();
            _selected.AddSelect(toAdd);
        }

    }

    public void ActionGroup(RaycastHit hit)
    {
        _groupManager.ActionGroup(hit);
    }

    private void MooveSelected(RaycastHit hit)
    {
        _groupManager.MooveSelected(hit.point);
    }

    private void AttackSelected(RaycastHit hit)
    {
        _groupManager.AttackSelected(hit);
    }

    public void AddTarget(EntityManager controller)
    {
        _groupManager.AddTarget(controller);
    }

    public void DoABuild(int nb, RaycastHit hit)
    {
        _groupManager.DoABuild(nb, hit);
    }

    private void FollowSelected(RaycastHit hit)
    {
        _groupManager.FollowSelected(hit);
    }

    public void ResetOrder()
    {
        _groupManager.ResetOrder();
    }

    public void PatrouilleOrder(Vector3 point)
    {
        _groupManager.PatrouilleOrder(point);
    }

    public void AttackingOnTravel(Vector3 point)
    {
        _groupManager.AttackingOnTravel(point);
    }

    public void TenirPositionOrder()
    {
        _groupManager.TenirPositionOrder();
    }

    public List<EntityController> getSelectList()
    {
        return _groupManager.getSelectList();
    }
}

