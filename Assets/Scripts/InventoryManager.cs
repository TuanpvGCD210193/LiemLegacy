using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{

    [SerializeField] Image heartShard;
    [SerializeField] Image manaShard;
    [SerializeField] GameObject upCast, sideCast, downCast;
    [SerializeField] GameObject dash, varJump, wallJump;

    private void OnEnable()
    {
        //heart shard
        heartShard.fillAmount = PlayerMovement.Instance.heartShards * 0.25f;

        //mana shards
        manaShard.fillAmount = PlayerMovement.Instance.orbShard * 0.34f;


        //spells
        if (PlayerMovement.Instance.unlockedUpCast)
        {
            upCast.SetActive(true);
        }
        else
        {
            upCast.SetActive(false);
        }
        if (PlayerMovement.Instance.unlockedSideCast)
        {
            sideCast.SetActive(true);
        }
        else
        {
            sideCast.SetActive(false);
        }
        if (PlayerMovement.Instance.unlockedDownCast)
        {
            downCast.SetActive(true);
        }
        else
        {
            downCast.SetActive(false);
        }

        //abilities
        if (PlayerMovement.Instance.unlockedDash)
        {
            dash.SetActive(true);
        }
        else
        {
            dash.SetActive(false);
        }
        if (PlayerMovement.Instance.unlockedVarJump)
        {
            varJump.SetActive(true);
        }
        else
        {
            varJump.SetActive(false);
        }
        if (PlayerMovement.Instance.unlockedWallJump)
        {
            wallJump.SetActive(true);
        }
        else
        {
            wallJump.SetActive(false);
        }
    }
}