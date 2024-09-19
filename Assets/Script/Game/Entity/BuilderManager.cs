using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderManager : MonoBehaviour
{
    [SerializeField] List<GameObject> _buildings;


    public List<GameObject> getBuildings()
    {
        return _buildings;
    }
}
