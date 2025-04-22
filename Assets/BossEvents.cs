using System.Collections;
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

    void Parrying()
    {
        Boss.Instance.parrying = true;
    }

    void BendDownCheck()
    {
        if (Boss.Instance.barrageAttack)
        {
            StartCoroutine(BarrageAttackTransition());
        }

        if (Boss.Instance.outbreakAttack)
        {
            StartCoroutine(OutbreakAttackTransition());
        }
        if (Boss.Instance.bounceAttack)
        {
            Boss.Instance.anim.SetTrigger("Bounce1");
        }
    }

    void BarrageOrOutbreak()
    {
        if (Boss.Instance.barrageAttack)
        {
            Boss.Instance.StartCoroutine(Boss.Instance.Barrage());
        }
        if (Boss.Instance.outbreakAttack)
        {
            Boss.Instance.StartCoroutine(Boss.Instance.Outbreak());
        }
    }

    IEnumerator OutbreakAttackTransition()
    {
        yield return new WaitForSeconds(1f);
        Boss.Instance.anim.SetBool("Cast", true);
    }

    IEnumerator BarrageAttackTransition()
    {
        yield return new WaitForSeconds(1f);
        Boss.Instance.anim.SetBool("Cast", true);
    }

    void DestroyAfterDeath()
    {
        Boss.Instance.DestroyAfterDeath();
    }

}
