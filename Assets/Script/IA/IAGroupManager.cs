using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IAGroupManager
{
    private int TailleDuGroupe = 3;

    private List<GroupManager> _ListOfGroup = new List<GroupManager>();
    private List<GroupManager> _ListOfGroupAttack = new List<GroupManager>();

    private Dictionary<GroupManager, BuildingIA> _ListOfGroupToSpawnEntity = new Dictionary<GroupManager, BuildingIA>();
    private Dictionary<GroupManager, BuildingIA> _ListOfGroupToProtectBuilding = new Dictionary<GroupManager, BuildingIA>();
    private Dictionary<GroupManager, List<BuildingIA>> _ListOfGroupPatrol = new Dictionary<GroupManager, List<BuildingIA>>();

    private List<BuilderController> _ListOfBuilder = new List<BuilderController>();

    public int nbGroup;

    public IABrain ia;

    public float DistanceOfSecurity = 3f;
   
    private void DebugGroup()
    {
        nbGroup = _ListOfGroupAttack.Count;
        foreach (var group in _ListOfGroupAttack) { Debug.Log(group.getNumberOnGroup()); }
        Debug.Log("---------------");
    }
    public void CreateGroup(AggressifEntityManager entity)
    {
        _ListOfGroupAttack.Add(new GroupManager());

        GroupManager groupToCreate = _ListOfGroupAttack.Last();

        groupToCreate.SetAllieTag(ia.gameObject.tag);
        groupToCreate.SetEnnemieTag(ia.ennemieTag);
        groupToCreate.GroupIsDeadevent.AddListener(RemoveAGroup);
        groupToCreate.SomeoneIsImmobile.AddListener(ia.ActualiseTheGroup);

        groupToCreate.AddSelect(entity);
        _ListOfGroup.Add(groupToCreate);

        if(ia.GetAllieBuilding().Count>0)
        {
            groupToCreate.AttackingOnTravel(GetPosWithSecurity(groupToCreate, ia.GetAllieBuilding()[0].building.transform.position));
        }

        nbGroup++;
    }

    public void CreateSpawnEntityGroup(AggressifEntityManager entity, BuildingIA building)
    {
        GroupManager groupToCreate = new GroupManager();
        groupToCreate.SetAllieTag(ia.gameObject.tag);
        groupToCreate.SetEnnemieTag(ia.ennemieTag);

        _ListOfGroupToSpawnEntity.Add(groupToCreate, building);

        groupToCreate.GroupIsDeadevent.AddListener(RemoveAGroup);

        groupToCreate.AddSelect(entity);

        groupToCreate.AttackingOnTravel(building.building.transform.position);

        _ListOfGroup.Add(groupToCreate);
        nbGroup++;
    }
    private bool BuildingHaveAlreadyAGroup(BuildingIA building)
    {
        foreach (GroupManager group in _ListOfGroupToSpawnEntity.Keys)
        {
            if (_ListOfGroupToSpawnEntity[group] == building) { return true; }
        }
        return false;
    }
    private bool EntityIsInSpawnGroup(AggressifEntityManager entity)
    {
        foreach(GroupManager group in _ListOfGroupToSpawnEntity.Keys)
        {
            if (group.EntityIsInGroup(entity.gameObject.GetComponent<EntityController>())) { return true;}
        }
        return false;
    }
    public void CreateProtectBuildingGroup(GroupManager group, BuildingIA building)
    {
        _ListOfGroupToProtectBuilding.Add(group,building);

        group.SomeoneIsImmobile.RemoveListener(ia.ActualiseTheGroup);

        _ListOfGroupAttack.Remove(group);

    }
    public void AddBuilder(BuilderController builder)
    {
        if (!_ListOfBuilder.Contains(builder))
        {
            _ListOfBuilder.Add(builder);
            builder.NoMoreToHarvest.AddListener(SendBuilderToHarvest);
        }
        if (builder._listForOrder.Count == 0)
        {
            SendBuilderToHarvest(builder);
        }
    }
    private void ClearUselessGroup()
    {
        List<GroupManager> groupList = new List<GroupManager>();
        foreach (GroupManager group in _ListOfGroup)
        {
            if (group.getNumberOnGroup() == 0) { groupList.Add(group);  }
        }
        foreach(GroupManager group in groupList)
        {
            RemoveAGroup(group);
        }
    }

    public void AddEntityToGroup(GroupManager group, EntityController entity)
    {
        entity.ClearAllOrder();
        entity.AddAggressivePath(group.getCenterofGroup());
        group.AddSelect(entity.gameObject.GetComponent<AggressifEntityManager>());
        if(_ListOfGroupPatrol.Keys.Contains(group))
        {
            entity.GetComponent<EntityController>().AddPatrol(_ListOfGroupPatrol[group][0].building.transform.position);
            entity.GetComponent<EntityController>().AddPatrol(_ListOfGroupPatrol[group][1].building.transform.position);
        }
        else {  SendEverybodyToTheCenter(group);}

    }

    public void RemoveAGroup(GroupManager groupToRemove)
    {
        if(_ListOfGroupAttack.Contains(groupToRemove)) { _ListOfGroupAttack.Remove(groupToRemove); }
        else if (_ListOfGroupToProtectBuilding.Keys.Contains(groupToRemove)) { _ListOfGroupToProtectBuilding.Remove(groupToRemove); }
        else if (_ListOfGroupToSpawnEntity.Keys.Contains(groupToRemove)) { _ListOfGroupToSpawnEntity.Remove(groupToRemove); }
        else if (_ListOfGroupPatrol.Keys.Contains(groupToRemove)) { _ListOfGroupPatrol.Remove(groupToRemove); }

        _ListOfGroup.Remove(groupToRemove);
        nbGroup--;
    }

    public void ClearListOfPatrol()
    {
        List<GroupManager> groupPatrol = new List<GroupManager> ();
        List<GroupManager> groupPatrolToParkour = new (_ListOfGroupPatrol.Keys);
        foreach (GroupManager i in groupPatrolToParkour)
        {
            if (!ia.GetAllieBuilding().Contains(_ListOfGroupPatrol[i][0]) || !ia.GetAllieBuilding().Contains(_ListOfGroupPatrol[i][1]))
            {
                i.ResetOrder();
                groupPatrol.Add(i);
            }
        }
        foreach(GroupManager i in groupPatrol)
        {
            _ListOfGroupPatrol.Remove(i);
            _ListOfGroupAttack.Add(i);
        }
    }

    public bool BuildingIsProtected(BuildingIA building)
    {
        foreach (GroupManager i in _ListOfGroupToProtectBuilding.Keys)
        {
            if (_ListOfGroupToProtectBuilding[i] == building)
            {
                return true;
            }
        }
        return false;
    }
    public void ClearListOfProtector()
    {
        foreach (GroupManager i in _ListOfGroupToProtectBuilding.Keys)
        {
            if (_ListOfGroupToProtectBuilding[i].TagOfEntity != i.GetAllieTag())
            {
                i.ResetOrder();
                _ListOfGroupAttack.Add(i);
            }
        }
        _ListOfGroupToProtectBuilding.Clear();
    }
    public GroupManager GetThenearsetGroupOfAPoint(Vector3 point)
    {
        GroupManager ThenearsetEntity = null;

        foreach (GroupManager Thenearset in _ListOfGroupAttack)
        {
            if (ThenearsetEntity == null) { ThenearsetEntity = Thenearset; }

            if (Vector3.Distance(point, ThenearsetEntity.getCenterofGroup()) > Vector3.Distance(point, Thenearset.getCenterofGroup()))
            {
                ThenearsetEntity = Thenearset;
            }
            
        }

        return ThenearsetEntity;
    }

    public BuilderController GetThenearsetBuildeurOfAPoint(Vector3 point)
    {
        BuilderController ThenearsetEntity = null;

        foreach (BuilderController Thenearset in _ListOfBuilder)
        {
            if (ThenearsetEntity == null) { ThenearsetEntity = Thenearset; }

            if (Vector3.Distance(point, ThenearsetEntity.transform.position) > Vector3.Distance(point, Thenearset.transform.position))
            {
                ThenearsetEntity = Thenearset;
            }
        }

        return ThenearsetEntity;
    }

    public void SendBuilderToHarvest(BuilderController builder)
    {
        if(ia.GetThenearsetHarvestOfABuilder(builder) != null) {  SendBuilderToNextHarvest(builder, ia.GetThenearsetHarvestOfABuilder(builder).gameObject); }
        
    }
    public void SendBuilderToNextHarvest(BuilderController builder, GameObject nextHarvest)
    {
        if (nextHarvest)
        {
            builder.AddHarvestTarget(nextHarvest);
        }
    }
    public bool SendBuilderToBuildTower(BuildingIA building,Vector3 position)
    {
        if(_ListOfBuilder.Count >= 1)
        {
            BuilderController buildeur = GetThenearsetBuildeurOfAPoint(position);
            if(buildeur && buildeur.DoAbuild(0, position, ia.gameObject.GetComponent<RessourceController>()))
            {
                buildeur.TowerIsBuild.AddListener((BuilderController builder, DefenseManager defense) => TowerIsNowBUild(builder,defense, building));
                return true;
            }
        }
        return false;
    }

    private void TowerIsNowBUild(BuilderController builder, DefenseManager defense, BuildingIA building)
    {
        ia.TowerToBuilding(defense, building);
    }
    private bool EverybodyIsImmobile(GroupManager group)
    {
        bool Immobile = true;
        foreach (EntityController entity in group.getSelectList())
        {
            if (entity._listForOrder.Count > 0)
            {
                Immobile = false;
            }
        }
        return Immobile;
    }
    public void GroupToAttack(GroupManager group, GameObject objectif)
    {
        if (group.getNumberOnGroup() >= TailleDuGroupe && EverybodyIsImmobile(group))
        {
            group.ResetOrder();
            SendToAttack(group, objectif); 
        }
    }
    public void SendEverybodyToTheCenter(GroupManager group)
    {
        foreach (EntityController entity in group.getSelectList())
        {
            entity.ClearAllOrder();
            entity.AddAggressivePath(group.getCenterofGroup());
        }
    }
    public void SendRenfortToBuilding(BuildingIA building, Vector3 location)
    {
        GroupManager group = GetThenearsetGroupOfAPoint(location);
        if (group != null && building.IsProtected==false)
        {
            CreateProtectBuildingGroup(group, building);

            Vector3 pos = GetPosWithSecurity(group, location);

            group.ResetOrder();
            SendAGroup(group, pos);

            building.SetAProtectionGroup(group);
        }
    }

    private Vector3 GetPosWithSecurity(GroupManager group, Vector3 location)
    {
        Vector3 centerOfGroup = group.getCenterofGroup();

        float angle = Vector3.Angle(centerOfGroup, location);
        angle = Vector3.Cross(centerOfGroup, location).y > 0 ? angle : 360 - angle;

        float x = DistanceOfSecurity * Mathf.Cos((float)angle);
        float y = DistanceOfSecurity * Mathf.Sin((float)angle);

        Vector3 pos = new Vector3(x, 1, y);

        pos.x += centerOfGroup.x;
        pos.z += centerOfGroup.z;

        return pos;
    }
    private void SendToAttack(GroupManager group, GameObject objectif) { group.AttackingOnTravel(objectif.transform.position); }
    public void SendAGroup(GroupManager group, Vector3 point)
    { 
        group.AttackingOnTravel(point);
    }

    public void SendEntityToBuilding(BuildingIA building, Vector3 point, EntityController entity)
    {
        if(!BuildingHaveAlreadyAGroup(building))
        {
            foreach (GroupManager group in _ListOfGroup)
            {
                if (group.GroupContainUnity(entity))
                {
                    group.RemoveSelect(entity.gameObject.GetComponent<AggressifEntityManager>());
                }
            }
            CreateSpawnEntityGroup(entity.GetComponent<AggressifEntityManager>(), building);
            ClearUselessGroup();

        }
        else
        {
            foreach (GroupManager group in _ListOfGroupToSpawnEntity.Keys)
            {
                if (_ListOfGroupToSpawnEntity[group] == building) 
                {
                    group.AttackingOnTravel(building.building.transform.position);
                    break;
                }
            }
        }
        
        //DebugGroup();
    }

    public bool EntityIsInAGroup(EntityController entity)
    {
        bool InGroup = false;
        foreach (GroupManager group in _ListOfGroupAttack)
        {
            if (group.GroupContainUnity(entity)) { InGroup = true; }
        }
        foreach (GroupManager group in _ListOfGroupToProtectBuilding.Keys)
        {
            if (group.GroupContainUnity(entity)) { InGroup = true; }
        }
        foreach (GroupManager group in _ListOfGroupPatrol.Keys)
        {
            if (group.GroupContainUnity(entity)) { InGroup = true; }
        }
        foreach (GroupManager group in _ListOfGroupToSpawnEntity.Keys)
        {
            if (group.GroupContainUnity(entity)) { InGroup = true; }
        }
        return InGroup;
    }

    public GroupManager WhatGroupWillJoin(EntityController entity, List<GroupManager> list)
    {
        GroupManager groupeARejoindre = null;
        foreach (GroupManager group in list)
        {
            if (group.getNumberOnGroup() < TailleDuGroupe)
            {
                if (groupeARejoindre == null) { groupeARejoindre = group; }
                else
                {
                    if (Vector3.Distance(groupeARejoindre.getCenterofGroup(), entity.gameObject.transform.position) > Vector3.Distance(group.getCenterofGroup(), entity.gameObject.transform.position))
                    {
                        groupeARejoindre = group;
                    }
                }
            }
        }
        return groupeARejoindre;
    }
    public GroupManager NearestGroup(EntityController entity,GroupManager group, GroupManager group2)
    {
        if (group2 != null)
        {
            if(group != null)
            {
                if (Vector3.Distance(group.getCenterofGroup(), entity.gameObject.transform.position) > Vector3.Distance(group2.getCenterofGroup(), entity.gameObject.transform.position))
                {
                    return group2;
                }
            }
            else{ return group2; }
           
        }
        return group;
    }
    public void EntityJoinAGroup(EntityController entity)
    {
        GroupManager groupeARejoindre = null;

        GroupManager c = WhatGroupWillJoin(entity, _ListOfGroupPatrol.Keys.ToList());
        groupeARejoindre = NearestGroup(entity, groupeARejoindre, c);

        c = WhatGroupWillJoin(entity, _ListOfGroupToProtectBuilding.Keys.ToList());
        groupeARejoindre = NearestGroup(entity, groupeARejoindre, c);

        c = WhatGroupWillJoin(entity, _ListOfGroupAttack);
        groupeARejoindre = NearestGroup(entity, groupeARejoindre, c);

        if (groupeARejoindre != null) { AddEntityToGroup(groupeARejoindre, entity); }
        else { CreateGroup(entity.gameObject.GetComponent<AggressifEntityManager>()); }
        ClearUselessGroup();
    }

    public void SendAGroupToPatrol(Vector3 position, List<BuildingIA> buildings)
    {
        bool exist = false;
        GroupManager NearestGroup = GetThenearsetGroupOfAPoint(position);
        if (NearestGroup != null)
        {
            foreach (GroupManager x in _ListOfGroupPatrol.Keys)
            {
                if (_ListOfGroupPatrol[x].SequenceEqual(buildings) || x == NearestGroup){  exist = true; }
            }
            if (!exist)
            {
                _ListOfGroupAttack.Remove(NearestGroup);
                _ListOfGroupPatrol.Add(NearestGroup, buildings);
                NearestGroup.SomeoneIsImmobile.RemoveListener(ia.ActualiseTheGroup);
                NearestGroup.SpecificPatrouilleOrder(buildings[0].building.transform.position, buildings[1].building.transform.position);
            }
        }
    }
}
