using System.Collections.Generic;
using UnityEngine;

public static class AnimationController 
{
    static readonly Dictionary<int, string> DicoOfAttackAnim = new()
    {
        { 0,"Attack1" },
        { 1,"Attack2" },
        { 2,"Attack3" },
    };

    public static string GetAttackAnimRandom()
    {
        return DicoOfAttackAnim[Random.Range(0, DicoOfAttackAnim.Keys.Count)];
    }
    public static string GetAttackAnimSpecific(int attackIndice)
    {
        return DicoOfAttackAnim[attackIndice];
    }
}
