using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationManager : MonoBehaviour
{
    public static UIAnimationManager Instance { get; private set; }

    [SerializeField] private Animator transitionAnimator;
    private static readonly int FadeOutHash = Animator.StringToHash("End");
    private static readonly int FadeInHash = Animator.StringToHash("Start");

    [SerializeField] private float sceneTransitionTime = 1f;

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

    public IEnumerator PlayFadeOut()
    {
        transitionAnimator.SetTrigger(FadeOutHash);

        // Wait until theAnimator transitions to the fade out state and finishes playing
        yield return null; // Frame delay for Animator state update
        yield return new WaitForSeconds(transitionAnimator.GetCurrentAnimatorStateInfo(0).length);
    }

    public void PlayCrossFadeIn()
    {
        transitionAnimator.SetTrigger(FadeInHash);
    }
}
