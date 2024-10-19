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
            IAbrain.AddObjectif(building.gameObject);
            NeedAGroup();
            IsProtected = false;
        }
    }

    public void NeedAGroup() {

        IAbrain.NeedToSendGroupToBuildingEvent.Invoke(this, building.transform.position); }

    public void changeHaveEntity(List<GameObject> Entity, BuildingController building)
    {
        CanSpawn = building.GetCanSpawn();

        if (CanSpawn && building.tagOfNerestEntity == IAbrain.gameObject.tag)
        {
            IAbrain.RemoveObjectif(building.gameObject);
            EntityNextTo.Clear();
            foreach (GameObject gameObject in Entity)
            {
                TagOfEntity = gameObject.tag;
                EntityNextTo.Add(gameObject);
            }
        }
        else
        {
            IAbrain.AddObjectif(building.gameObject);
            if(building.tag == IAbrain.tag || building.tag == "neutral" && !CanSpawn)
            {
                IAbrain.NeedToSendEntityToBuildingEvent.Invoke(this, building.gameObject.transform.position);
            }
        }

    }
}
