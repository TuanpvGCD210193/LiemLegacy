using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health = 10f;
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;

    [SerializeField] protected PlayerMovement player;
    [SerializeField] protected float speed;
    [SerializeField] protected float damage;
    [SerializeField] protected GameObject orangeBlood;

    protected float recoilTimer;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator anim;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = PlayerMovement.Instance;
        if (player == null)
        {
            Debug.LogError(" Player instance is NULL in Enemy script!");
        }
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    protected EnemyStates currentEnemyState;

    protected virtual void Awake()
    {


    }



    protected virtual EnemyStates GetCurrentEnemyState
    {
        get { return currentEnemyState; }
        set
        {
            if (currentEnemyState != value)
            {
                currentEnemyState = value;
                ChangeCurrentAnimation();
            }
        }
    }

    protected enum EnemyStates
    {
        //Crawler
        Crawler_Idle,
        Crawler_Flip,

        //Dragon
        Drag_Idle,
        Drag_Chase,
        Drag_Stunned,
        Drag_Death,

        //Charger
        Charger_Idle,
        Charger_Suprised,
        Charger_Charge,

        //Shade
        Shade_Idle,
        Shade_Chase,
        Shade_Stunned,
        Shade_Death
    }
    

    // Update is called once per frame
    protected virtual void Update()
    {

        if (health <= 0)
        {
            Destroy(gameObject);
        }
        if (isRecoiling)
        {
            if (recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }

        else
        {
            UpdateEnemyStates();
        }
    }

    public virtual void EnemyGetsHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        if (!isRecoiling)
        {
            GameObject _orangeBlood = Instantiate(orangeBlood, transform.position, Quaternion.identity);
            Destroy(_orangeBlood, 5.5f);
            rb.AddForce(-_hitForce * recoilFactor * _hitDirection);
            isRecoiling = true;
        }
    }

    //protected void OnCollisionStay2D(Collision2D _other)
    //{
    //    if (_other.gameObject.CompareTag("Player") && !PlayerMovement.Instance.playerState.invincible)
    //    {
    //        Attack();
    //        PlayerMovement.Instance.HitStopTime(0, 5, 0.5f);
    //    }
    //}

    protected void OnCollisionStay2D(Collision2D _other)
    {
        //Debug.Log("Enemy is colliding with " + _other.gameObject.name);

        if (_other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Colliding with Player!");
            if (!PlayerMovement.Instance.playerState.invincible)
            {
                Debug.Log("Enemy is attacking the player!");
                Attack();
                if (PlayerMovement.Instance.playerState.alive)
                {
                    PlayerMovement.Instance.HitStopTime(0, 5, 0.5f);
                }
            }
        }
    }




    protected virtual void Death(float _destroyTime)
    {
        Destroy(gameObject, _destroyTime);
    }

    protected virtual void UpdateEnemyStates() { }
    protected virtual void ChangeCurrentAnimation() { }

    protected void ChangeState(EnemyStates _newState)
    {
        GetCurrentEnemyState = _newState;
    }
    protected virtual void Attack()
    {
        PlayerMovement.Instance.TakeDamage(damage);
    }
}
