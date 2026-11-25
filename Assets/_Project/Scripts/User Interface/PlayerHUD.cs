using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class PlayerHUD : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Scrollbar detectionUI;
    [SerializeField] private Scrollbar staminaUI;
    [SerializeField] private Scrollbar senseUI;
    [SerializeField] private GameObject interactionPrompt; // Assign "Press E" UI panel/text here
    [SerializeField] private Image itemIcon;
    

    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private TextMeshProUGUI promptText;       // Optional: To dynamically set text
    [SerializeField] private GameObject playerCrosshair;

    [Header("Values")]
    [SerializeField] private int totalQuota = 10;
    [SerializeField] private int currentItems = 0;
    [SerializeField] private float maxDetection = 1f;

    [Header("Circle Values")]
    [SerializeField] private RectTransform detectionCircle;
    [SerializeField] private CanvasGroup detectionCircleGroup; // on the circle Image
    [SerializeField] private float minCircleScale = 2f;
    [SerializeField] private float maxCircleScale = 16f;

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

        if (staminaUI != null)
        {
            staminaUI.size = 1f;
        }

        if (detectionCircle != null)
        {

            detectionCircle.localScale = new Vector3(16f, 16f, 0f);

        }

        // Hide interaction prompt by default
        ToggleInteractionPrompt(false);
        playerCrosshair.SetActive(true);
    }

    /// <summary>
    /// Shows or hides the interaction UI prompt.
    /// </summary>
    public void ToggleInteractionPrompt(bool show, string message = "'E' to Interact")
    {

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(show);
            promptText.gameObject.SetActive(show);
            playerCrosshair.SetActive(!show);

            Debug.Log($"Prompt {show}\n{System.Environment.StackTrace}", this);
        }

        if (promptText != null && show)
        {
            promptText.text = message;
        }
    }


    public void UpdateDetectionUI(float currentDetection, float maxDetection)
    {
        float t = Mathf.Clamp01(currentDetection / maxDetection);

        // Don't update detection UI if the game is paused
        if (GameManager.Instance.IsPaused)
            return;

        // TODO: More logic is being handled than necessary. Migrate logic back to EnemyBehaviour.cs
        if (detectionUI != null)
        {
            //scrollbar
            detectionUI.size = Mathf.Clamp01(currentDetection / maxDetection);

            //circle
            float scale = Mathf.Lerp(maxCircleScale, minCircleScale, t); // big -> small as t rises
            detectionCircle.localScale = new Vector3(scale, scale, 1f);
            detectionCircleGroup.alpha = t;                              // faint -> opaque as t rises

        }
    }

    // essentially, when it's depleted, there's a two second pause before the float refills
    public void UpdateStaminaDisplay(float currentStamina, float maxStamina)
    {
        if (staminaUI != null)
        {
            staminaUI.size = Mathf.Clamp01(currentStamina / maxStamina);
        }
    }

    public void UpdateSenseDisplay(float currentSense, float maxSense)
    {
        if (senseUI != null)
        {
            senseUI.size = Mathf.Clamp01(currentSense / maxSense);
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

    public void ChangeItemColourIcon()
    {
        itemIcon.color = new Color32(82, 248, 29, 255);
    }
}