using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

public class PlayerPickUpDrop : MonoBehaviour
{
    [SerializeField] private Camera mainCamera; // Assign Main Camera in Inspector

    [SerializeField] public Transform playerCamTransform;
    [SerializeField] private LayerMask pickUpMask;

    public UnityEvent OnInteraction;
    

    // Start is called before the first frame update
    void Start()
    {
        // Fall back to Camera.main if unassigned in the Inspector
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        LogHandler.Log($"Player Camera's rotation is currently {mainCamera.transform.rotation}");

    }

    // Update is called once per frame

    // We will interact with objects by shooting a ray from the player camera
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnInteraction.Invoke();
        }
    }

    public void PickItems()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera reference is missing on PlayerPickUpDrop!");
            return;
        }

        float pickUpDistance = 4f;

        // Shoot from camera center (0.5, 0.5 is screen center, not 0, 0)
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // define it's direction vector (i think this is where the problem is)
        Vector3 rayDirection = new Vector3(0f,0f,0f);

        // Show cordinates in the console.
        LogHandler.Log($"Pickup Raycast origin point: {ray}");

        // I think there is an issue with the direction(Vector3)
        bool hasHit = Physics.Raycast(ray,out RaycastHit raycastHit, pickUpDistance, pickUpMask);

        // Debug ray now matches the actual raycast
        Color rayColor = hasHit ? Color.green : Color.red;
        Debug.DrawRay(ray.origin, transform.forward * pickUpDistance, rayColor, 2.0f);

        // Show co ordinates in console.
        LogHandler.Log($"GREEN ray origin is: {ray.origin}");

        if (hasHit)
        {
            LogHandler.Log($"{raycastHit.transform.name} raycast hit.");

            // Check if the object has the target component
            if (raycastHit.transform.TryGetComponent(out ObjectTouchable objectTouchable))
            {
                LogHandler.Log($"{objectTouchable.name} has been collected.");
                objectTouchable.Vanish();
            }
            else
            {
                // Fixed: Access the hit transform directly rather than the null component variable
                LogHandler.Log($"{raycastHit.transform.name} was hit, but has no ObjectTouchable script.");
            }
        }
        else
        {
            LogHandler.Log("Raycast fired but missed everything within range.");
        }
    }
}

