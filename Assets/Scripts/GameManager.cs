﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string transitionedFromScene;

    public Vector2 platformingRespawnPoint;
    public Vector2 respawnPoint;
    [SerializeField] Vector2 defaultRespawnPoint;
    [SerializeField] Bench bench;

    public GameObject shade;

    public bool Boss_Defeated = false;

    [SerializeField] FadeUI pauseMenu;
    [SerializeField] float fadeTime;
    public bool gameIsPaused;

    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        SaveData.Instance.Initialize();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        if (PlayerMovement.Instance != null)
        {
            if (PlayerMovement.Instance.halfMana)
            {
                SaveData.Instance.LoadShadeData();
                if (SaveData.Instance.sceneWithShade == SceneManager.GetActiveScene().name || SaveData.Instance.sceneWithShade == "")
                {
                    Instantiate(shade, SaveData.Instance.shadePos, SaveData.Instance.shadeRot);
                }
            }
        }
        SaveScene();
        DontDestroyOnLoad(gameObject);
        bench = FindObjectOfType<Bench>();

        SaveData.Instance.LoadBossData();
    }

    private void Update()
    {
        //test save game
        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveData.Instance.SavePlayerData();
            Debug.Log("SavePlayerData");
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !gameIsPaused)
        {
            pauseMenu.FadeUIIn(fadeTime);
            Time.timeScale = 0;
            gameIsPaused = true;
        }
    }
    public void UnPauseGame()
    {
        gameIsPaused = false;
        Time.timeScale = 1;
    }
    public void SaveGame()
    {
        SaveData.Instance.SavePlayerData();
    }

    public void SaveScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SaveData.Instance.sceneNames.Add(currentSceneName);
    }
    public void RespawnPlayer()
    {
        SaveData.Instance.LoadBench();
        if (SaveData.Instance.benchSceneName != null) //load the bench's scene if it exists.
        {
            SceneManager.LoadScene(SaveData.Instance.benchSceneName);
        }

        if (SaveData.Instance.benchPos != null) //set the respawn point to the bench's position.
        {
            respawnPoint = SaveData.Instance.benchPos;
        }
        else
        {
            respawnPoint = defaultRespawnPoint;
        }

        PlayerMovement.Instance.transform.position = respawnPoint;
        StartCoroutine(UIManager.Instance.DeactivateDeathScreen());
        PlayerMovement.Instance.Respawned();
    }
}