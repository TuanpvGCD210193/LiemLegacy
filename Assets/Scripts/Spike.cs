using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spike : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            StartCoroutine(RespawnPoint());
        }
    }

    IEnumerator RespawnPoint()
    {
        PlayerMovement.Instance.playerState.cutscene = true;
        PlayerMovement.Instance.playerState.invincible = true;
        PlayerMovement.Instance.rb.linearVelocity = Vector2.zero;
        Time.timeScale = 0f;
        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.In));
        PlayerMovement.Instance.TakeDamage(1);
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1;
        PlayerMovement.Instance.transform.position = GameManager.Instance.platformingRespawnPoint;
        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
        yield return new WaitForSecondsRealtime(UIManager.Instance.sceneFader.fadeTime);
        PlayerMovement.Instance.playerState.cutscene = false;
        PlayerMovement.Instance.playerState.invincible = false;
    }
}
