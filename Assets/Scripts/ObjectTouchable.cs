 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectTouchable : MonoBehaviour
{
    public UnityEvent onVanish;

    [Header("Movement Settings")]
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float floatHeight = 0.25f;

    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        // Store the starting local or world position so it bobs around its initial spot
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Continuous rotation around the Y axis
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

        // Sinusoidal floating up and down
        float newY = startPosition.y + (Mathf.Sin(Time.time * floatSpeed) * floatHeight);
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    public void Vanish()
    {
        gameObject.SetActive(false);
        onVanish.Invoke();
    }
}
