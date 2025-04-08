using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaOrbsHandler : MonoBehaviour
{

    public bool usedMana;
    public List<GameObject> manaOrbs;
    public List<Image> orbFills;

    public float countDown = 3f;
    float totalManaPool;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < PlayerMovement.Instance.manaOrbs; i++)
        {
            manaOrbs[i].SetActive(true);
        }

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < PlayerMovement.Instance.manaOrbs; i++)
        {
            manaOrbs[i].SetActive(true);
        }
        CashInMana();
    }
    public void UpdateMana(float _manaGainFrom)
    {
        for (int i = 0; i < manaOrbs.Count; i++)
        {
            if (manaOrbs[i].activeInHierarchy && orbFills[i].fillAmount < 1)
            {
                orbFills[i].fillAmount += _manaGainFrom;
                break;
            }
        }
    }
    void CashInMana()
    {
        if (usedMana && PlayerMovement.Instance.Mana <= 1)
        {
            countDown -= Time.deltaTime;
        }

        if (countDown <= 0)
        {
            usedMana = false;
            countDown = 3;

            totalManaPool = (orbFills[0].fillAmount += orbFills[1].fillAmount += orbFills[2].fillAmount) * 0.33f;
            float manaNeeded = 1 - PlayerMovement.Instance.Mana;

            if (manaNeeded > 0)
            {
                if (totalManaPool >= manaNeeded)
                {
                    PlayerMovement.Instance.Mana += manaNeeded;
                    for (int i = 0; i < orbFills.Count; i++)
                    {
                        orbFills[i].fillAmount = 0;
                    }

                    float addBackTotal = (totalManaPool - manaNeeded) / 0.33f;
                    while (addBackTotal > 0)
                    {
                        UpdateMana(addBackTotal);
                        addBackTotal -= 1;
                    }
                }
                else
                {
                    PlayerMovement.Instance.Mana += totalManaPool;
                    for (int i = 0; i < orbFills.Count; i++)
                    {
                        orbFills[i].fillAmount = 0;
                    }
                }
            }
        }
    }
}
