using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IAStockBuilding
{
    public IABrain IAbrain;
    private Dictionary<BuildingController, BuildingIA> DicoOfBuilding = new Dictionary<BuildingController, BuildingIA>();
    public List<BuildingIA> _AllieBuilding = new List<BuildingIA>();
    public List<BuildingIA> _EnnemieBuilding = new List<BuildingIA>();
    public List<BuildingIA> _NeutralBuilding = new List<BuildingIA>();

    public List<BuildingIA> GetAllieBuilding()
    {
        return _AllieBuilding;
    }

    private BuildingIA CreateBuildingForIa(BuildingController building)
    {
        BuildingIA stats = new BuildingIA();
        stats.Tag = building.tag;
        stats.building = building;
        stats.IAbrain = IAbrain;

        building.EntityNextToEvent.AddListener(stats.changeHaveEntity);

        DicoOfBuilding[building] = stats;
        return stats;
    }
    public void ActualiseBuilding(BuildingController[] buildings)
    {
        foreach (BuildingController building in buildings)
        {
            BuildingIA stats = DicoOfBuilding.Keys.Contains(building) ? DicoOfBuilding[building] : CreateBuildingForIa(building); 

            if (building.CompareTag(IAbrain.tag)) {  AddAllieBuilding(stats);  }
            else if (building.CompareTag(IAbrain.ennemieTag)) { AddEnnemieBuilding(stats);  }
            else
            {
                if(_AllieBuilding.Contains(stats)) { _AllieBuilding.Remove(stats); }
                if(_EnnemieBuilding.Contains(stats)) { _EnnemieBuilding.Remove(stats);  IAbrain.RemoveObjectif(building.gameObject); }
                if (_NeutralBuilding.Contains(stats)) { _NeutralBuilding.Remove(stats); IAbrain.RemoveObjectif(building.gameObject); }

                if (building.tagOfNerestEntity == IAbrain.tag) { AddAllieBuilding(stats); }
                if(building.tagOfNerestEntity == "") { AddNeutralBuilding(stats); }
                else { AddEnnemieBuilding(stats); }
            }
        }
    }

    private void AddAllieBuilding(BuildingIA building)
    {
        if (!_AllieBuilding.Contains(building)) 
        { 
            _AllieBuilding.Add(building);
            building.NeedAGroup();
            building.NeedToSendEntity();
        }
    }
    private void AddNeutralBuilding(BuildingIA building)
    {
        if (!_NeutralBuilding.Contains(building))
        {
            _NeutralBuilding.Add(building);
            IAbrain.AddObjectif(building.building.gameObject);
            building.NeedAGroup();
            building.NeedToSendEntity();
        }
    }
    private void AddEnnemieBuilding(BuildingIA building)
    {
        if (!_EnnemieBuilding.Contains(building)) 
        { 
            _EnnemieBuilding.Add(building);
            IAbrain.AddObjectif(building.building.gameObject);
        }
    }

    public void AddTowerToEveryBuilding(GameObject newObject)
    {
        foreach (BuildingIA building in _AllieBuilding)
        {
            if(!building.building)
            {
                _AllieBuilding.Remove(building);
                building.Dispose();
            }
            if (DicoOfBuilding[building.building].NbOfTower < IAbrain.nbMaxOfTower)
            {
                IAbrain.AddTowerToBuilding(building, newObject);
            }
        }
    }
}
