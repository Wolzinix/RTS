using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingIA
{
    public IABrain IAbrain;
    public List<GameObject> EntityNextTo = new List<GameObject>();
    public string Tag;
    public bool CanSpawn;
    public string TagOfEntity;
    public BuildingController building;

    public bool IsProtected;
    public List<GroupManager> _ListOfProtector = new List<GroupManager>();
    public GroupManager _GroupOfSpawn = null;

    public int NbOfTower = 0;
    public List<DefenseManager> _ListOfTower = new List<DefenseManager>();

    public void SetAProtectionGroup(GroupManager group)
    {
        _ListOfProtector.Add(group);
        IsProtected = true;
        group.GroupIsDeadevent.AddListener(RemoveAGroup);
    }

    public void RemoveAGroup(GroupManager group)
    {
        _ListOfProtector.Remove(group);
        if (_ListOfProtector.Count == 0)
        {
            IsProtected = false;
            IAbrain.ActualiseBuilding();
            if (IAbrain.GetAllieBuilding().Contains(this)) { NeedAGroup(); }
        }
    }

    public void NeedAGroup()
    {
        IAbrain.NeedToSendGroupToBuildingEvent.Invoke(this, building.transform.position);
    }

    public void changeHaveEntity(List<GameObject> Entity, BuildingController building)
    {
        CanSpawn = building.GetCanSpawn();

        IAbrain.ActualiseBuilding();
        if (CanSpawn && building.tagOfNerestEntity == IAbrain.gameObject.tag)
        {

            EntityNextTo.Clear();
            foreach (GameObject gameObject in Entity)
            {
                if (gameObject)
                {
                    TagOfEntity = gameObject.tag;
                    EntityNextTo.Add(gameObject);
                }
            }
            IAbrain.stockBuilding.ABuildingCanSpawn.Invoke(this);
        }
        else
        {
            IAbrain.stockBuilding.ABuildingCanNotSpawn.Invoke(this);
            if (building.tag == IAbrain.tag || building.tag == "neutral" && !CanSpawn)
            {
                IAbrain.NeedToSendEntityToBuildingEvent.Invoke(this, building.gameObject.transform.position);
            }
        }
    }
    public void NeedToSendEntity()
    {
        if (!CanSpawn)
        {
            IAbrain.NeedToSendEntityToBuildingEvent.Invoke(this, building.gameObject.transform.position);
        }
    }
    public void AddTower(DefenseManager tower)
    {
        _ListOfTower.Add(tower);
        NbOfTower += 1;
        tower.deathEvent.AddListener(ATowerIsDeath);
    }

    public void ATowerIsDeath(SelectableManager tower)
    {
        DefenseManager deadTower = (DefenseManager)tower;
        _ListOfTower.Remove(deadTower);
        NbOfTower -= 1;
        IAbrain.ATowerIsDestroyEvent.Invoke(this, tower.transform.position);
    }

    public void Dispose()
    {
        foreach (DefenseManager i in _ListOfTower)
        {
            i.deathEvent.RemoveListener(ATowerIsDeath);
        }

        foreach (GroupManager i in _ListOfProtector)
        {
            i.GroupIsDeadevent.RemoveListener(RemoveAGroup);
        }

        GC.SuppressFinalize(this);
    }

    public void RemoveSpawnGroup(GroupManager group)
    {
        if (_GroupOfSpawn != null)
        {
            _GroupOfSpawn.GroupIsDeadevent.RemoveListener(RemoveSpawnGroup);
        }
        _GroupOfSpawn = null;
    }

    public void AddSpawnGroup(GroupManager group)
    {
        _GroupOfSpawn = group;
        group.GroupIsDeadevent.AddListener(RemoveSpawnGroup);
    }


}
