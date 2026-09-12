using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FinishLine : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onGameFinish;

    private void OnTriggerEnter(Collider other)
    {
        // Ensure the colliding object is the player
        if (other.CompareTag("Player"))
        {
            // Optional: Prevent double-triggering if the player collides twice in one frame
            gameObject.SetActive(false);

            LogHandler.Log($"Player stepped into the victory portal. Triggering finish sequence.");
            onGameFinish?.Invoke();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
