using System.Collections;
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

    // The MenCoOrdinator.LoadMainGame calls this method.
    public IEnumerator PlayFadeOut()
    {
        if (transitionAnimator == null)
            yield break;

        transitionAnimator.ResetTrigger(FadeInHash);
        transitionAnimator.SetTrigger(FadeOutHash);

        // Wait until the Animator actually enters the target state
        // Replace "EndStateName" with the exact state name in your Animator controller
        yield return null;
        while (!transitionAnimator.IsInTransition(0) &&
               !transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName("Crossfade_start"))
        {
            yield return null;
        }

        // Wait through any transition blend into the state
        while (transitionAnimator.IsInTransition(0))
        {
            yield return null;
        }

        float clipLength = transitionAnimator.GetCurrentAnimatorStateInfo(0).length;
        if (clipLength > 0f)
        {
            yield return new WaitForSecondsRealtime(clipLength);
        }
    }

    public void PlayCrossFadeIn()
    {
        transitionAnimator.SetTrigger(FadeInHash);
    }
}
