using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace koudaigame2017
{
    public class ClimbCriff_state : StateMachineBehaviour
    {
        Vector3 firstpos;
        Vector3 moveVec;
        CharacterController cc;
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("RejectInput", true);
            cc = animator.GetComponent<CharacterController>();
            cc.enabled = false;
            firstpos = animator.transform.position;
            moveVec = animator.transform.forward * cc.radius * 2f + Vector3.up * cc.height;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.transform.position = firstpos + moveVec * stateInfo.normalizedTime;
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("RejectInput", false);
            cc.enabled = true;
            animator.transform.position = firstpos + moveVec;
            animator.SetBool("catchCriff", false);
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}
    }
}
