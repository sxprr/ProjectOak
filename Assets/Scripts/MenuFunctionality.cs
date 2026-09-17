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

    private void Awake()
    {
        /*

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        */
        
        

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void LoadMainGame()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadLevelRoutine(1));
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadLevelRoutine(0));
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



}