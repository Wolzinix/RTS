using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveForNextlevel : MonoBehaviour
{
    public static SaveForNextlevel Instance { get { return _instance; } }
    private static SaveForNextlevel _instance;

    public List<GameObject> _list = new List<GameObject>();
    int index = 0;

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
        index = 0;
    }
    public void ClearSave()
    {
        foreach(GameObject i in  _list)
        {
            Destroy(i);
        }
        _list.Clear();
        index = 0;
    }
    public GameObject LoadEntity()
    {
        if(_list.Count>0 && index < _list.Count)
        {
            index++;
            return _list[index -1 ];
        }
        else
        {
            ClearSave();
        }
        return null;
    }
}
