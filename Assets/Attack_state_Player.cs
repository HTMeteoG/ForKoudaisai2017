using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace koudaigame2017
{
    public class Attack_state_Player : StateMachineBehaviour
    {
        [SerializeField]
        float actionTime;
        [SerializeField]
        float finActionTime;
        [SerializeField]
        GameObject attackCollider;
        [SerializeField]
        string effectPath = "";

        actionState actioned = actionState.preAction;
        GameObject effect;
        AttackColliderT col;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            actioned = actionState.preAction;
            animator.SetBool("RejectInput", true);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (actioned == actionState.preAction && stateInfo.normalizedTime >= actionTime)
            {
                Transform t = animator.transform;

                if (effectPath.Length > 0)
                {
                    if (effect == null)
                    {
                        Transform et = t.FindChild(effectPath);
                        effect = Instantiate(et.gameObject, et.position, et.rotation, et.parent);
                        //effect.transform.SetParent(et.parent);
                    }
                    effect.SetActive(true);
                }

                GameObject aobject = Instantiate(attackCollider, t.position, t.rotation);
                col = aobject.GetComponentInChildren<AttackColliderT>();
                if (col)
                {
                    col.SetOwner(animator.gameObject);
                }

                actioned = actionState.action;
            }
            if (actioned == actionState.action && stateInfo.normalizedTime >= finActionTime)
            {
                FinAction();
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("RejectInput", false);
            FinAction();
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        void FinAction()
        {
            if (effect)
            {
                effect.transform.SetParent(null);
                Destroy(effect, 0.5f);
            }
            if (col)
            {
                Destroy(col.transform.root.gameObject);
            }

            actioned = actionState.endAction;
        }
    }
}
