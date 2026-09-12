using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    
    public static CameraShakeManager instance;

    [SerializeField] private float globalShakerForce = 1f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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

    public void StartContinuousShake(CinemachineImpulseSource impulseSource)
    {
        // Instead of a for loop, we start a Coroutine
        StartCoroutine(ShakeOverTime(impulseSource, 5.0f)); // Shake for 5 seconds
    }

    IEnumerator ShakeOverTime(CinemachineImpulseSource impulseSource, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Generate a small impulse every frame
            impulseSource.GenerateImpulseWithForce(globalShakerForce);

            elapsed += Time.deltaTime; // Track how much real time has passed
            yield return null; // "Wait" until the next frame before looping again
        }
    }
}

