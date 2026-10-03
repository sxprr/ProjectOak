using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SorrowlessUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text[] letterTexts;

    [Header("Color Settings")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color completedColor = Color.green;

    private List<TMP_Text> inactiveLetters = new List<TMP_Text>();

    private void Awake()
    {
        // Initialize the tracking list and ensure all letters start hidden & standard color
        foreach (var letter in letterTexts)
        {
            if (letter != null)
            {
                letter.color = defaultColor;
                letter.gameObject.SetActive(false);
                inactiveLetters.Add(letter);
            }
        }
    }

    // Call this method via itemCollect event
    public void ShowWord()
    {
        // If all letters are already revealed, nothing left to do
        if (inactiveLetters.Count == 0) return;

        // Pick a random remaining inactive letter
        int randomIndex = Random.Range(0, inactiveLetters.Count);
        TMP_Text selectedLetter = inactiveLetters[randomIndex];

        // Reveal it and remove it from the available pool
        selectedLetter.gameObject.SetActive(true);
        inactiveLetters.RemoveAt(randomIndex);

        // Check if all 10 have now been collected
        if (inactiveLetters.Count == 0)
        {
            SetAllLettersColor(completedColor);
        }
    }

    private void SetAllLettersColor(Color color)
    {
        foreach (var letter in letterTexts)
        {
            if (letter != null)
            {
                letter.color = color;
            }
        }
    }
}