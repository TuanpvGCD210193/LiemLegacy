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
    public Transform wallCheckPoint; //point at which wall check happens
    [SerializeField] private float groundCheckY = 0.2f; //how far down from ground chekc point is Grounded() checked
    [SerializeField] private float groundCheckX = 0.5f; //how far horizontally from ground chekc point to the edge of the player is
    [SerializeField] private LayerMask whatIsGround; //sets the ground layer

    int hitCounter;
    bool stunned, canStun;
    bool alive;

    [HideInInspector] public float runSpeed;

    public GameObject impactParticle;

    float bloodCountdown;
    float bloodTimer;

    [HideInInspector] public bool attacking;
    [HideInInspector] public float attackCountdown;
    [HideInInspector] public bool damagedPlayer = false;
    [HideInInspector] public bool parrying;


    [HideInInspector] public Vector2 moveToPosition;//slill boss Dive Attack
    [HideInInspector] public bool diveAttack; //slill boss Dive Attack
    public GameObject divingCollider; //slill boss Dive Attack
    public GameObject pillar; //slill boss Dive Attack

    [HideInInspector] public bool barrageAttack;
    public GameObject barrageFireball;
    [HideInInspector] public bool outbreakAttack;

    [HideInInspector] public bool bounceAttack;
    [HideInInspector] public float rotationDirectionToTarget;
    [HideInInspector] public int bounceCount;


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
        alive = true;

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

    public bool TouchedWall()
    {
        if (Physics2D.Raycast(wallCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
    || Physics2D.Raycast(wallCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
    || Physics2D.Raycast(wallCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
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
    protected override void Update()
    {
        if (!alive)
            return;       // ← nếu đã chết thì thôi, không chạy AI nữa
        base.Update();

        if (health <= 0 && alive)
            Death(0);

        if (!attacking)
        {
            attackCountdown -= Time.deltaTime;
        }

        if (stunned)
        {
            rb.linearVelocity = Vector2.zero;
        }

        bloodCountdown -= Time.deltaTime;
        if (bloodCountdown <= 0 && (currentEnemyState != EnemyStates.Boss_Stage1 && currentEnemyState != EnemyStates.Boss_Stage2))
        {
            GameObject _orangeBlood = Instantiate(orangeBlood, groundCheckPoint.position, Quaternion.identity);
            Destroy(_orangeBlood, 4f);
            bloodCountdown = bloodTimer;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            anim.SetTrigger("Die");
            Debug.Log("Pressed K - Trigger Die Sent");
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
                    canStun = true;
                    attackTimer = 6;
                    runSpeed = speed;
                    break;

                case EnemyStates.Boss_Stage2:
                    canStun = true;
                    attackTimer = 5;
                    break;

                case EnemyStates.Boss_Stage3:
                    canStun = true;
                    attackTimer = 8;
                    bloodTimer = 5f;
                    break;

                case EnemyStates.Boss_Stage4:
                    canStun = true;
                    attackTimer = 10;
                    runSpeed = speed / 2;
                    bloodTimer = 1.5f;
                    break;
            }
        }
    }

    protected override void OnCollisionStay2D(Collision2D _other) { }

    //public override void EnemyGetsHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    //{
    //    if (!stunned)
    //    {
    //        if (!parrying)
    //        {
    //            if (canStun)
    //            {
    //                hitCounter++;
    //                if (hitCounter >= 3)
    //                {
    //                    ResetAllAttacks();
    //                    StartCoroutine(Stunned());
    //                }
    //            }

    //            ResetAllAttacks();
    //            base.EnemyGetsHit(_damageDone, _hitDirection, _hitForce);

    //            if (currentEnemyState != EnemyStates.Boss_Stage4)
    //            {
    //                ResetAllAttacks(); //cancel any current attack to avoid bugs 
    //                StartCoroutine(Parry());
    //            }

    //        }
    //        else
    //        {
    //            StopCoroutine(Parry());
    //            parrying = false;
    //            ResetAllAttacks();
    //            StartCoroutine(Slash());  //riposte
    //        }
    //    }
    //    else
    //    {
    //        StopCoroutine(Stunned());
    //        anim.SetBool("Stunned", false);
    //        stunned = false;
    //    }

    //    if (health > 15)
    //    {
    //        ChangeState(EnemyStates.Boss_Stage1);
    //    }
    //    if (health <= 15 && health > 10)
    //    {
    //        ChangeState(EnemyStates.Boss_Stage2);
    //    }
    //    if (health <= 10 && health > 5)
    //    {
    //        ChangeState(EnemyStates.Boss_Stage3);
    //    }
    //    if (health < 5)
    //    {
    //        ChangeState(EnemyStates.Boss_Stage4);
    //    }
    //    if (health <= 0 && alive)
    //    {
    //        Death(0);
    //    }
    //}

    //start refactore
    public override void EnemyGetsHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        if (stunned)
        {
            StopCoroutine(Stunned());
            anim.SetBool("Stunned", false);
            stunned = false;
        }
        else if (parrying)
        {
            StopCoroutine(Parry());
            parrying = false;
            ResetAllAttacks();
            StartCoroutine(Slash());  // riposte
        }
        else
        {
            HandleStunCheck();

            ResetAllAttacks();
            base.EnemyGetsHit(_damageDone, _hitDirection, _hitForce);

            if (currentEnemyState != EnemyStates.Boss_Stage4)
            {
                ResetAllAttacks(); // cancel any current attack to avoid bugs
                StartCoroutine(Parry());
            }
        }

        UpdateBossStage();
    }

    private void HandleStunCheck()
    {
        if (!canStun) return;

        hitCounter++;
        if (hitCounter >= 3)
        {
            ResetAllAttacks();
            StartCoroutine(Stunned());
        }
    }

    private void UpdateBossStage()
    {
        if (health <= 0 && alive)
        {
            Death(0);
            return;
        }

        if (health < 5)
        {
            ChangeState(EnemyStates.Boss_Stage4);
        }
        else if (health <= 10)
        {
            ChangeState(EnemyStates.Boss_Stage3);
        }
        else if (health <= 15)
        {
            ChangeState(EnemyStates.Boss_Stage2);
        }
        else
        {
            ChangeState(EnemyStates.Boss_Stage1);
        }
    }

    // end 

    public void AttackHandler()
    {
        if (currentEnemyState == EnemyStates.Boss_Stage1)
        {
            if (Vector2.Distance(PlayerMovement.Instance.transform.position, rb.position) <= attackRange)
            {
                StartCoroutine(TripleSlash());
            }

            else
            {
                StartCoroutine(Lunge());
                //DiveAttackJump();
                //BarrageBendDown();
                //OutbreakBendDown();
                //BounceAttack();
            }
        }

        if (currentEnemyState == EnemyStates.Boss_Stage2)
        {
            if (Vector2.Distance(PlayerMovement.Instance.transform.position, rb.position) <= attackRange)
            {
                StartCoroutine(TripleSlash());
            }
            else
            {
                int _attackChosen = Random.Range(1, 3);
                if (_attackChosen == 1)
                {
                    StartCoroutine(Lunge());
                }
                if (_attackChosen == 2)
                {
                    DiveAttackJump();
                }
                if (_attackChosen == 3)
                {
                    BarrageBendDown();
                }
            }
        }
        if (currentEnemyState == EnemyStates.Boss_Stage3)
        {
            int _attackChosen = Random.Range(1, 4);
            if (_attackChosen == 1)
            {
                OutbreakBendDown();
            }
            if (_attackChosen == 2)
            {
                DiveAttackJump();
            }
            if (_attackChosen == 3)
            {
                BarrageBendDown();
            }
            if (_attackChosen == 4)
            {
                BounceAttack();
            }
        }
        if (currentEnemyState == EnemyStates.Boss_Stage4)
        {
            if (Vector2.Distance(PlayerMovement.Instance.transform.position, rb.position) <= attackRange)
            {
                StartCoroutine(Slash());
            }
            else
            {
                BounceAttack();
            }
        }

    }

    public void ResetAllAttacks()
    {
        attacking = false;

        StopCoroutine(TripleSlash());

        StopCoroutine(Lunge());

        StopCoroutine(Parry());

        StopCoroutine(Slash());

        diveAttack = false;

        barrageAttack = false;

        outbreakAttack = false;

        bounceAttack = false;
    }

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
    IEnumerator Lunge()
    {
        Flip();
        attacking = true;

        anim.SetBool("Lunge", true);
        yield return new WaitForSeconds(1f);
        anim.SetBool("Lunge", false);
        damagedPlayer = false;
        ResetAllAttacks();
    }

    IEnumerator Parry()
    {
        attacking = true;
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("Parry", true);
        yield return new WaitForSeconds(0.8f);
        anim.SetBool("Parry", false);

        parrying = false;
        ResetAllAttacks();
    }

    IEnumerator Slash()
    {
        attacking = true;
        rb.linearVelocity = Vector2.zero; // Dừng di chuyển

        anim.SetTrigger("Slash");
        yield return new WaitForSeconds(0.4f); // đợi nửa animation
        SlashAngle();
        yield return new WaitForSeconds(0.3f); // đợi tiếp phần còn lại
        anim.ResetTrigger("Slash");

        ResetAllAttacks();
    }

    void DiveAttackJump()
    {
        attacking = true;
        moveToPosition = new Vector2(PlayerMovement.Instance.transform.position.x, rb.position.y + 10);
        diveAttack = true;
        anim.SetBool("Jump", true);
    }

    public void Dive()
    {
        anim.SetBool("Dive", true);
        anim.SetBool("Jump", false);
    }
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.GetComponent<PlayerMovement>() != null && (diveAttack || bounceAttack))
        {
            _other.GetComponent<PlayerMovement>().TakeDamage(damage * 2);
            PlayerMovement.Instance.playerState.recoilingX = true;
        }
    }

    public void DivingPillars()
    {
        Vector2 _impactPoint = groundCheckPoint.position;
        float _spawnDistance = 5;

        for (int i = 0; i < 10; i++)
        {
            Vector2 _pillarSpawnPointRight = _impactPoint + new Vector2(_spawnDistance, 0);
            Vector2 _pillarSpawnPointLeft = _impactPoint - new Vector2(_spawnDistance, 0);
            Instantiate(pillar, _pillarSpawnPointRight, Quaternion.Euler(0, 0, -90));
            Instantiate(pillar, _pillarSpawnPointLeft, Quaternion.Euler(0, 0, -90));

            _spawnDistance += 5;
        }
        ResetAllAttacks();
    }

    void BarrageBendDown()
    {
        attacking = true;
        rb.linearVelocity = Vector2.zero;
        barrageAttack = true;
        anim.SetTrigger("BendDown");
    }

    public IEnumerator Barrage()//used BossEvent
    {
        rb.linearVelocity = Vector2.zero;

        float _currentAngle = 30f;
        for (int i = 0; i < 10; i++)
        {
            Debug.Log("launch");
            GameObject _projectile = Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, _currentAngle));

            if (facingRight)
            {
                _projectile.transform.eulerAngles = new Vector3(_projectile.transform.eulerAngles.x, 0, _currentAngle);
            }
            else
            {
                _projectile.transform.eulerAngles = new Vector3(_projectile.transform.eulerAngles.x, 180, _currentAngle);
            }

            _currentAngle += 5f;

            yield return new WaitForSeconds(0.3f);
        }
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("Cast", false);
        ResetAllAttacks();
    }

    void OutbreakBendDown()
    {
        attacking = true;
        rb.linearVelocity = Vector2.zero;
        moveToPosition = new Vector2(transform.position.x, rb.position.y + 5);
        outbreakAttack = true;
        anim.SetTrigger("BendDown");
    }

    public IEnumerator Outbreak()
    {
        yield return new WaitForSeconds(1f);
        anim.SetBool("Cast", true);

        rb.linearVelocity = Vector2.zero;
        for (int i = 0; i < 30; i++)
        {
            Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, Random.Range(110, 130))); //downwards
            Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, Random.Range(50, 70))); // diagonally right
            Instantiate(barrageFireball, transform.position, Quaternion.Euler(0, 0, Random.Range(260, 280))); // diagonally left

            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(0.1f);
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -10);
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("Cast", false);
        ResetAllAttacks();
    }

    void BounceAttack()
    {
        attacking = true;
        bounceCount = Random.Range(2, 5);
        BounceBendDown();
    }

    int _bounces = 0;
    public void CheckBounce()
    {
        if (_bounces < bounceCount - 1)
        {
            _bounces++;
            BounceBendDown();
        }
        else
        {
            _bounces = 0;
            anim.Play("Boss_Run");
        }
    }

    public void BounceBendDown()
    {
        rb.linearVelocity = Vector2.zero;
        moveToPosition = new Vector2(PlayerMovement.Instance.transform.position.x, rb.position.y + 10);
        bounceAttack = true;
        anim.SetTrigger("BendDown");
    }

    public void CalculateTargetAngle()
    {
        Vector3 _directionToTarget = (PlayerMovement.Instance.transform.position - transform.position).normalized;

        float _angleOfTarget = Mathf.Atan2(_directionToTarget.y, _directionToTarget.x) * Mathf.Rad2Deg;
        rotationDirectionToTarget = _angleOfTarget;
    }

    public IEnumerator Stunned()
    {
        stunned = true;
        hitCounter = 0;
        anim.SetBool("Stunned", true);
        yield return new WaitForSeconds(6f);
        anim.SetBool("Stunned", false);
        stunned = false;
    }
    protected override void Death(float _destroyTime)
    {
        Debug.Log("DEATH FUNCTION CALLED");
        ResetAllAttacks();
        alive = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -25);

        // Thêm debug để chắc chắn gọi trigger
        Debug.Log("Calling Die Trigger");
        anim.SetTrigger("Die");
        Debug.Log("DIE TRIGGER SET!");
        bloodTimer = 0.8f;

        // Xác nhận animation
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
        {
            Debug.Log("Die animation is playing");
        }
        else
        {
            Debug.Log("Die animation is NOT playing");
        }
    }

    public void DestroyAfterDeath()
    {
        Destroy(gameObject);
    }
}
