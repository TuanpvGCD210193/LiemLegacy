using UnityEngine;

public class Bench : MonoBehaviour
{
    bool inRange = false;
    public bool interacted = false;
    // Update is called once per frame
    void Update()
    {
        //if (Input.GetButtonDown("Interact") && inRange) interacted = true;
        if (Input.GetButtonDown("Interact") && inRange)
        {
            if (GameManager.Instance.lastInteractedBench != null)
            {
                GameManager.Instance.lastInteractedBench.interacted = false;
            }

            interacted = true;
            GameManager.Instance.lastInteractedBench = this;
            GameManager.Instance.SetRespawnPoint(transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player")) inRange = true;
    }

    private void OnTriggerExit2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player")) inRange = false;
    }
}
