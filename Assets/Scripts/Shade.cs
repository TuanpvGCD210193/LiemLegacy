using UnityEngine;

public class Shade : Enemy
{
    [SerializeField] private float chaseDistance;
    [SerializeField] private float stunDuration;

    float timer;
    public static Shade Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        Debug.Log("shade spawned");
        SaveData.Instance.SaveShadeData();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Shade_Idle);

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (!PlayerMovement.Instance.playerState.alive)
        {
            ChangeState(EnemyStates.Shade_Idle);
        }
    }

    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, PlayerMovement.Instance.transform.position);

        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Shade_Idle:
                rb.linearVelocity = new Vector2(0, 0);
                if (_dist < chaseDistance)
                {
                    ChangeState(EnemyStates.Shade_Chase);
                }
                break;

            case EnemyStates.Shade_Chase:
                rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerMovement.Instance.transform.position, Time.deltaTime * speed));

                Flip();
                if (_dist > chaseDistance)
                {
                    ChangeState(EnemyStates.Shade_Idle);
                }
                break;

            case EnemyStates.Shade_Stunned:
                timer += Time.deltaTime;

                if (timer > stunDuration)
                {
                    ChangeState(EnemyStates.Shade_Idle);
                    timer = 0;
                }
                break;

            case EnemyStates.Shade_Death:
                Death(Random.Range(5, 10));
                break;
        }
    }
    public override void EnemyGetsHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyGetsHit(_damageDone, _hitDirection, _hitForce);

        if (health > 0)
        {
            ChangeState(EnemyStates.Shade_Stunned);
        }
        else
        {
            ChangeState(EnemyStates.Shade_Death);
        }
    }

    protected override void Death(float _destroyTime)
    {
        rb.gravityScale = 12;
        base.Death(_destroyTime);
    }
    protected override void ChangeCurrentAnimation()
    {
        if (GetCurrentEnemyState == EnemyStates.Shade_Idle)
        {
            anim.Play("Player_animation_idle");
        }

        anim.SetBool("Walking", GetCurrentEnemyState == EnemyStates.Shade_Chase);


        if (GetCurrentEnemyState == EnemyStates.Shade_Death)
        {
            PlayerMovement.Instance.RestoreMana();
            SaveData.Instance.SavePlayerData();
            anim.SetTrigger("Death");
            Destroy(gameObject, 0.5f);
        }
    }

    protected override void Attack()
    {
        anim.SetTrigger("Attacking");
        PlayerMovement.Instance.TakeDamage(damage);
    }
    void Flip()
    {
        sr.flipX = PlayerMovement.Instance.transform.position.x < transform.position.x;
    }

}
