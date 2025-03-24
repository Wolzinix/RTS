using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NameIndex
{
    static readonly Dictionary<int, string> DicoOfName = new()
    {
        { 0,"Jean" },
        { 1,"Michel" },
        { 2,"Nepser" },
        { 3,"Nezelithe" },
        { 4,"FireWolf_xy" },
        { 5,"Pokyton" },
        { 6,"GloK__" },
        { 7,"MyouuMahjong" },
        { 8,"lilicornette" },
        { 9,"Subbarath" },
        { 10,"Provencal" }
    };


    public static string GetAName()
    {
        return DicoOfName[Random.Range(0,DicoOfName.Count)];
    }

    public static string GetAName(int indexName)
    {
        return DicoOfName[indexName];
    }
}
