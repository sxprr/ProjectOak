using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MenuFunctionality : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private float sceneTransitionTime = 1f;
  

    private void Awake()
    {
  
    }


    public void LoadMainGame()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadLevelRoutine(1));
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;

        if (MenuCoordinator.Instance != null)
        {
            MenuCoordinator.Instance.LoadMenu();
            
        }
        else
        {
            LogHandler.Log("MenuCoordinator instance not found when trying to load menu.");
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



}