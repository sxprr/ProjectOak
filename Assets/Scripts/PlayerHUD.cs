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
    [SerializeField] private int totalQuota = 10;
    [SerializeField] private int currentItems = 0;

    [Header("Events")]
    public UnityEvent onCaught;

    private void Start()
    {

        currentItems = 0;

        // Fallback checks if references aren't assigned via Inspector
        if (detectionUI == null)
        {
            detectionUI = GetComponentInChildren<Scrollbar>();
        }

        if (detectionUI != null)
        {
            detectionUI.size = 0f;
        }
    }

    /// <summary>
    /// Updates the enemy detection UI scrollbar fill (value between 0.0 and 1.0).
    /// </summary>
    /// <param name="detectionAmount">Normalized detection amount.</param>
    public void UpdateDetection(float detectionAmount)
    {
        detectionAmount += detectionUI.size;

        if (detectionUI != null)
        {
            detectionUI.size = Mathf.Clamp01(detectionAmount);
        }
    }

    public void SubtractDetection(float detectionAmount)
    {
        detectionAmount -= detectionUI.size;

        if (detectionUI != null)
        {
            detectionUI.size = Mathf.Clamp01(detectionAmount);
        }
    }


    /// <summary>
    /// Updates the item quota UI text display.
    /// </summary>
    /// <param name="currentItems">Number of items currently collected.</param>
    /// <param name="totalQuota">Total items required.</param>
    public void UpdateItemQuota(int Items)
    {
        Items += currentItems;

        if (itemText != null)
        {
            itemText.text = $"{++currentItems}/{totalQuota}";
        }
    }
}