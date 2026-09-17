using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    [Header("UI Panels (Assign in Main Game Scene)")]
    [SerializeField] private GameObject pauseInterface;
    [SerializeField] private GameObject gameOverInterface;
    [SerializeField] private GameObject victoryInterface;

    private void OnEnable()
    {
        // Safely subscribe when the scene loads and this UI spawns
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGamePaused += ShowPauseUI;
            GameManager.Instance.OnGameResumed += HidePauseUI;
            GameManager.Instance.OnGameOverTriggered += ShowGameOverUI;
            GameManager.Instance.OnVictoryTriggered += ShowVictoryUI;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe immediately when the scene unloads to prevent memory leaks/null errors
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGamePaused -= ShowPauseUI;
            GameManager.Instance.OnGameResumed -= HidePauseUI;
            GameManager.Instance.OnGameOverTriggered -= ShowGameOverUI;
            GameManager.Instance.OnVictoryTriggered -= ShowVictoryUI;
        }
    }

    private void Start()
    {
        // Ensure panels start disabled on scene load
        if (pauseInterface) pauseInterface.SetActive(false);
        if (gameOverInterface) gameOverInterface.SetActive(false);
        if (victoryInterface) victoryInterface.SetActive(false);
    }

    private void ShowPauseUI()
    {
        if (pauseInterface) pauseInterface.SetActive(true);

        LogHandler.Log("Pause Interface has been activated");
    }

    private void HidePauseUI()
    {
        if (pauseInterface) pauseInterface.SetActive(false);
        LogHandler.Log("Pause Interface has been de-activated");
    }

    private void ShowGameOverUI()
    {
        if (gameOverInterface) gameOverInterface.SetActive(true);
        LogHandler.Log("Game Over UI activated");
    }

    private void ShowVictoryUI()
    {
        if (victoryInterface) victoryInterface.SetActive(true);
        LogHandler.Log("Victory UI activated");
    }
}