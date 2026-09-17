using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    
    public bool IsPaused { get; private set; }
    public bool IsGameOver { get; private set; }

    // C# Events for code-based UI subscription
    public event Action OnGamePaused;
    public event Action OnGameResumed;
    public event Action OnGameOverTriggered;
    public event Action OnVictoryTriggered;

    int MainMenuScene = 0;
    int MainGameScene = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Check current scene.
        if(SceneManager.GetActiveScene() ==  SceneManager.GetSceneByBuildIndex(MainMenuScene))
        {
            LogHandler.Log($"This is the Main Menu Scene, indexed at {MainMenuScene}");
        }
        else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(MainMenuScene))
        {
            LogHandler.Log($"This is the Main Game Scene, indexd at {MainGameScene}");
        }
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {



        if (IsGameOver) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Do not allow pausing in the Main Menu scene (Build Index 0)
            if (SceneManager.GetActiveScene().buildIndex == 0) return;

            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (IsPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        IsPaused = true;
        SetCursorState(visible: true, locked: false);

        OnGamePaused?.Invoke(); // Fires the event to all listening UI
        LogHandler.Log("<color=yellow>[GameManager]</color> Game paused.");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SetCursorState(visible: false, locked: true);

        OnGameResumed?.Invoke(); // Fires the event
        LogHandler.Log("<color=green>[GameManager]</color> Game resumed.");
    }

    public void TriggerGameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        Time.timeScale = 0f;
        SetCursorState(visible: true, locked: false);

        OnGameOverTriggered?.Invoke();
    }

    public void TriggerVictory()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        Time.timeScale = 0f;
        SetCursorState(visible: true, locked: false);

        OnVictoryTriggered?.Invoke();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetGameState();
    }

    private void ResetGameState()
    {
        IsGameOver = false;
        IsPaused = false;
        Time.timeScale = 1f;

        bool isMenuScene = SceneManager.GetActiveScene().buildIndex == 0;
        SetCursorState(visible: isMenuScene, locked: !isMenuScene);

        LogHandler.Log("The Game Scene has been reset");
    }

    private void SetCursorState(bool visible, bool locked)
    {
        Cursor.visible = visible;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void Quit()
    {
        Application.Quit();
        LogHandler.Log("Application closed.");
    }
}

