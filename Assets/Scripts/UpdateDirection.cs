using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateDirection : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int lc = animator.GetInteger("LeftClick");
        int rc = animator.GetInteger("RightClick");
        if (lc > rc) {
            animator.SetInteger("LeftClick", lc - rc);
            animator.SetInteger("RightClick", 0);
        } else {
            animator.SetInteger("LeftClick", 0);
            animator.SetInteger("RightClick", rc - lc);
        }

        int cd = animator.GetInteger("CameraDirection");
        GameObject.Find("CameraBase").transform.Rotate(new Vector3(0, cd, 0));
        animator.SetInteger("CameraDirection", 0);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
