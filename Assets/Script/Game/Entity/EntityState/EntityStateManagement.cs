using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityStateManagement
{
    private List<StateClassEntity> _ListOfstate;

    readonly Type[] listOfTypeStateForSearch = { typeof(PatrolState), typeof(AggressifState), typeof(FollowState) };

    public EntityStateManagement()
    {
        _ListOfstate = new();
    }

    public int GetLenghtOfState()
    {
        return _ListOfstate.Count;
    }
    public StateClassEntity GetFirstState()
    {
        return _ListOfstate[0];
    }


    public void Update()
    {
        if (_ListOfstate.Count > 0)
        {
            _ListOfstate[0].Update();
        }
    }

    
    public void StartFirstOrder()
    {
        if (_ListOfstate.Count >= 1) { _ListOfstate[0].Start(); }
    }

    

    public void RemoveFirstOrder()
    {
        if (_ListOfstate.Count > 0)
        {
            _ListOfstate[0].Dispose();
            _ListOfstate.RemoveAt(0);

        }
        StartFirstOrder();
    }

    public void RemoveOrder(StateClassEntity type)
    {
        _ListOfstate.Remove(type);
    }
    public void ClearAllOrderOfType(Type type)
    {
        _ListOfstate.RemoveAll(x => x.GetType() == type);
    }
    public void ClearAllOrder()
    {
        while (_ListOfstate.Count > 0) { _ListOfstate[0].End(); }
    }
    public void AddPath(Vector3 newPath, EntityController entity, NavMeshController navMesh)
    {
        if (!IsInStun())
        { 
            if (navMesh && Vector3.Distance(entity.gameObject.transform.position, newPath) >= navMesh.HaveStoppingDistance() + 0.5)
            {
                _ListOfstate.Add(new MoveState(navMesh, newPath, entity));
                StartFirstOrder();
            }
        }
    }

    public void AddPathWithRange(Vector3 newPath, EntityController entity, NavMeshController navMesh)
    {
        if (!IsInStun()) 
        { 
            if (navMesh && Vector3.Distance(entity.gameObject.transform.position, newPath) >= navMesh.HaveStoppingDistance() + 0.5)
            {
                _ListOfstate.Insert(0, new MoveToDistanceState(navMesh, newPath, entity));
                StartFirstOrder();
            }
        }

    }

    public void AddPathWithRange(Vector3 newPath, float range, EntityController entity, NavMeshController navMesh)
    {
        if (!IsInStun())
        {
            if (navMesh && Vector3.Distance(entity.gameObject.transform.position, newPath) >= navMesh.HaveStoppingDistance() + 0.5 + range)
            {
                _ListOfstate.Insert(0, new MoveToDistanceState(navMesh, newPath, entity, range));
                StartFirstOrder();
            }
        }
    }

    public void AddPathInFirst(Vector3 newPath, EntityController entity, NavMeshController navMesh)
    {
        if (!IsInStun())
        {
            if (navMesh && Vector3.Distance(entity.gameObject.transform.position, newPath) >= navMesh.HaveStoppingDistance() + 0.5)
            {
                _ListOfstate.Insert(0, new MoveState(navMesh, newPath, entity));
                StartFirstOrder();
            }
        }
    }
    public void AddPatrol(Vector3 point, EntityController entity, NavMeshController navMesh)
    {
        List<Vector3> destination = new(){ point };
        if (_ListOfstate.Count >= 1 && _ListOfstate.Exists(r => r.GetType() == typeof(PatrolState)))
        {
            PatrolState patrol = (PatrolState)_ListOfstate[_ListOfstate.FindIndex(r => r.GetType() == typeof(PatrolState))];
            patrol.AddDestination(point);
        }
        else
        {
            _ListOfstate.Add(new PatrolState(destination, navMesh, entity));
            StartFirstOrder();
        }
    }

    public void AddAttackState(SelectableManager target, EntityController entity, ProjectilManager projectile)
    {
        if (!IsInStun())
        {
            if (projectile) { _ListOfstate.Insert(0, new AttackState(entity, projectile, target)); }
            else { _ListOfstate.Insert(0, new AttackState(entity, target)); }
            StartFirstOrder();
        }
    }

    public void InsertTarget(SelectableManager target, EntityController entity, NavMeshController navMesh)
    {
        if (!IsInStun())
        {
            _ListOfstate.Insert(0, new TargetState(target, entity, navMesh));
            StartFirstOrder();
        }
    }


    public void AddTarget(SelectableManager target, EntityController entity, NavMeshController navMesh)
    {
        _ListOfstate.Add(new TargetState(target, entity, navMesh));
        StartFirstOrder();
    }

    public void AddAllie(SelectableManager target, EntityController entity, NavMeshController navMesh)
    {
        _ListOfstate.Add(new FollowState(target, navMesh, entity));
        StartFirstOrder();
    }

    public void AddAggressivePath(Vector3 newPath, EntityController entity, NavMeshController navMesh)
    {
        _ListOfstate.Add(new AggressifState(navMesh, newPath, entity));
        StartFirstOrder();
    }
    public void AddStayOrder(EntityController entity, NavMeshController navMesh)
    {
        _ListOfstate.Add(new StayState(navMesh, entity));
        StartFirstOrder();
    }
    public void AddStayOrderAtFirst(EntityController entity, NavMeshController navMesh)
    {
        if (!IsInStun())
        {
            _ListOfstate.Insert(0, new StayState(navMesh, entity));
            StartFirstOrder();
        }
    }

    public void AddStuntOrder(EntityController entity, NavMeshController navMesh)
    {
        if (!IsInStun())
        {
            _ListOfstate.Insert(0, new StuntState(navMesh, entity));
            StartFirstOrder();
        }
    }
    public void AddHarvestTarget(GameObject hit, BuilderController entity)
    {
        if (!AlreadyGotThisOrder(typeof(HarvestState)))
        {
            _ListOfstate.Add(new HarvestState(entity, hit.GetComponent<RessourceManager>()));
        }
    }
    public void AddAggresseurTarget(AggressifEntityManager entityToAggresse, EntityController entity, NavMeshController navMesh)
    {
        if (navMesh && navMesh.NotOnTraject() && _ListOfstate.Count == 0 || _ListOfstate.Count != 0 && (_ListOfstate[0].GetType() == typeof(PatrolState) || _ListOfstate[0].GetType() == typeof(AggressifState)) || navMesh == null)
        {
            AddTarget(entityToAggresse, entity, navMesh);
        }
    }

    public void AddBuildState(BuilderController entity, Vector3 pos ,SelectableManager defense)
    {
        _ListOfstate.Add(new BuildState(entity, pos, defense));
    }

    public StateClassEntity SearchAState(Type type)
    {
        return _ListOfstate.Find(x => x.GetType() == type);
    }
    public void SortTarget(EntityController entity, NavMeshController navMesh)
    {
        if (!IsInStun())
        {
            TargetState nearest = (TargetState)SearchAState(typeof(TargetState));
            if (nearest != null)
            {
                foreach (StateClassEntity i in _ListOfstate)
                {
                    if (i.GetType() == typeof(TargetState))
                    {
                        TargetState c = (TargetState)i;
                        if (c.target && nearest != i)
                        {
                            if (Vector3.Distance(entity.gameObject.transform.position, nearest.target.gameObject.transform.position) > Vector3.Distance(entity.gameObject.transform.position, c.target.gameObject.transform.position))
                            {
                                nearest = (TargetState)i;
                            }
                        }
                    }
                }
                RemoveOrder(nearest);
                InsertTarget(nearest.target, entity, navMesh);
            }
        }
    }

    public bool IsInSeachState()
    {
        return _ListOfstate.Count > 0 && listOfTypeStateForSearch.Contains(_ListOfstate[0].GetType());
    }
    public bool IsInStun()
    {
        return AlreadyGotThisOrder(typeof(StuntState));
    }

    public bool AlreadyGotThisOrder(Type type)
    {
        return _ListOfstate.Count > 0 && _ListOfstate.Exists(r => r.GetType() == type); ;
    }
}
