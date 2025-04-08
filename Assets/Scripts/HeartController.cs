using UnityEngine;
using UnityEngine.UI;

public class HeartController : MonoBehaviour
{
    PlayerMovement player;

    private GameObject[] heartContainers;
    private Image[] heartFills;
    public Transform heartsParent;
    public GameObject heartContainerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerMovement.Instance;
        heartContainers = new GameObject[PlayerMovement.Instance.maxTotalHealth];
        heartFills = new Image[PlayerMovement.Instance.maxTotalHealth];

        PlayerMovement.Instance.onHealthChangedCallback += UpdateHeartsHUD;
        InstantiateHeartContainers();
        UpdateHeartsHUD();
    }
    // Update is called once per frame
    void Update()
    {

    }

    void InstantiateHeartContainers()
    {
        for (int i = 0; i < PlayerMovement.Instance.maxTotalHealth; i++)
        {
            GameObject temp = Instantiate(heartContainerPrefab);
            temp.transform.SetParent(heartsParent, false);
            heartContainers[i] = temp;
            heartFills[i] = temp.transform.Find("HeartFill").GetComponent<Image>();
        }
    }   
  
    void UpdateHeartsHUD()
    {
        SetHeartContainers();
        SetFilledHearts();
    }

    void SetHeartContainers()
    {
        for (int i = 0; i < heartContainers.Length; i++) // so luong heart UI hien thi
        {
            if (i < PlayerMovement.Instance.maxHealth)
            {
                heartContainers[i].SetActive(true);
            }
            else
            {
                heartContainers[i].SetActive(false);
            }
        }
    }


    void SetFilledHearts()
    {
        for (int i = 0; i < heartFills.Length; i++)
        {
            if (i < PlayerMovement.Instance.Health)
            {
                heartFills[i].fillAmount = 1;
            }
            else
            {
                heartFills[i].fillAmount = 0;
            }
        }
    } 
}
