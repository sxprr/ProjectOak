using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public UnityEvent OnEnemySight;

    private void Awake()
    {
        LogHandler.Log($"<color=green>SUCCESS:</color> {gameObject.name} has entered the chat.");
    }

    private void Update()
    {
   
    }
}
