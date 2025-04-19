using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss : Enemy
{
    public static Boss Instance;

    [SerializeField] GameObject slashEffect;
    public Transform SideAttackTransform; //the middle of the side attack area
    public Vector2 SideAttackArea; //how large the area of side attack is

    public Transform UpAttackTransform; //the middle of the up attack area
    public Vector2 UpAttackArea; //how large the area of side attack is

    public Transform DownAttackTransform; //the middle of the down attack area
    public Vector2 DownAttackArea; //how large the area of down attack is

    public float attackRange;
    public float attackTimer;

    [HideInInspector] public bool facingRight;

    [Header("Ground Check Settings:")]
    public Transform groundCheckPoint; //point at which ground check happens
    [SerializeField] private float groundCheckY = 0.2f; //how far down from ground chekc point is Grounded() checked
    [SerializeField] private float groundCheckX = 0.5f; //how far horizontally from ground chekc point to the edge of the player is
    [SerializeField] private LayerMask whatIsGround; //sets the ground layer

    int hitCounter;
    bool stunned, canStun;
    bool alive;

    [HideInInspector] public float runSpeed;

    public GameObject impactParticle;

    [HideInInspector] public bool attacking;
    [HideInInspector] public float attackCountdown;

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
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        ChangeState(EnemyStates.Boss_Stage1);


    }

    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
        Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
        Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        if (!attacking)
        {
            attackCountdown -= Time.deltaTime;
        }
    }

    public void Flip()
    {
        if (PlayerMovement.Instance.transform.position.x < transform.position.x && transform.localScale.x > 0)
        {
            transform.eulerAngles = new Vector2(transform.eulerAngles.x, 180);
            facingRight = false;
        }
        else
        {
            transform.eulerAngles = new Vector2(transform.eulerAngles.x, 0);
            facingRight = true;
        }
    }

    protected override void UpdateEnemyStates()
    {
        if (PlayerMovement.Instance != null)
        {
            switch (GetCurrentEnemyState)
            {
                case EnemyStates.Boss_Stage1:
                    break;

                case EnemyStates.Boss_Stage2:
                    break;

                case EnemyStates.Boss_Stage3:
                    break;

                case EnemyStates.Boss_Stage4:
                    break;
            }
        }
    }

    protected override void OnCollisionStay2D(Collision2D _other) { }

    public void AttackHandler()
    {
        if (currentEnemyState == EnemyStates.Boss_Stage1)
        {
            if (Vector2.Distance(PlayerMovement.Instance.transform.position, rb.position) <= attackRange)
            {
                StartCoroutine(TripleSlash());
            }
        }
    }

    public void ResetAllAttacks()
    {
        attacking = false;

        StopCoroutine(TripleSlash());
    }

    //IEnumerator TripleSlash()
    //{
    //    attacking = true;
    //    rb.linearVelocity = Vector2.zero;//dừng chuyển động của Boss trước khi đánh.

    //    anim.SetTrigger("Slash");
    //    SlashAngle();
    //    yield return new WaitForSeconds(0.7f);
    //    anim.ResetTrigger("Slash");

    //    anim.SetTrigger("Slash");
    //    SlashAngle();
    //    yield return new WaitForSeconds(0.8f);
    //    anim.ResetTrigger("Slash");

    //    anim.SetTrigger("Slash");
    //    SlashAngle();
    //    yield return new WaitForSeconds(0.9f);
    //    anim.ResetTrigger("Slash");

    //    ResetAllAttacks();
    //}

    IEnumerator TripleSlash()
    {
        attacking = true;
        rb.linearVelocity = Vector2.zero; // Dừng di chuyển

        anim.SetTrigger("Slash");
        yield return new WaitForSeconds(0.4f); // đợi nửa animation
        SlashAngle();
        yield return new WaitForSeconds(0.3f); // đợi tiếp phần còn lại
        anim.ResetTrigger("Slash");

        anim.SetTrigger("Slash");
        yield return new WaitForSeconds(0.5f);
        SlashAngle();
        yield return new WaitForSeconds(0.3f);
        anim.ResetTrigger("Slash");

        anim.SetTrigger("Slash");
        yield return new WaitForSeconds(0.5f);
        SlashAngle();
        yield return new WaitForSeconds(0.3f);
        anim.ResetTrigger("Slash");

        ResetAllAttacks();
    }


    void SlashAngle()
    {
        if (PlayerMovement.Instance.transform.position.x - transform.position.x != 0)
        {
            Instantiate(slashEffect, SideAttackTransform);
        }
        else if (PlayerMovement.Instance.transform.position.y > transform.position.y)
        {
            SlashEffectAtAngle(slashEffect, 80, UpAttackTransform);
        }
        else if (PlayerMovement.Instance.transform.position.y < transform.position.y)
        {
            SlashEffectAtAngle(slashEffect, -90, UpAttackTransform);
        }
    }

    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    {
        _slashEffect = Instantiate(_slashEffect, _attackTransform);
        _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }
}
