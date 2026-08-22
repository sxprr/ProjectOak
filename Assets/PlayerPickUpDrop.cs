using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpDrop : MonoBehaviour
{
    [SerializeField] public Transform playerCamTransform;
    [SerializeField] private LayerMask pickUpMask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

    // We will interact with objects by shooting a ray from the player camera
    void Update()
    {
        Debug.DrawRay(playerCamTransform.position, playerCamTransform.forward * 4f, Color.green);
        PickItems();
    }

    public void PickItems()
    {
        float pickUpDistance = 4f;

        // Visual debug line: visible in Scene view (and Game view if 'Gizmos' is enabled)
        // Color turns Green on hit, Red on miss. Lasts for 2 seconds.
        bool hasHit = Physics.Raycast(playerCamTransform.position, playerCamTransform.forward, out RaycastHit raycastHit, pickUpDistance, pickUpMask);

        Color rayColor = hasHit ? Color.green : Color.red;
        Debug.DrawRay(playerCamTransform.position, playerCamTransform.forward * pickUpDistance, rayColor, 2.0f);

        if (hasHit)
        {
            LogHandler.Log($"{raycastHit.transform.name} raycast hit.");

            // Check if the object has the target component
            if (raycastHit.transform.TryGetComponent(out ObjectTouchable objectTouchable))
            {
                LogHandler.Log($"{objectTouchable.name}has been collected.");
                objectTouchable.gameObject.SetActive(false);
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

