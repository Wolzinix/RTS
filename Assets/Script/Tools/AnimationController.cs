using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public static class AnimationController 
{
    static readonly Dictionary<int, string> DicoOfAttackAnim = new()
    {
        { 0,"Attack1" },
        { 1,"Attack2" },
    };


    public static readonly int Moving = Animator.StringToHash("Mooving");
    public static readonly int Idle = Animator.StringToHash("IdleBool");

    public static string GetAttackAnimRandom()
    {
        return DicoOfAttackAnim[Random.Range(0, DicoOfAttackAnim.Keys.Count)];
    }
    public static string GetAttackAnimSpecific(int attackIndice)
    {
        return DicoOfAttackAnim[attackIndice];
    }

    public static void CancelAnimation(Animator animator)
    {
        animator.SetBool(Idle, true);
        animator.SetBool(Moving, false);
        animator.Play("Nothing");
    }
}
