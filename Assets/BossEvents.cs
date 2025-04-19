using UnityEngine;

public class BossEvents : MonoBehaviour
{

    void SlashDamagePlayer()
    {
        if (PlayerMovement.Instance.transform.position.x - transform.position.x != 0)
        {
            Hit(Boss.Instance.SideAttackTransform, Boss.Instance.SideAttackArea);
        }
        else if (PlayerMovement.Instance.transform.position.y > transform.position.y)
        {
            Hit(Boss.Instance.UpAttackTransform, Boss.Instance.UpAttackArea);
        }
        else if (PlayerMovement.Instance.transform.position.y < transform.position.y)
        {
            Hit(Boss.Instance.DownAttackTransform, Boss.Instance.DownAttackArea);
        }
    }

    void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] _objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0);

        for (int i = 0; i < _objectsToHit.Length; i++)
        {
            if (_objectsToHit[i].GetComponent<PlayerMovement>() != null && !PlayerMovement.Instance.playerState.invincible)
            {
                _objectsToHit[i].GetComponent<PlayerMovement>().TakeDamage(Boss.Instance.damage);
                if (PlayerMovement.Instance.playerState.alive)
                {
                    PlayerMovement.Instance.HitStopTime(0, 5, 0.5f);
                }
            }
        }
    }

}
