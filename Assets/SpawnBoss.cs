using System.Collections;
using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    public static SpawnBoss Instance;
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject boss;
    [SerializeField] Vector2 exitDirection;
    bool callOnce;
    BoxCollider2D col;

    private void Awake()
    {
        if (Boss.Instance != null)
        {
            Destroy(Boss.Instance);
            callOnce = false;
            col.isTrigger = true;
        }

        if (GameManager.Instance.Boss_Defeated)
        {
            callOnce = true;
        }
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player") && !callOnce && !GameManager.Instance.Boss_Defeated)
        {
            StartCoroutine(WalkIntoRoom());
            callOnce = true;
        }
    }
    IEnumerator WalkIntoRoom()
    {
        StartCoroutine(PlayerMovement.Instance.WalkIntoNewScene(exitDirection, 1));
        PlayerMovement.Instance.GetComponent<PlayerStateList>().cutscene = true;
        yield return new WaitForSeconds(1f);
        col.isTrigger = false;
        Instantiate(boss, spawnPoint.position, Quaternion.identity);
    }
    public void IsNotTrigger()
    {
        col.isTrigger = true;
    }
}