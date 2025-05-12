using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;


[System.Serializable]

public struct SaveData
{
    public static SaveData Instance;

    //map stuff
    public HashSet<string> sceneNames;
    //bench stuff
    public string benchSceneName;
    public Vector2 benchPos;
    //player stuff
    public int playerHealth;
    public int playerMaxHealth;// increse max health
    public int playerHeartShards;// increse max health

    public float playerMana;
    public int playerManaOrbs;
    public int playerOrbShard;
    public float playerOrb0fill, playerOrb1fill, playerOrb2fill;
    public bool playerHalfMana;

    public Vector2 playerPosition;
    public string lastScene;

    public bool playerUnlockedWallJump, playerUnlockedDash, playerUnlockedVarJump;
    public bool playerUnlockedSideCast, playerUnlockedUpCast, playerUnlockedDownCast;

    //enemies stuff
    //shade
    public Vector2 shadePos;
    public string sceneWithShade;
    public Quaternion shadeRot;

    public bool Boss_Defeated;


    public void Initialize()
    {
        if (!File.Exists(Application.persistentDataPath + "/save.bench.data")) //if file doesnt exist, well create the file
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.bench.data"));
        }
        if (!File.Exists(Application.persistentDataPath + "/save.player.data")) //if file doesnt exist, well create the file
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.player.data"));
        }
        if (!File.Exists(Application.persistentDataPath + "/save.shade.data")) //if file doesnt exist, well create the file
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.shade.data"));
        }
        if (sceneNames == null)
        {
            sceneNames = new HashSet<string>();
        }
    }
    #region Bench Stuff
    public void SaveBench()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.bench.data")))
        {
            writer.Write(benchSceneName);
            writer.Write(benchPos.x);
            writer.Write(benchPos.y);
        }
    }
    public void LoadBench()
    {
        if (File.Exists(Application.persistentDataPath + "/save.bench.data"))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.bench.data")))
            {
                benchSceneName = reader.ReadString();
                benchPos.x = reader.ReadSingle();
                benchPos.y = reader.ReadSingle();
            }
        }
        else
        {
            Debug.Log("Bench doesnt exist");
        }
    }
    #endregion
    #region Player stuff
    public void SavePlayerData()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.player.data")))
        {
            playerHealth = PlayerMovement.Instance.Health;
            writer.Write(playerHealth);
            //Savin player health
            playerMaxHealth = PlayerMovement.Instance.maxHealth;
            writer.Write(playerMaxHealth);
            playerHeartShards = PlayerMovement.Instance.heartShards;
            writer.Write(playerHeartShards);
            //
            playerMana = PlayerMovement.Instance.Mana;
            writer.Write(playerMana);
            playerHalfMana = PlayerMovement.Instance.halfMana;
            writer.Write(playerHalfMana);
            playerManaOrbs = PlayerMovement.Instance.manaOrbs;
            writer.Write(playerManaOrbs);
            playerOrbShard = PlayerMovement.Instance.orbShard;
            writer.Write(playerOrbShard);
            playerOrb0fill = PlayerMovement.Instance.manaOrbsHandler.orbFills[0].fillAmount;
            writer.Write(playerOrb0fill);
            playerOrb1fill = PlayerMovement.Instance.manaOrbsHandler.orbFills[1].fillAmount;
            writer.Write(playerOrb1fill);
            playerOrb2fill = PlayerMovement.Instance.manaOrbsHandler.orbFills[2].fillAmount;
            writer.Write(playerOrb2fill);
            //
            playerUnlockedWallJump = PlayerMovement.Instance.unlockedWallJump;//unlock wall jump
            writer.Write(playerUnlockedWallJump);//unlock wall jump

            playerUnlockedDash = PlayerMovement.Instance.unlockedDash;//unlock dash
            writer.Write(playerUnlockedDash);//unlock dash
            playerUnlockedVarJump = PlayerMovement.Instance.unlockedVarJump;//unlock dash
            writer.Write(playerUnlockedVarJump);//unlock dash

            playerUnlockedSideCast = PlayerMovement.Instance.unlockedSideCast;
            writer.Write(playerUnlockedSideCast);
            playerUnlockedUpCast = PlayerMovement.Instance.unlockedUpCast;
            writer.Write(playerUnlockedUpCast);
            playerUnlockedDownCast = PlayerMovement.Instance.unlockedDownCast;
            writer.Write(playerUnlockedDownCast);

            playerPosition = PlayerMovement.Instance.transform.position;
            writer.Write(playerPosition.x);
            writer.Write(playerPosition.y);

            lastScene = SceneManager.GetActiveScene().name;
            writer.Write(lastScene);
        }
        Debug.Log("saved player data");
    }
    public void LoadPlayerData()
    {
        if (File.Exists(Application.persistentDataPath + "/save.player.data"))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.player.data")))
            {
                playerHealth = reader.ReadInt32();
                playerMaxHealth = reader.ReadInt32();
                playerHeartShards = reader.ReadInt32();

                playerMana = reader.ReadSingle();
                playerHalfMana = reader.ReadBoolean();
                //
                playerManaOrbs = reader.ReadInt32();
                playerOrbShard = reader.ReadInt32();
                playerOrb0fill = reader.ReadSingle();
                playerOrb1fill = reader.ReadSingle();
                playerOrb2fill = reader.ReadSingle();
                //

                playerUnlockedWallJump = reader.ReadBoolean();// Unlocking Abilities
                playerUnlockedDash = reader.ReadBoolean();//unlock dash
                playerUnlockedVarJump = reader.ReadBoolean();

                playerUnlockedSideCast = reader.ReadBoolean();
                playerUnlockedUpCast = reader.ReadBoolean();
                playerUnlockedDownCast = reader.ReadBoolean();

                playerPosition.x = reader.ReadSingle();
                playerPosition.y = reader.ReadSingle();
                lastScene = reader.ReadString();

                SceneManager.LoadScene(lastScene);
                PlayerMovement.Instance.transform.position = playerPosition;
                PlayerMovement.Instance.halfMana = playerHalfMana;
                PlayerMovement.Instance.Health = playerHealth;
                PlayerMovement.Instance.maxHealth = playerMaxHealth;
                PlayerMovement.Instance.heartShards = playerHeartShards;
                PlayerMovement.Instance.Mana = playerMana;
                //
                PlayerMovement.Instance.manaOrbs = playerManaOrbs;
                PlayerMovement.Instance.orbShard = playerOrbShard;
                PlayerMovement.Instance.manaOrbsHandler.orbFills[0].fillAmount = playerOrb0fill;
                PlayerMovement.Instance.manaOrbsHandler.orbFills[1].fillAmount = playerOrb1fill;
                PlayerMovement.Instance.manaOrbsHandler.orbFills[2].fillAmount = playerOrb2fill;
                //

                PlayerMovement.Instance.unlockedWallJump = playerUnlockedWallJump;// Unlocking Abilities
                PlayerMovement.Instance.unlockedDash = playerUnlockedDash;//unlock dash
                PlayerMovement.Instance.unlockedVarJump = playerUnlockedVarJump;

                PlayerMovement.Instance.unlockedSideCast = playerUnlockedSideCast;
                PlayerMovement.Instance.unlockedUpCast = playerUnlockedUpCast;
                PlayerMovement.Instance.unlockedDownCast = playerUnlockedDownCast;
            }
            Debug.Log("load player data");
            Debug.Log(playerHalfMana);
        }
        else
        {
            Debug.Log("File doesnt exist");
            PlayerMovement.Instance.halfMana = false;
            PlayerMovement.Instance.Health = PlayerMovement.Instance.maxHealth;
            PlayerMovement.Instance.Mana = 0.5f;
            PlayerMovement.Instance.heartShards = 0;

            PlayerMovement.Instance.unlockedWallJump = false;
            PlayerMovement.Instance.unlockedDash = false;
            PlayerMovement.Instance.unlockedVarJump = false;

            PlayerMovement.Instance.unlockedSideCast = false;
            PlayerMovement.Instance.unlockedUpCast = false;
            PlayerMovement.Instance.unlockedDownCast = false;
        }
    }

    #endregion
    #region enemy stuff
    public void SaveShadeData()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.shade.data")))
        {
            sceneWithShade = SceneManager.GetActiveScene().name;
            shadePos = Shade.Instance.transform.position;
            shadeRot = Shade.Instance.transform.rotation;

            writer.Write(sceneWithShade);

            writer.Write(shadePos.x);
            writer.Write(shadePos.y);

            writer.Write(shadeRot.x);
            writer.Write(shadeRot.y);
            writer.Write(shadeRot.z);
            writer.Write(shadeRot.w);
        }
    }
    public void LoadShadeData()
    {
        if (File.Exists(Application.persistentDataPath + "/save.shade.data"))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.shade.data")))
            {
                sceneWithShade = reader.ReadString();
                shadePos.x = reader.ReadSingle();
                shadePos.y = reader.ReadSingle();

                float rotationX = reader.ReadSingle();
                float rotationY = reader.ReadSingle();
                float rotationZ = reader.ReadSingle();
                float rotationW = reader.ReadSingle();
                shadeRot = new Quaternion(rotationX, rotationY, rotationZ, rotationW);
            }
            Debug.Log("Load shade data");
        }
        else
        {
            Debug.Log("Shade doesnt exist");
        }
    }
    #endregion

    public void SaveBossData()
    {
        if (!File.Exists(Application.persistentDataPath + "/save.boss.data")) //if file doesnt exist, well create the file
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.boss.data"));
        }

        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.boss.data")))
        {
            Boss_Defeated = GameManager.Instance.Boss_Defeated;

            writer.Write(Boss_Defeated);
        }
    }

    public void LoadBossData()
    {
        if (File.Exists(Application.persistentDataPath + "/save.Boss.data"))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.boss.data")))
            {
                Boss_Defeated = reader.ReadBoolean();

                GameManager.Instance.Boss_Defeated = Boss_Defeated;
            }
        }
        else
        {
            Debug.Log("Boss doesnt exist");
        }
    }
}