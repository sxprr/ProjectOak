using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class PlayerHUD : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Scrollbar detectionUI;
    [SerializeField] private float maxDetection = 1f;
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private GameObject interactionPrompt; // Assign "Press E" UI panel/text here
    [SerializeField] private TextMeshProUGUI promptText;       // Optional: To dynamically set text
    [SerializeField] private int totalQuota = 10;
    [SerializeField] private int currentItems = 0;

    [Header("Events")]
    public UnityEvent onCaught;

    private void Start()
    {
        currentItems = 0;

        if (detectionUI == null)
        {
            detectionUI = GetComponentInChildren<Scrollbar>();
        }

        if (detectionUI != null)
        {
            detectionUI.size = 0f;
        }

        // Hide interaction prompt by default
        ToggleInteractionPrompt(false);
    }

    /// <summary>
    /// Shows or hides the interaction UI prompt.
    /// </summary>
    public void ToggleInteractionPrompt(bool show, string message = "Press E to Interact")
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(show);
        }

        if (promptText != null && show)
        {
            promptText.text = message;
        }
    }

    public void UpdateDetection(float detectionAmount)
    {
        if (detectionUI != null)
        {
            detectionUI.size = Mathf.Clamp01(detectionUI.size + detectionAmount);

            if (detectionUI.size >= maxDetection)
            {
                onCaught.Invoke();
            }
        }
    }

    public void SubtractDetection(float detectionAmount)
    {
        if (detectionUI != null)
        {
            detectionUI.size = Mathf.Clamp01(detectionUI.size - detectionAmount);
        }
    }

    public void UpdateItemQuota(int itemsToAdd = 1)
    {
        currentItems += itemsToAdd;

        if (itemText != null)
        {
            itemText.text = $"{currentItems}/{totalQuota}";
        }
    }
}