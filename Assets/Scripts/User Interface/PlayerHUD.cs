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
    [SerializeField] private Scrollbar senseUI;
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
    [SerializeField] private float circleScaleRate = 0.0140f;


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

    // Listening to "OnPlayerSight" event from EnemyBehaviour.cs
    public void UpdateDetection(float detectionAmount)
    {
        // Don't update detection UI if the game is paused
        if (GameManager.Instance.IsPaused)
            return;

        // TODO: More logic is being handled than necessary. Migrate logic back to EnemyBehaviour.cs
        if (detectionUI != null)
        {
            // detection UI is the scrollbar. detectionCircle is the red circle that gets more prominent over time
            detectionUI.size = Mathf.Clamp01(detectionUI.size + detectionAmount);
            detectionCircle.rectTransform.localScale -= new Vector3(circleScaleRate, circleScaleRate, 0f);


            // when the bar fills up:
            if (detectionUI.size >= maxDetection)
            {
                // Similar to many fps games where you take damage and you see a red outline
                detectionCircle.rectTransform.localScale = minCircleScale;
            }
        }
    }
    // This point of this is to "discharge" the detection amount over time.
    // Listening to onPlayerLoss event from EnemyBehaviour.cs
    public void SubtractDetection(float detectionAmount)
    {

        if (detectionUI != null)
        {
            detectionUI.size -= Mathf.Clamp01(detectionUI.size - detectionAmount * Time.deltaTime);
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
        if(senseUI != null)
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