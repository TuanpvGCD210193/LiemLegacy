using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Lunge : StateMachineBehaviour
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
        rb.gravityScale = 0;
        int _dir = Boss.Instance.facingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(_dir * (Boss.Instance.speed * 5), 0f);

        if (Vector2.Distance(PlayerMovement.Instance.transform.position, rb.position) <= Boss.Instance.attackRange &&
            !Boss.Instance.damagedPlayer && !PlayerMovement.Instance.playerState.invincible)
        {
            PlayerMovement.Instance.TakeDamage(Boss.Instance.damage);
            if (PlayerMovement.Instance.playerState.alive)
            {
                PlayerMovement.Instance.HitStopTime(0, 5, 0.5f);
            }
            Boss.Instance.damagedPlayer = true;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}