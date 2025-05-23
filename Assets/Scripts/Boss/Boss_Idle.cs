using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Idle : StateMachineBehaviour
{
    Rigidbody2D rb;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb.linearVelocity = Vector2.zero;
        RunToPlayer(animator);

        if (Boss.Instance.attackCountdown <= 0)
        {
            Boss.Instance.AttackHandler();
            Boss.Instance.attackCountdown = Random.Range(Boss.Instance.attackTimer - 1, Boss.Instance.attackTimer + 1);
        }

        if (!Boss.Instance.Grounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -10); //if knight is not grounded, fall to ground
        }
    }

    void RunToPlayer(Animator animator)
    {
        if (Vector2.Distance(PlayerMovement.Instance.transform.position, rb.position) >= Boss.Instance.attackRange)
        {
            animator.SetBool("Run", true);
        }
        else
        {
            return;
        }
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}