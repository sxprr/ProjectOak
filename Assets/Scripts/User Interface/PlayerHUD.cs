using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class PlayerHUD : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Scrollbar detectionUI;
    [SerializeField] private Scrollbar staminaUI;
    [SerializeField] private GameObject interactionPrompt; // Assign "Press E" UI panel/text here
    [SerializeField] private Image itemIcon;
    [SerializeField] private RawImage detectionCircle;

    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private TextMeshProUGUI promptText;       // Optional: To dynamically set text
    [SerializeField] private GameObject playerCrosshair;
    
    [Header("Values")]
    [SerializeField] private int totalQuota = 10;
    [SerializeField] private int currentItems = 0;
    [SerializeField] private float maxDetection = 1f;

    private Vector3 minCircleScale = new Vector3(2f, 2f, 2f);
    private Vector3 maxCircleScale = new Vector3(16f, 16f, 16f);

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

            detectionCircle.rectTransform.localScale = maxCircleScale;

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
        }

        if (promptText != null && show)
        {
            promptText.text = message;
        }
    }

    public void UpdateDetection(float detectionAmount)
    {
        // Don't update detection UI if the game is paused
        if (GameManager.Instance.IsPaused)
            return;

        if (detectionUI != null)
        {
            //this circle detection needs to pause when the enemy loses contact.
            float circleDetectionAmount = 0.0140f;

            detectionUI.size = Mathf.Clamp01(detectionUI.size + detectionAmount);
            detectionCircle.rectTransform.localScale -= new Vector3(circleDetectionAmount, circleDetectionAmount, 0f);
            
            
            if (detectionUI.size >= maxDetection)
            {
                LogHandler.Log($"Detection number has reached {detectionUI.size}, Game Over!");
                GameManager.Instance.TriggerGameOver();

                // The logic is that when we die, the screen will have red detection circle.
                // Similar to many fps games where you take damage and you see a red outline
                detectionCircle.rectTransform.localScale = minCircleScale;
            }
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

    public void ChangeItemColourIcon()
    {
        itemIcon.color = new Color32(82, 248, 29, 255);
    }
}