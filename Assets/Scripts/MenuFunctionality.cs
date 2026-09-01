using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MenuFunctionality : MonoBehaviour
{
    public static MenuFunctionality Instance { get; private set; }

    [Header("Transition Settings")]
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private float sceneTransitionTime = 1f;

    [Header("Audio")]
    [SerializeField] private SoundManager musicManager;

    [Header("UI Events (Inspector Dynamic Wiring)")]
    public UnityEvent OnGamePaused;
    public UnityEvent OnGameResumed;
    public UnityEvent OnGameOverTriggered;
    public UnityEvent OnVictoryTriggered;

    public bool IsPaused { get; private set; }
    public static bool IsGameOver { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicManager != null)
        {
            DontDestroyOnLoad(musicManager.gameObject);
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

    private void Start()
    {
        ResetGameState();
    }

    private void Update()
    {
        if (IsGameOver) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Disable pausing in the Main Menu scene (Build Index 0)
            if (SceneManager.GetActiveScene().buildIndex == 0) return;

            
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

        LogHandler.Log("<color=yellow>[Pause]</color> Game paused.");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        IsPaused = false;

        SetCursorState(visible: false, locked: true);
        OnGameResumed?.Invoke();

        LogHandler.Log("<color=green>[Resume]</color> Game restored.");
    }

    public void DisplayGameOver()
    {
        IsGameOver = true;
        Time.timeScale = 0f;

        SetCursorState(visible: true, locked: false);
        OnGameOverTriggered?.Invoke();
    }

    public void DisplayVictory()
    {
        IsGameOver = true;
        Time.timeScale = 0f;

        SetCursorState(visible: true, locked: false);
        OnVictoryTriggered?.Invoke();
    }

    public void LoadMainGame()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadLevelRoutine(1));

        if (musicManager != null && musicManager.TryGetComponent(out AudioSource source))
        {
            source.enabled = true;
        }
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadLevelRoutine(0));

        if (musicManager != null && musicManager.TryGetComponent(out AudioSource source))
        {
            source.enabled = false;
        }
    }

    private IEnumerator LoadLevelRoutine(int levelIndex)
    {
        CanvasGroup canvasGroup = transitionAnimator != null ? transitionAnimator.GetComponent<CanvasGroup>() : null;
        if (canvasGroup != null) canvasGroup.blocksRaycasts = true;

        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("Start");
        }

        yield return new WaitForSecondsRealtime(sceneTransitionTime);

        SceneManager.LoadScene(levelIndex);

        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("End");
        }

        if (canvasGroup != null) canvasGroup.blocksRaycasts = false;
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