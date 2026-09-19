using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCoordinator : MonoBehaviour
{
    public static MenuCoordinator Instance { get; private set; }

    [Header("Transition Settings")]
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private float sceneTransitionTime = 1f;

    [Header("Audio")]
    [SerializeField] private SoundManager musicManager;

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel; // Reference your Main Menu panel here

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

    private void Update()
    {

        Instance = this;

        // I'M DESPERATE. I WILL JUST HARD CODE THIS FOR NOW.
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
        {
            LogHandler.Log($"This is the Main Menu Scene, indexed at {0}, turning on  Menu Panel!");

            if(Input.GetKeyDown(KeyCode.M))
            {
                mainMenuPanel.SetActive(true);
                LogHandler.Log("Menu turned on!");

            }
        }
    }

    public void LoadMainGame()
    {
        LogHandler.Log("Loading into Main Game Scene");

        Time.timeScale = 1f;
        StartCoroutine(LoadLevelRoutine(1));

        if (musicManager != null && musicManager.TryGetComponent(out AudioSource source))
        {
            source.enabled = true;
        }
    }

    public void LoadMenu()
    {
        mainMenuPanel.SetActive(true);
        LogHandler.Log("Loading into Main Menu Scene");

        Time.timeScale = 1f;
        StartCoroutine(LoadLevelRoutine(0));

        if (musicManager != null && musicManager.TryGetComponent(out AudioSource source))
        {
            source.enabled = false;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        LogHandler.Log("Application closed.");
    }

    private IEnumerator LoadLevelRoutine(int levelIndex)
    {
        // 1. If we are leaving the menu to go to gameplay, hide the main menu panel right away
        if (levelIndex != 0 && mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);

            LogHandler.Log("Heading into Game Scene. Hiding Menu Panel");
        }

        CanvasGroup canvasGroup = transitionAnimator != null ? transitionAnimator.GetComponent<CanvasGroup>() : null;
        if (canvasGroup != null) canvasGroup.blocksRaycasts = true;

        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("Start");
        }

        yield return new WaitForSecondsRealtime(sceneTransitionTime);

        SceneManager.LoadScene(levelIndex);

        // 2. If we are returning to the Main Menu scene (Index 0), make sure the panel turns back on
        if (levelIndex == 0 && mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);

            LogHandler.Log("Heading into Main Menu Scene. Showing Menu Panel");
        }

        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("End");
        }

        if (canvasGroup != null) canvasGroup.blocksRaycasts = false;
    }



}