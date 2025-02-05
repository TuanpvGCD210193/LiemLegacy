    using System.Collections;
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
        float timeBetweenAttack, timeSinceAttack;
        [SerializeField] Transform SideAttackTransform, UpAttackTransform, DownAttackTransform;
        [SerializeField] Vector2 SideAttackArea, UpAttackArea, DownAttackArea;
        [SerializeField] LayerMask attackableLayer;
        [SerializeField] float damage;

        private float xAxis;
        private float yAxis;
        private Rigidbody2D rb;
        Animator animator;
        PlayerStateList playerState;
        private float gravity;
        private bool canDash = true;
        private bool dashed;
        
        

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
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            playerState = GetComponent<PlayerStateList>();
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            gravity = rb.gravityScale;

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
            Move();
            jump();
            PlayerFlip();
            StartDash();
            Attack();
        }

        void GetInputs()
        {
            xAxis = Input.GetAxisRaw("Horizontal");
            yAxis = Input.GetAxisRaw("Vertical");
            attack = Input.GetMouseButtonDown(0); 
            

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
                Hitbox(SideAttackTransform, SideAttackArea);
            }
            else if (yAxis > 0)
            {
                Hitbox(UpAttackTransform, UpAttackArea);
            }
            else if (yAxis < 0 && !Grounded())
            {
                Hitbox(DownAttackTransform, DownAttackArea);
            }
            }
        }

        private void Hitbox (Transform _attackTransform, Vector2 _attackArea)
        {
            Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

            for(int i = 0; i < objectsToHit.Length; i++)
            {
                if (objectsToHit[i].GetComponent<Enemy>() != null)
                {
                    objectsToHit[i].GetComponent<Enemy>().EnemyHit(damage);
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
