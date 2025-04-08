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
    public float playerMana;
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
            playerMana = PlayerMovement.Instance.Mana;
            writer.Write(playerMana);
            playerHalfMana = PlayerMovement.Instance.halfMana;
            writer.Write(playerHalfMana);

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
                playerMana = reader.ReadSingle();
                playerHalfMana = reader.ReadBoolean();

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
                PlayerMovement.Instance.Mana = playerMana;

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
}