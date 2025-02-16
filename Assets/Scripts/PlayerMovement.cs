    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class PlayerMovement : MonoBehaviour
    {
    
        [Header("Horizontal change move settings for Player")]
        [SerializeField] private float walkSpeed = 1;
        [Header("Vertical change move settings for Player")]
        private int jumpBufferCounter = 0;
        [SerializeField] private int jumpBufferFrames;
        private float coyoteTimeCounter = 0;
        [SerializeField] private float coyoteTime;
        private int airJumpCounter = 0;
        [SerializeField] private int maxAirJump;

        [Header("Horizontal change Jump settings for Player")]
        [SerializeField] private float jumpHeight=20;
        [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private float groundCheckY = 0.2f;
        [SerializeField] private float groundCheckX = 0.5f;
        [SerializeField] private LayerMask WhatIsGround;

        [Header("Change Dash Setting for player")]
        [SerializeField] private float dashSpeed;
        [SerializeField] private float dashTime;
        [SerializeField] private float dashCoolDown;
        [SerializeField] GameObject dashEffect;

        [Header("Change Attack Setting for player")]
        bool attack = false;
        [SerializeField] private float timeBetweenAttack;
        [SerializeField] private float timeSinceAttack;
        [SerializeField] Transform SideAttackTransform, UpAttackTransform, DownAttackTransform;
        [SerializeField] Vector2 SideAttackArea, UpAttackArea, DownAttackArea;
        [SerializeField] LayerMask attackableLayer;
        [SerializeField] float damage;
        [SerializeField] GameObject slashEffect;

        bool restoreTime;
        float restoreTimeSpeed;

        [Header("Player Attack recoil")]
        [SerializeField] private float recoilXSteps = 5;
        [SerializeField] private float recoilYSteps = 5;
        [SerializeField] private float recoilXSpeed = 100;
        [SerializeField] private float recoilYSpeed = 100;
        private int stepsXRecoiled, stepsYRecoiled;

        [Header("Player Health Point setting")]
        public int health;
        public int maxHealth;
        [SerializeField] GameObject bloodSpurt;
        [SerializeField] float hitFlashSpeed;
        public delegate void OnHealthChangedDelegate();
        [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallback;

        float healTimer;
        [SerializeField] float timeToHeal;

        private float xAxis;
        private float yAxis;
        private Rigidbody2D rb;
        Animator animator;
        public PlayerStateList playerState;
        private float gravity;
        private bool canDash = true;
        private bool dashed;
        private SpriteRenderer sr;



        public static PlayerMovement Instance;

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
            Health = maxHealth;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            playerState = GetComponent<PlayerStateList>();
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            gravity = rb.gravityScale;
            sr = GetComponent<SpriteRenderer>();
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
            GetInputs();
            UpdateJumpVariables();

            if(playerState.dashing) return;
            RestoreTimeScale();
            FlashWhileInvincible();
            Move();
            Heal();
            jump();

            PlayerFlip();
            StartDash();
            Attack();
            //Recoil();
            
            
        }

        private void FixedUpdate()
        {
            if (playerState.dashing) return;
            Recoil();
        }

    void GetInputs()
        {
            xAxis = Input.GetAxisRaw("Horizontal");
            yAxis = Input.GetAxisRaw("Vertical");
            attack = Input.GetButtonDown("Attack");
            

        }

        private void Move()
        {
            rb.linearVelocity = new Vector2(walkSpeed * xAxis, rb.linearVelocity.y);
            animator.SetBool("Walking", rb.linearVelocity.x != 0 && Grounded());
        }

        void StartDash()
        {
            if (Input.GetButtonDown("Dash") && canDash && !dashed)
            {
                StartCoroutine(Dash());
                dashed = true;
            }

            if (Grounded())
            {
                dashed = false;
            }
        }

        IEnumerator Dash() 
        {
            canDash = false;
            playerState.dashing = true;
            animator.SetTrigger("Dashing");
            rb.gravityScale = 0;
            rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0);
            if (Grounded()) Instantiate(dashEffect,transform);
            yield return new WaitForSeconds(dashTime);
            rb.gravityScale = gravity;
            playerState.dashing =false;
            yield return new WaitForSeconds(dashCoolDown);
            canDash = true;
        }
        
        void Attack()
        {
            timeSinceAttack += Time.deltaTime;
            if( attack && timeSinceAttack >= timeBetweenAttack)
            {
                timeSinceAttack = 0;
                animator.SetTrigger("Attacking");
                if (yAxis == 0 || yAxis < 0 && Grounded())
                {
                        Hitbox(SideAttackTransform, SideAttackArea, ref playerState.recoilingX, recoilXSpeed);
                        Instantiate(slashEffect, SideAttackTransform);

                }
                else if (yAxis > 0)
                {
                    Hitbox(UpAttackTransform, UpAttackArea, ref playerState.recoilingY, recoilYSpeed);
                    SlashEffectAtAngle(slashEffect, 80, UpAttackTransform);
                }
            //else if (yAxis < 0 && !Grounded())
            //{
            //    Debug.Log("Down Attack Activated!");
            //    Hitbox(DownAttackTransform, DownAttackArea, , ref playerState.recoilingY, recoilYSpeed);
            //    SlashEffectAtAngle(slashEffect, -90, DownAttackTransform);
            //}
            }
        }

        private void Hitbox(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilDir, float _recoilStrength)
        {
        //Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

        //if (objectsToHit.Length > 0)
        //{
        //    _recoilDir = true;
        //}

        //for (int i = 0; i < objectsToHit.Length; i++)
        //{
        //    if (objectsToHit[i].GetComponent<Enemy>() != null)
        //    {
        //        objectsToHit[i].GetComponent<Enemy>().EnemyHit
        //        (damage, (transform.position - objectsToHit[i].transform.position).normalized, _recoilStrength);
        //    }

        //}

        //Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

        //bool hitSomething = false;

        //for (int i = 0; i < objectsToHit.Length; i++)
        //{
        //    Enemy enemy = objectsToHit[i].GetComponent<Enemy>();
        //    if (enemy != null)
        //    {
        //        hitSomething = true; // Chỉ recoil nếu thực sự đánh trúng enemy
        //        enemy.EnemyHit(damage, (transform.position - objectsToHit[i].transform.position).normalized, _recoilStrength);
        //    }
        //}

        //if (hitSomething)
        //{
        //    _recoilDir = true;
        //}
            Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

            bool hitSomething = false;
            HashSet<Enemy> hitEnemies = new HashSet<Enemy>();

            for (int i = 0; i < objectsToHit.Length; i++)
            {
                Enemy enemy = objectsToHit[i].GetComponent<Enemy>();
                if (enemy != null && !hitEnemies.Contains(enemy))
                {
                    hitSomething = true;
                    hitEnemies.Add(enemy);
                    enemy.EnemyHit(damage, (transform.position - objectsToHit[i].transform.position).normalized, _recoilStrength);
                }
            }

            if (hitSomething)
            {
                _recoilDir = true;
            }
        }

        void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
        {
            _slashEffect = Instantiate(_slashEffect, _attackTransform);
            _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
            _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
        }

    void Recoil()
    {
        if (playerState.recoilingX)
        {
            if (playerState.lookingRight)
            {
                rb.linearVelocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rb.linearVelocity = new Vector2(recoilXSpeed, 0);
            }

            //if (playerState.recoilingX)
            //{
            //    rb.linearVelocity = new Vector2(-transform.localScale.x * recoilXSpeed, 0);
            //}
        }

        if (playerState.recoilingY)
        {
            rb.gravityScale = 0;
            if (yAxis < 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, recoilYSpeed);
            }
            else
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -recoilYSpeed);
            }
            airJumpCounter = 0;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        //stop recoil
        if (playerState.recoilingX && stepsXRecoiled < recoilXSteps)
        {
            stepsXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }
        if (playerState.recoilingY && stepsYRecoiled < recoilYSteps)
        {
            stepsYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }

        if (Grounded())
        {
            StopRecoilY();
        }
    }

    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        playerState.recoilingX = false;
    }
    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        playerState.recoilingY = false;
    }

    public void TakeDamage(float _damage)
    {
        Health -= Mathf.RoundToInt(_damage);
        StartCoroutine(StopTakingDamage());
    }
    IEnumerator StopTakingDamage()
    {
        playerState.invincible = true;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        animator.SetTrigger("TakeDamage");
        yield return new WaitForSeconds(1.5f);// tg bất chỉ định
        playerState.invincible = false;
    }

    void FlashWhileInvincible()
    {
        sr.material.color = playerState.invincible ? Color.Lerp
            (Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashSpeed, 1.0f)) : Color.white;
    }

    void RestoreTimeScale()
    {
        if (restoreTime)
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale += Time.unscaledDeltaTime * restoreTimeSpeed;
            }
            else
            {
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }

    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        restoreTimeSpeed = _restoreSpeed;
        if (_delay > 0)
        {
            StopCoroutine(StartTimeAgain(_delay));
            StartCoroutine(StartTimeAgain(_delay));
        }
        else
        {
            restoreTime = true;
        }
        Time.timeScale = _newTimeScale;
    }

    IEnumerator StartTimeAgain(float _delay)
    {
        yield return new WaitForSecondsRealtime(_delay);
        restoreTime = true;
    }

    public int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);

                if (onHealthChangedCallback != null)
                {
                    onHealthChangedCallback.Invoke();
                }
            }
        }
    }

    void Heal()
    {
        if (Input.GetButton("Healing") && Health < maxHealth && Grounded() && !playerState.dashing) // Khi player nhan roi thi se heal
        {
            playerState.healing = true;
            animator.SetBool("Healing", true);

            //healing
            healTimer += Time.deltaTime;
            if (healTimer >= timeToHeal)
            {
                Health++;
                healTimer = 0;
            }
        }
        else
        {
            playerState.healing = false;
            animator.SetBool("Healing", false);
            healTimer = 0;
        }
    }

    public bool Grounded()
        {
            if(Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, WhatIsGround) 
                || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, WhatIsGround)
                || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, WhatIsGround))
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        void jump()
        {
            if (Input.GetButtonDown("Jump") && rb.linearVelocity.y >0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                playerState.jumping = false;
            }
            if (!playerState.jumping)
            {
                if (jumpBufferCounter >0 && coyoteTimeCounter >0)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpHeight);
                    playerState.jumping = true;
                }
                else if (!Grounded() && airJumpCounter < maxAirJump && Input.GetButtonDown("Jump"))
                {
                    playerState.jumping = true;
                    airJumpCounter++;
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpHeight);
                }
            }

            animator.SetBool("Jumping", !Grounded());
        }

        void PlayerFlip()
        {
            if (xAxis < 0)
            {
                transform.localScale = new Vector2(-1, transform.localScale.y);

            }
            else if (xAxis > 0)
            {
                transform.localScale = new Vector2(1, transform.localScale.y);
            }
        }

        void UpdateJumpVariables()
        {
            if (Grounded())
            {
                playerState.jumping = false;
                coyoteTimeCounter = coyoteTime;
                airJumpCounter = 0;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }

            if (Input.GetButtonDown("Jump"))
            {
                jumpBufferCounter = jumpBufferFrames;
            }
            else
            {
                jumpBufferCounter--;
            }
        }
    }
