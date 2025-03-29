using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string transitionedFromScene;
    public static GameManager Instance { get; private set; }
    public Vector2 platformingRespawnPoint;
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
    }
}
