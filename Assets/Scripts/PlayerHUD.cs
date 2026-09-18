using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class PlayerHUD : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Scrollbar detectionUI;
    [SerializeField] private Scrollbar staminaUI;
    [SerializeField] private GameObject interactionPrompt; // Assign "Press E" UI panel/text here
    [SerializeField] private Image itemIcon;

    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private TextMeshProUGUI promptText;       // Optional: To dynamically set text
    [SerializeField] private GameObject playerCrosshair;
    
    [Header("Values")]
    [SerializeField] private int totalQuota = 10;
    [SerializeField] private int currentItems = 0;
    [SerializeField] private float staminaPauseTime = 2f;
    [SerializeField] private float maxDetection = 1f;

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
            detectionUI.size = Mathf.Clamp01(detectionUI.size + detectionAmount);

            if (detectionUI.size >= maxDetection)
            {
                LogHandler.Log($"Detection number has reached {detectionUI.size}");
                GameManager.Instance.TriggerGameOver();
            }
        }
    }

    // essentially, when it's depleted, there's a two second pause before the float refills
    public void UpdateStamina(float staminaAmount)
    {
        staminaPauseTime = 0f;

        if (staminaUI != null)
        {
            staminaUI.size = Mathf.Clamp01(staminaUI.size + staminaAmount);
            
            // if the player runs out of stamina, start the pause timer.
            if (staminaUI.size <= 0f)
            {
                staminaPauseTime += Time.deltaTime;
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

    public void ChangeItemColourIcon()
    {
        itemIcon.color = new Color32(82, 248, 29, 255);
    }
}