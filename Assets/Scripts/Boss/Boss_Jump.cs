using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Jump : StateMachineBehaviour
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
        DiveAttack();
    }

    void DiveAttack()
    {
        if (Boss.Instance.diveAttack)
        {
            Boss.Instance.Flip();

            Vector2 _newPos = Vector2.MoveTowards(rb.position, Boss.Instance.moveToPosition,
                Boss.Instance.speed * 3 * Time.fixedDeltaTime);
            rb.MovePosition(_newPos);

            if (Boss.Instance.TouchedWall())
            {
                Boss.Instance.moveToPosition.x = rb.position.x;
                _newPos = Vector2.MoveTowards(rb.position, Boss.Instance.moveToPosition,
                    Boss.Instance.speed * 3 * Time.fixedDeltaTime);
            }

            float _distance = Vector2.Distance(rb.position, _newPos);
            if (_distance < 0.1f)
            {
                Boss.Instance.Dive();
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}