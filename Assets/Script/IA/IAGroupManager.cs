using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IAGroupManager
{
    private int TailleDuGroupe = 3;

    private List<GroupManager> _ListOfGroup = new List<GroupManager>();
    private List<GroupManager> _ListOfGroupToSpawnEntity = new List<GroupManager>();
    private List<GroupManager> _ListOfGroupToProtectBuilding = new List<GroupManager>();
    private Dictionary<GroupManager, List<BuildingController>> _ListOfGroupPatrol = new Dictionary<GroupManager, List<BuildingController>>();
    private List<BuilderController> _ListOfBuilder = new List<BuilderController>();

    public int nbGroup;

    public IABrain ia;

    private float DistanceOfSecurity = 3f;
    public GroupManager GetThenearsetGroupOfAPoint(Vector3 point)
    {
        GroupManager ThenearsetEntity = null;

        foreach (GroupManager Thenearset in _ListOfGroup)
        {
            if (!_ListOfGroupToSpawnEntity.Contains(Thenearset) && !_ListOfGroupToProtectBuilding.Contains(Thenearset) && !_ListOfGroupPatrol.Keys.Contains(Thenearset))
            {
                if (ThenearsetEntity == null) { ThenearsetEntity = Thenearset; }

                if (Vector3.Distance(point, ThenearsetEntity.getCenterofGroup()) > Vector3.Distance(point, Thenearset.getCenterofGroup()))
                {
                    ThenearsetEntity = Thenearset;
                }
            }
        }

        return ThenearsetEntity;
    }
    private void DebugGroup()
    {
        nbGroup = _ListOfGroup.Count;
        foreach (var group in _ListOfGroup) { Debug.Log(group.getNumberOnGroup()); }
        Debug.Log("---------------");
    }
    public void Creategroup(AggressifEntityManager entity)
    {
        GroupManager groupToCreate = new GroupManager();
        groupToCreate.SetAllieTag(ia.gameObject.tag);
        groupToCreate.SetEnnemieTag(ia.ennemieTag);
        _ListOfGroup.Add(groupToCreate);

        groupToCreate.GroupIsDeadevent.AddListener(RemoveAGroup);
        groupToCreate.SomeoneIsImmobile.AddListener(ia.ActualiseTheGroup);

        groupToCreate.AddSelect(entity);
        entity.gameObject.GetComponent<EntityController>().GroupNumber = _ListOfGroup.IndexOf(groupToCreate);

    }

    private void ClearUselessGroup()
    {
        List<GroupManager> groupManagers = new List<GroupManager>(_ListOfGroup);
        foreach (GroupManager group in groupManagers)
        {
            if (group.getNumberOnGroup() == 0) { RemoveAGroup(group); }
        }
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
            SendBuilderToNextHarvest(builder,ia.GetThenearsetHarvestOfABuilder(builder).gameObject);
        }
    }
    public void AddEntityToGroup(GroupManager group, EntityController entity)
    {
        entity.ClearAllOrder();
        entity.AddPath(group.getCenterofGroup());
        group.AddSelect(entity.gameObject.GetComponent<AggressifEntityManager>());

        entity.GroupNumber = _ListOfGroup.IndexOf(group);
    }

    public void RemoveAGroup(GroupManager groupToRemove)
    {
        if (_ListOfGroupToProtectBuilding.Contains(groupToRemove)) { _ListOfGroupToProtectBuilding.Remove(groupToRemove); }
        if (_ListOfGroupToSpawnEntity.Contains(groupToRemove)) { _ListOfGroupToSpawnEntity.Remove(groupToRemove); }
        if (_ListOfGroupPatrol.Keys.Contains(groupToRemove)) { _ListOfGroupPatrol.Remove(groupToRemove); }
        _ListOfGroup.Remove(groupToRemove);
    }

    public void ClearListOfPatrol()
    {
        foreach (GroupManager i in _ListOfGroupPatrol.Keys)
        {
            if (!ia._AllieBuilding.Contains(_ListOfGroupPatrol[i][0]) || !ia._AllieBuilding.Contains(_ListOfGroupPatrol[i][1]))
            {
                i.ResetOrder();
                _ListOfGroupPatrol.Remove(i);
            }
        }
    }

    public void SendBuilderToHarvest(BuilderController builder)
    {
        SendBuilderToNextHarvest(builder,ia.GetThenearsetHarvestOfABuilder(builder).gameObject);
    }
    public void SendBuilderToNextHarvest(BuilderController builder, GameObject nextHarvest)
    {
        if (nextHarvest)
        {
            builder.AddHarvestTarget(nextHarvest);
        }
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
            entity.AddPath(group.getCenterofGroup());
        }
    }


    public void SendRenfortToBuilding(BuildingIA building, Vector3 location)
    {
        GroupManager group = GetThenearsetGroupOfAPoint(location);
        if (group != null)
        {
            _ListOfGroupToProtectBuilding.Add(group);

            Vector3 centerOfGroup = group.getCenterofGroup();

            float angle = Vector3.Angle(centerOfGroup, location);
            angle = Vector3.Cross(centerOfGroup, location).y > 0 ? angle : 360 - angle;

            float x = DistanceOfSecurity * Mathf.Cos((float)angle);
            float y = DistanceOfSecurity * Mathf.Sin((float)angle);

            Vector3 pos = new Vector3(x, 1, y);

            pos.x += centerOfGroup.x;
            pos.z += centerOfGroup.z;

            group.ResetOrder();
            SendAGroup(group, pos);

            building.SetAProtectionGroup(group);
        }
    }
    private void SendToAttack(GroupManager group, GameObject objectif) { group.AttackingOnTravel(objectif.transform.position); }
    public void SendAGroup(GroupManager group, Vector3 point) { group.MooveSelected(point); }

    public void SendEntityToBuilding(BuildingIA building, Vector3 point, EntityController entity)
    {
        entity.AddPath(point);
        foreach (GroupManager group in _ListOfGroup)
        {
            if (group.GroupContainUnity(entity))
            {
                group.RemoveSelect(entity.gameObject.GetComponent<AggressifEntityManager>());
            }
        }
        Creategroup(entity.GetComponent<AggressifEntityManager>());
        ClearUselessGroup();
        _ListOfGroupToSpawnEntity.Add(_ListOfGroup.Last());

        //DebugGroup();
    }

    public bool EntityIsInAGroup(EntityController entity)
    {
        bool InGroup = false;
        foreach (GroupManager group in _ListOfGroup)
        {
            if (group.GroupContainUnity(entity)) { InGroup = true; }
        }
        return InGroup;
    }

    public void EntityJoinAGroup(EntityController entity)
    {
        GroupManager groupeARejoindre = null;
        foreach (GroupManager group in _ListOfGroup)
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
        if (groupeARejoindre != null) { AddEntityToGroup(groupeARejoindre, entity); }
        else { Creategroup(entity.gameObject.GetComponent<AggressifEntityManager>()); }
        ClearUselessGroup();
    }

    public void SendAGroupToPatrol(Vector3 position, List<BuildingController> buildings)
    {
        bool exist = false;
        GroupManager NearestGroup = GetThenearsetGroupOfAPoint(position);
        if (NearestGroup != null)
        {

            foreach (GroupManager x in _ListOfGroupPatrol.Keys)
            {
                if (_ListOfGroupPatrol[x] == buildings || x == NearestGroup)
                {
                    exist = true;
                }
            }
            if (!exist)
            {
                _ListOfGroupPatrol.Add(NearestGroup, buildings);
                NearestGroup.SpecificPatrouilleOrder(buildings[0].transform.position, buildings[1].transform.position);
            }
        }
    }
}
