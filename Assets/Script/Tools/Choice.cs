using UnityEngine;

public class Choice : StateMachineBehaviour
{
    [SerializeField] private string nameOfParameter;
    [SerializeField] private int NumberOfAnimation;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger(nameOfParameter, Random.Range(0, NumberOfAnimation));
    }
}
