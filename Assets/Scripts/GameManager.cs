using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public UnityEvent OnInteraction;
    public UnityEvent OnEnemySight;
    public UnityEvent OnVictoryAchieved;

    private void Awake()
    {
        LogHandler.Log($"<color=green>SUCCESS:</color> {gameObject.name} has entered the chat.");
    }

    public static void TriggerMoonResist()
    {
        
    }

    // The event bus belongs here.
}
