using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCoordinator : MonoBehaviour
{
    public static MenuCoordinator Instance { get; private set; }

    [Header("Transition Settings")]
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private float sceneTransitionTime = 1f;

    [Header("UI Panels")]
    [SerializeField] public GameObject mainMenuPanel; // Reference your Main Menu panel here

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

    private void Update()
    {
        Instance = this;
    }

    public void LoadMainGame()
    {
        LogHandler.Log("Loading into Main Game Scene");

        Time.timeScale = 1f;
        StartCoroutine(LoadLevelRoutine(1));

    }

    public void LoadMenu()
    {
        mainMenuPanel.SetActive(true);
        LogHandler.Log("Loading into Main Menu Scene");

        Time.timeScale = 1f;
        StartCoroutine(LoadLevelRoutine(0));
    }

    public void QuitGame()
    {
        Application.Quit();
        LogHandler.Log("Application closed.");
    }

    private IEnumerator LoadLevelRoutine(int levelIndex)
    {
        // 1. If leaving menu, hide panel before fading out
        if (levelIndex != 0 && mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
            LogHandler.Log("Heading into Game Scene. Hiding Menu Panel");
        }

        yield return new WaitForSecondsRealtime(sceneTransitionTime);

        // 1. Trigger transition out and yield until finished
        yield return UIAnimationManager.Instance.PlayFadeOut();

        // 2. Load the scene asynchronously and wait until fully loaded
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelIndex);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 3. Re-enable panel AFTER the Main Menu scene has finished loading
        if (levelIndex == 0 && mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
            LogHandler.Log("Main Menu Scene Loaded. Showing Menu Panel");
        }
    }

}