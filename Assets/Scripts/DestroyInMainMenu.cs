using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyInMainMenu : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Destroy(gameObject);
        }
    }
}