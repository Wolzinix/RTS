using System.Collections.Generic;
using UnityEngine;

public static class AnimationController 
{
    public enum AnimType
    {
        Attack,
        Idle,
        Move,
    }

    static readonly Dictionary<int, List<string>> DicoOFAnim = new()
    {
        { 0, new List<string> () {"Attack1","Attack2" } },
        { 1,new List<string> () {"Idle1","Idle2"} },
        { 2,new List<string> () {"Move" } }
    };
    static readonly Dictionary<int, int> DicoOfVar = new()
    {
        {0, Animator.StringToHash("Attacking")},
        {1, Animator.StringToHash("IdleBool") },
        {2, Animator.StringToHash("Mooving")}
    };

    public static string GetAnimRandom(int categorie)
    {
        return DicoOFAnim[categorie][Random.Range(0, DicoOFAnim[categorie].Count)];
    }
    public static string GetAnimSpecific(int categorie,int attackIndice)
    {
        return DicoOFAnim[categorie][attackIndice];
    }

    public static void CancelAnimation(Animator animator)
    {
        if(animator.isActiveAndEnabled)
        {
            foreach(int i in DicoOfVar.Keys)
            {
                animator.SetBool(DicoOfVar[i], false);
            }
            animator.SetBool(DicoOfVar[(int)AnimType.Idle], true);
            animator.Play("Nothing");
        }
    }
    public static void PlayAnimation(int categorie,Animator animator)
    {
        CancelAnimation(animator);
        animator.Play(GetAnimRandom(categorie));
        animator.SetBool(DicoOfVar[categorie], true);
    }
    public static void StopMoveAnimation(Animator animator)
    {
        CancelAnimation(animator);
    }

    public static bool IsMovingAnimation(Animator animator)
    {
        return animator.GetBool(DicoOfVar[(int)AnimType.Move]);
    }

    public static float GetAnimationStateInfo(Animator animator)
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}
