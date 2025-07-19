using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SaveForNextlevel : MonoBehaviour
{
    public static SaveForNextlevel Instance { get { return _instance; } }
    private static SaveForNextlevel _instance;

    public List<GameObject> _list = new List<GameObject>();

    private void Awake()
    {
        if(_instance && _instance!= this)
        {
            Destroy(this.gameObject);
        }else
        {
            DontDestroyOnLoad(this);
            _instance = this;
        }
    }
    private void Start()
    {
        SaveEntity("Allie");
    }
    public void SaveEntity(String tag)
    {
        ClearSave();
        foreach (EntityController go in FindObjectsOfType<EntityController>()) 
        { 
            if(go.CompareTag(tag)) 
            {
                _list.Add(Instantiate(go.gameObject,gameObject.transform));
                _list[^1].SetActive(false);
            }
        }
    }
    public void ClearSave()
    {
        foreach(GameObject i in  _list)
        {
            Destroy(i);
        }
        _list.Clear();
    }
    public void LoadEntity()
    {
        
    }
}
