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

        [Header("Player Mana Settings")]
        [SerializeField] float mana;
        [SerializeField] float manaDrainSpeed;
        [SerializeField] float manaGain;

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
            Mana = mana;
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
        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            animator.SetTrigger("Attacking");

            if (yAxis == 0 || (yAxis < 0 && Grounded()))
            {
                if (Hitbox(SideAttackTransform, SideAttackArea, ref playerState.recoilingX, recoilXSpeed))
                {
                    Instantiate(slashEffect, SideAttackTransform);
                }
            }
            else if (yAxis > 0)
            {
                if (Hitbox(UpAttackTransform, UpAttackArea, ref playerState.recoilingY, recoilYSpeed))
                {
                    SlashEffectAtAngle(slashEffect, 80, UpAttackTransform);
                }
            }
        }
    }

    private bool Hitbox(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);
        List<Enemy> hitEnemies = new List<Enemy>();

        bool hitSomething = false;

        foreach (Collider2D obj in objectsToHit)
        {
            Enemy e = obj.GetComponent<Enemy>();
            if (e && !hitEnemies.Contains(e))
            {
                Vector2 recoilDirection = (transform.position - e.transform.position).normalized;
                e.EnemyHit(damage, recoilDirection, _recoilStrength);
                hitEnemies.Add(e);

                _recoilDir = true; // Chỉ recoil khi đánh trúng enemy
                rb.linearVelocity = recoilDirection * _recoilStrength; // Recoil theo hướng ngược lại

                if (obj.CompareTag("Enemy"))
                {
                    Mana += manaGain;
                }

                hitSomething = true;
            }
        }

        return hitSomething;
    }

    void Recoil()
    {
        if (playerState.recoilingX)
        {
            stepsXRecoiled++;
            if (stepsXRecoiled >= recoilXSteps)
            {
                StopRecoilX();
            }
        }

        if (playerState.recoilingY)
        {
            rb.gravityScale = 0;
            stepsYRecoiled++;
            if (stepsYRecoiled >= recoilYSteps)
            {
                StopRecoilY();
            }
        }
        else
        {
            rb.gravityScale = gravity;
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

    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
        {
            _slashEffect = Instantiate(_slashEffect, _attackTransform);
            _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
            _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
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
        if (Input.GetButton("Healing") && Health < maxHealth && Grounded() && Mana > 0 && !playerState.dashing) 
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
            Mana -= Time.deltaTime * manaDrainSpeed; 
        }
        else
        {
            playerState.healing = false;
            animator.SetBool("Healing", false);
            healTimer = 0;
        }
    }

    float Mana
    {
        get { return mana; }
        set
        {
            if (mana != value)
            {
                mana = Mathf.Clamp(value, 0, 1);

            }
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
