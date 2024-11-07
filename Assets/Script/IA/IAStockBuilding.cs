using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IAStockBuilding
{
    public IABrain IAbrain;
    private Dictionary<BuildingController, BuildingIA> DicoOfBuilding = new Dictionary<BuildingController, BuildingIA>();
    public List<BuildingIA> _AllieBuilding = new List<BuildingIA>();
    public List<BuildingIA> _EnnemieBuilding = new List<BuildingIA>();

    private void AddTowerToBuilding(GameObject newObject)
    {
        foreach (BuildingController building in _AllieBuilding)
        {
            if (DicoOfBuilding[building]._ListOfTower.Count < IAbrain.nbMaxOfTower)
            {
                groupManager.SendBuilderToBuildTower(DicoOfBuilding[building], GetTheNerestPoint(newObject.transform.position, building.SpawnTower(groupManager.DistanceOfSecurity)));
            }
        }
    }


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

            if (building.CompareTag(IAbrain.ennemieTag)) {  AddAllieBuilding(stats);  }
            else if (building.CompareTag(IAbrain.ennemieTag)) { AddEnnemieBuilding(stats);  }
            else
            {
                if(_AllieBuilding.Contains(stats)) { _AllieBuilding.Remove(stats); }
                if(_EnnemieBuilding.Contains(stats)) { _EnnemieBuilding.Remove(stats); }

                if(building.tagOfNerestEntity == IAbrain.tag) { AddAllieBuilding(stats); }
                else { AddEnnemieBuilding(stats); }
            }
        }
    }

    private void AddAllieBuilding(BuildingIA building)
    {
        _AllieBuilding.Add(building);
    }

    private void AddEnnemieBuilding(BuildingIA building)
    {
        _EnnemieBuilding.Add(building);
    }
}
