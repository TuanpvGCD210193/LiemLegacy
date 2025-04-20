using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Dive : StateMachineBehaviour
{
    Rigidbody2D rb;
    bool callOnce;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    Boss.Instance.divingCollider.SetActive(true);

    //    if (Boss.Instance.Grounded())
    //    {
    //        Boss.Instance.divingCollider.SetActive(false);

    //        if (!callOnce)
    //        {
    //            Boss.Instance.DivingPillars();
    //            animator.SetBool("Dive", false);
    //            Boss.Instance.ResetAllAttacks();
    //            callOnce = true;
    //        }
    //    }
    //}

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Đặt collider active
        Boss.Instance.divingCollider.SetActive(true);

        if (Boss.Instance.divingCollider.activeSelf)
        {
            Debug.Log(" divingCollider SET TO ACTIVE");
        }
        else
        {
            Debug.Log(" divingCollider FAILED TO SET ACTIVE");
        }

        // Kiểm tra boss có grounded không
        if (Boss.Instance.Grounded())
        {
            Debug.Log(" BOSS IS GROUNDED");

            Boss.Instance.divingCollider.SetActive(false);

            if (!callOnce)
            {
                Debug.Log(" CALLING DivingPillars() AND RESETTING ATTACK");
                Boss.Instance.DivingPillars();
                animator.SetBool("Dive", false);
                Boss.Instance.ResetAllAttacks();
                callOnce = true;
            }
        }
        else
        {
            Debug.Log("❌ BOSS NOT GROUNDED YET");
        }
    }


    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        callOnce = false;
    }
}