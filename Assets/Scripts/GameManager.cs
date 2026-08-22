using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public UnityEvent OnInteraction;
    public UnityEvent OnEnemySight;
    public UnityEvent OnVictoryAchieved;
    public UnityEvent OnPlayerDetection;

    private void Awake()
    {
        LogHandler.Log($"<color=green>SUCCESS:</color> {gameObject.name} has entered the chat.");
    }

    private void Update()
    {
       
    }

    // The event bus belongs here.
}
