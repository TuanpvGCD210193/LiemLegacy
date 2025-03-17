using UnityEngine;
using UnityEngine.SceneManagement;

//public class SceneTransition : MonoBehaviour
//{
//    [SerializeField] private string transitionTo; //Represents the scene to transition to

//    [SerializeField] private Transform startPoint; //Defines the player's entry point in the scene

//    [SerializeField] private Vector2 exitDirection; //Specifies the direction for the player's exit

//    [SerializeField] private float exitTime; //Determines the time it takes for the player to exit the scene transition

//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    private void Start()
//    {
//        if (GameManager.Instance.transitionedFromScene == transitionTo)
//        {
//            PlayerMovement.Instance.transform.position = startPoint.position;
//            StartCoroutine(PlayerMovement.Instance.WalkIntoNewScene(exitDirection, exitTime));
//        }
//    }

//    private void OnTriggerEnter2D(Collider2D _other)
//    {
//        if (_other.CompareTag("Player"))
//        {
//            GameManager.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;
//            PlayerMovement.Instance.playerState.cutscene = true;
//            PlayerMovement.Instance.playerState.invincible = true;
//            SceneManager.LoadScene(transitionTo);
//        }
//    }
//}

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string transitionTo; // Scene sẽ chuyển tới
    [SerializeField] private Transform startPoint; // Điểm spawn của player
    [SerializeField] private Vector2 exitDirection; // Hướng thoát khỏi scene cũ
    [SerializeField] private float exitTime; // Thời gian di chuyển lúc đổi scene

    private bool canTransition = true; // Cờ kiểm soát chuyển scene

    private void Start()
    {
        if (GameManager.Instance.transitionedFromScene == transitionTo)
        {
            PlayerMovement.Instance.transform.position = startPoint.position;
            StartCoroutine(PlayerMovement.Instance.WalkIntoNewScene(exitDirection, exitTime));

            // 🟢 Reset trạng thái để player điều khiển được
            PlayerMovement.Instance.playerState.cutscene = false;
            PlayerMovement.Instance.playerState.invincible = false;

            StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
        }
    }

    private void ResetPlayerState()
    {
        PlayerMovement.Instance.rb.linearVelocity = Vector2.zero;
        PlayerMovement.Instance.xAxis = 0;
        PlayerMovement.Instance.playerState.cutscene = false;
        PlayerMovement.Instance.playerState.invincible = false;
    }

    //private void OnTriggerEnter2D(Collider2D _other)
    //{
    //    if (_other.CompareTag("Player") && canTransition)
    //    {
    //        // Lưu lại scene hiện tại trước khi chuyển
    //        GameManager.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;
    //        PlayerMovement.Instance.playerState.cutscene = true;
    //        PlayerMovement.Instance.playerState.invincible = true;

    //        SceneManager.LoadScene(transitionTo);
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            GameManager.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;

            PlayerMovement.Instance.playerState.cutscene = true;
            PlayerMovement.Instance.playerState.invincible = true;

            //SceneManager.LoadScene(transitionTo);

            // 🟢 Đảm bảo reset ngay sau khi load
            PlayerMovement.Instance.playerState.cutscene = false;
            PlayerMovement.Instance.playerState.invincible = false;

            StartCoroutine(UIManager.Instance.sceneFader.FadeAndLoadScene(SceneFader.FadeDirection.In, transitionTo));
        }
    }

    private void EnableTransition()
    {
        canTransition = true; // Bật lại chuyển scene sau thời gian chờ
    }
}