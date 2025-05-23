using UnityEngine;

public class Crawler : Enemy 
{
    [Header("Enemy.Crawler Settings:")]
    [SerializeField] private float flipWaitTime;
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgeCheckY;
    [SerializeField] private LayerMask whatIsGround;

    float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{
    //    base.Start();
    //    rb.gravityScale = 12f;
    //}

    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 12f;
    }

    protected override void Update()
    {
        base.Update();
        if (!PlayerMovement.Instance.playerState.alive)
        {
            ChangeState(EnemyStates.Crawler_Idle);
        }
    }
    //protected override void UpdateEnemyStates()
    //{
    //    switch (currentEnemyState)
    //    {
    //        case EnemyStates.Crawler_Idle:
    //            Vector3 _ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
    //            Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;

    //            if (!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround)
    //                || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
    //            {
    //                ChangeState(EnemyStates.Crawler_Flip);
    //            }

    //            if (transform.localScale.x > 0)
    //            {
    //                rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
    //            }
    //            else
    //            {
    //                rb.linearVelocity = new Vector2(-speed, rb.linearVelocity.y);
    //            }
    //            break;
    //        case EnemyStates.Crawler_Flip:
    //            timer += Time.deltaTime;

    //            if (timer > flipWaitTime)
    //            {
    //                timer = 0;
    //                transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
    //                ChangeState(EnemyStates.Crawler_Idle);
    //            }
    //            break;
    //    }
    //}

    private void OnCollisionEnter2D(Collision2D _collision)
    {
        if (_collision.gameObject.CompareTag("Enemy"))
        {
            ChangeState(EnemyStates.Crawler_Flip);
        }
    }

    protected override void UpdateEnemyStates()
    {
        if (health <= 0)
        {
            Death(0.05f);
        }

        switch (currentEnemyState)
        {
            case EnemyStates.Crawler_Idle:
                Vector3 _ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
                Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;

                if (!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround)
                    || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                {
                    ChangeState(EnemyStates.Crawler_Flip);
                }

                // **Thêm đoạn code này để enemy đuổi theo player**
                if (player != null)
                {
                    float direction = Mathf.Sign(player.transform.position.x - transform.position.x);
                    rb.linearVelocity = new Vector2(speed * direction, rb.linearVelocity.y);

                    // Điều chỉnh hướng nhìn
                    if (direction > 0)
                    {
                        transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                    }
                    else
                    {
                        transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                    }
                }
                break;

            case EnemyStates.Crawler_Flip:
                timer += Time.deltaTime;

                if (timer > flipWaitTime)
                {
                    timer = 0;
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                    ChangeState(EnemyStates.Crawler_Idle);
                }
                break;
        }
    }

}
