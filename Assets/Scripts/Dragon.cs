using UnityEngine;

public class Dragon : Enemy
{
    [SerializeField] private float chaseDistance;
    [SerializeField] private float stunDuration;

    float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Drag_Idle);
    }

    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, PlayerMovement.Instance.transform.position);

        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Drag_Idle:
                rb.linearVelocity = new Vector2(0, 0);
                if (_dist < chaseDistance)
                {
                    ChangeState(EnemyStates.Drag_Chase);
                }
                break;

            case EnemyStates.Drag_Chase:
                rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerMovement.Instance.transform.position, Time.deltaTime * speed));

                FlipDragon();
                if (_dist > chaseDistance)
                {
                    ChangeState(EnemyStates.Drag_Idle);
                }
                break;
            case EnemyStates.Drag_Stunned:
                timer += Time.deltaTime;

                if (timer > stunDuration)
                {
                    ChangeState(EnemyStates.Drag_Idle);
                    timer = 0;
                }
                break;
        }
    }

    public override void EnemyGetsHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyGetsHit(_damageDone, _hitDirection, _hitForce);

        if (health > 0)
        {
            ChangeState(EnemyStates.Drag_Stunned);
        }
        else
        {
            ChangeState(EnemyStates.Drag_Death);
        }
    }
    protected override void Death(float _destroyTime)
    {
        rb.gravityScale = 12;
        base.Death(_destroyTime);
    }

    protected override void ChangeCurrentAnimation()
    {
        anim.SetBool("Idle", GetCurrentEnemyState == EnemyStates.Drag_Idle);

        anim.SetBool("Chase", GetCurrentEnemyState == EnemyStates.Drag_Chase);

        anim.SetBool("Stunned", GetCurrentEnemyState == EnemyStates.Drag_Stunned);

        if (GetCurrentEnemyState == EnemyStates.Drag_Death)
        {
            anim.SetTrigger("Death");
            int LayerIgnoreRaycast = LayerMask.NameToLayer("Ignore Player");
            gameObject.layer = LayerIgnoreRaycast;
        }
    }



    void FlipDragon()
    {
        sr.flipX = PlayerMovement.Instance.transform.position.x < transform.position.x;
    }
}
