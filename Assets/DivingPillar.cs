using UnityEngine;

public class DivingPillar : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player") && !PlayerMovement.Instance.playerState.invincible)
        {
            _other.GetComponent<PlayerMovement>().TakeDamage(Boss.Instance.damage);
            if (PlayerMovement.Instance.playerState.alive)
            {
                PlayerMovement.Instance.HitStopTime(0, 5, 0.5f);
            }
        }
    }
}