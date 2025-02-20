using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class IAStockBuilding
{
    public IABrain IAbrain;
    private Dictionary<ProductBuildingController, BuildingIA> DicoOfBuilding = new Dictionary<ProductBuildingController, BuildingIA>();
    public List<BuildingIA> _AllieBuilding = new List<BuildingIA>();
    public List<BuildingIA> _EnnemieBuilding = new List<BuildingIA>();
    public List<BuildingIA> _NeutralBuilding = new List<BuildingIA>();
    public List<BuildingIA> _SpawnableBuilding = new List<BuildingIA>();

    public UnityEvent<BuildingIA> ABuildingCanNotSpawn = new UnityEvent<BuildingIA>();
    public UnityEvent<BuildingIA> ABuildingCanSpawn = new UnityEvent<BuildingIA>();

    private GameObject MainBase;

    public IAStockBuilding(IABrain brain)
    {
        IAbrain = brain;
        ABuildingCanNotSpawn.AddListener(RemoveBuldingToSpawnable);
        ABuildingCanSpawn.AddListener(AddBuldingToSpawnable);
        MainBase = brain.MainBase;
    }
    public List<BuildingIA> GetAllieBuilding()
    {
        return _AllieBuilding;
    }

    private BuildingIA CreateBuildingForIa(ProductBuildingController building)
    {
        BuildingIA stats = new BuildingIA();
        stats.Tag = building.tag;
        stats.building = building;
        stats.IAbrain = IAbrain;
        stats.distanceFromMainBase = DistanceFromMainBase(building);

        building.EntityNextToEvent.AddListener(stats.changeHaveEntity);

        DicoOfBuilding[building] = stats;
        return stats;
    }
    public void ActualiseBuilding(ProductBuildingController[] buildings)
    {
        foreach (ProductBuildingController building in buildings)
        {
            BuildingIA stats = DicoOfBuilding.Keys.Contains(building) ? DicoOfBuilding[building] : CreateBuildingForIa(building);

            if (building.CompareTag(IAbrain.tag) && !_AllieBuilding.Contains(stats)) { AddAllieBuilding(stats); }
            else if (building.CompareTag(IAbrain.ennemieTag) && !_EnnemieBuilding.Contains(stats)) { AddEnnemieBuilding(stats); }
            else
            {
                if (_NeutralBuilding.Contains(stats)) { _NeutralBuilding.Remove(stats); IAbrain.RemoveObjectif(building.gameObject); }

                if (building.tagOfNerestEntity == IAbrain.tag) 
                { 
                    AddAllieBuilding(stats);
                    stats.NeedToSendEntity();
                }
                else if (building.tagOfNerestEntity == "") { AddNeutralBuilding(stats); }
                else { AddEnnemieBuilding(stats); }
            }
        }
    }
    private void RemoveAllieBuilding(BuildingIA building)
    {
        _AllieBuilding.Remove(building);
        IAbrain.ActualisePatrol(building);
        building.building.entityCanSpawnNow.RemoveListener(IAbrain.SpawnEntityOfBuilding);
        building.building.entityAsBeenBuy.AddListener(IAbrain.ActualiseGroup);
    }
    private void AddAllieBuilding(BuildingIA building)
    {
        if (!_AllieBuilding.Contains(building))
        {
            building.building.entityCanSpawnNow.AddListener(IAbrain.SpawnEntityOfBuilding);
            building.building.entityAsBeenBuy.AddListener(IAbrain.ActualiseGroup);
            SortByDistance(_AllieBuilding, building);
            IAbrain.ActualisePatrol(building);
            building.NeedToSendEntity();
            building.NeedAGroup();
        }
    }
    private void AddNeutralBuilding(BuildingIA building)
    {
        if (!_NeutralBuilding.Contains(building))
        {
            SortByDistance(_NeutralBuilding, building);
            IAbrain.AddObjectif(building.building.gameObject);
            building.NeedToSendEntity();
            
        }
    }
    private void AddEnnemieBuilding(BuildingIA building)
    {
        if (!_EnnemieBuilding.Contains(building))
        {
            SortByDistance(_EnnemieBuilding,building);
            IAbrain.AddObjectif(building.building.gameObject);
        }
    }

    public void AddTowerToEveryBuilding(GameObject newObject)
    {
        foreach (BuildingIA building in _AllieBuilding)
        {
            if (!building.building)
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

    private void AddBuldingToSpawnable(BuildingIA building)
    {
        if (!_SpawnableBuilding.Contains(building))
        {
            _SpawnableBuilding.Add(building);
            IAbrain.SpawnEveryEntityOfABuilding(building.building);
        }
    }

    private void RemoveBuldingToSpawnable(BuildingIA building)
    {
        if (_SpawnableBuilding.Contains(building))
        {
            _SpawnableBuilding.Remove(building);
        }
    }

    private float DistanceFromMainBase(ProductBuildingController building)
    {
        NavMeshPath navMeshPath = new NavMeshPath();
        if(NavMesh.CalculatePath(MainBase.transform.position,building.transform.position,NavMesh.AllAreas, navMeshPath))
        {
            float lengthSoFar = 0.0F;
            for (int i = 1; i < navMeshPath.corners.Length; i++)
            {
                lengthSoFar += Vector3.Distance(navMeshPath.corners[i - 1], navMeshPath.corners[i]);
            }
            return lengthSoFar;
        }
        return -1;
    }

    private void SortByDistance(List<BuildingIA> listOfBuilding, BuildingIA building)
    {
        foreach(BuildingIA i in listOfBuilding)
        {
            if(i.distanceFromMainBase > building.distanceFromMainBase)
            {
                listOfBuilding.Insert(listOfBuilding.IndexOf(i), building);
                return;
            }
        }
        listOfBuilding.Add(building);
    }
}
