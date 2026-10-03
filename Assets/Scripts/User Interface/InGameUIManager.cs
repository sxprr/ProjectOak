using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    [Header("UI Panels (Assign in Main Game Scene)")]
    [SerializeField] private GameObject pauseInterface;
    [SerializeField] private GameObject gameOverInterface;
    [SerializeField] private GameObject victoryInterface;

    private void OnEnable()
    {
        LogHandler.Log("UI Methods have been subscribed");

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
        LogHandler.Log("UI Methods have been unsubscribed");

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

    public void HidePauseUI()
    {
        if (pauseInterface) pauseInterface.SetActive(false);
        LogHandler.Log("Pause Interface has been de-activated");
    }

    private void ShowGameOverUI()
    {
        if (gameOverInterface) gameOverInterface.SetActive(true);
        LogHandler.Log("Game Over UI activated");
    }

    public void HideGameOverUI()
    {
        if (gameOverInterface) gameOverInterface.SetActive(false);
        LogHandler.Log("Game Over UI de-activated");
    }


    private void ShowVictoryUI()
    {
        if (victoryInterface) victoryInterface.SetActive(true);
        LogHandler.Log("Victory UI activated");
    }

    public void HideVictoryUI()
    {
        if (victoryInterface) victoryInterface.SetActive(false);
        LogHandler.Log("Victory UI de-activated");
    }
}