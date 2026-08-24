using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

public class PlayerPickUpDrop : MonoBehaviour
{
    [SerializeField] public Transform playerCamTransform;
    [SerializeField] private LayerMask pickUpMask;

    public UnityEvent OnInteraction;
    public CinemachineVirtualCamera vcam;

    // Start is called before the first frame update
    void Start()
    {
        // Correct C# instantiation syntax for Vector3
        playerCamTransform.position = Vector3.zero;

        // Alternative shortcut for zeroing position
        // playerCamTransform.position = Vector3.zero;

        // Reset rotation as well to prevent the camera from pointing down
        playerCamTransform.rotation = Quaternion.identity;

        // Reset local transform of the virtual camera child
        vcam.transform.localPosition = Vector3.zero;
        vcam.transform.localRotation = Quaternion.identity;

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

