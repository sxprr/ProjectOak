using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //[Header("Game States")]
    
    public bool IsPaused { get; private set; }
    public bool IsGameOver { get; private set; }

    [Header("UI Events (Inspector Dynamic Wiring)")]
    public UnityEvent OnGamePaused;
    public UnityEvent OnGameResumed;
    public UnityEvent OnGameOverTriggered;
    public UnityEvent OnVictoryTriggered;

    private void Awake()
    {

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
            // Disable pausing in the Main Menu scene (Build Index 0)
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
        OnGamePaused?.Invoke();

        LogHandler.Log("<color=yellow>[GameManager]</color> Game paused.");

    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        IsPaused = false;

        SetCursorState(visible: false, locked: true);
        OnGameResumed?.Invoke();

        LogHandler.Log("<color=green>[GameManager]</color> Game restored.");
    }

    public void TriggerGameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        Time.timeScale = 0f;

        SetCursorState(visible: true, locked: false);
        OnGameOverTriggered?.Invoke();

        LogHandler.Log("<color=red>[GameManager]</color> Game Over triggered.");
    }

    public void TriggerVictory()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        Time.timeScale = 0f;

        SetCursorState(visible: true, locked: false);
        OnVictoryTriggered?.Invoke();

        LogHandler.Log("<color=cyan>[GameManager]</color> Victory triggered!");
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

