using System.Collections;
using UnityEngine;

public class UnlockWallJump : MonoBehaviour
{
    bool used;
    [SerializeField] GameObject canvasUI;
    [SerializeField] GameObject particles;
    private void Start()
    {
        if (PlayerMovement.Instance.unlockedWallJump)
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !used)
        {
            used = true;
            StartCoroutine(ShowUI());
        }
    }
    IEnumerator ShowUI()
    {
        GameObject _particles = Instantiate(particles, transform.position, Quaternion.identity);
        Destroy(_particles, 0.5f);
        yield return new WaitForSeconds(0.5f);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;

        canvasUI.SetActive(true);

        yield return new WaitForSeconds(4f);
        PlayerMovement.Instance.unlockedWallJump = true;
        SaveData.Instance.SavePlayerData();
        canvasUI.SetActive(false);
        Destroy(gameObject);
    }
}
