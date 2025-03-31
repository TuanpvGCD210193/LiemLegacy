using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string transitionedFromScene;
    public static GameManager Instance { get; private set; }
    public Vector2 platformingRespawnPoint;
    public Vector2 respawnPoint;
    [SerializeField] Vector2 defaultRespawnPoint;
    [SerializeField] Bench bench;
    public Bench lastInteractedBench;
    public GameObject shade;

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
        DontDestroyOnLoad(gameObject);
        respawnPoint = defaultRespawnPoint; // Đặt điểm hồi sinh mặc định
    }
    public void SetRespawnPoint(Vector2 position)
    {
        respawnPoint = position;
    }
    public void RespawnPlayer()
    {
        //if (bench.interacted) //set the respawn point to the bench's position.
        //{
        //    respawnPoint = bench.transform.position;
        //}
        //else
        //{
        //    respawnPoint = defaultRespawnPoint;
        //}

        if (lastInteractedBench != null && lastInteractedBench.interacted)
        {
            respawnPoint = lastInteractedBench.transform.position;
        }

        PlayerMovement.Instance.transform.position = respawnPoint;
        StartCoroutine(UIManager.Instance.DeactivateDeathScreen());
        PlayerMovement.Instance.Respawned();
    }

    
}
