using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    public float sensX = 100f;
    public float sensY = 100f;

    [Header("Transform References")]
    public Transform orientation;
    public Transform playerCapsule;
    public Transform cameraHolder; // Assign your CameraHolder / VCam parent here

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private float xRotation;
    private float yRotation;

    void Update()
    {
        HandleLook();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void HandleLook()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // 1. Tilt ONLY the camera vertically (Pitch)
        if (cameraHolder != null)
        {
            cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        // 2. Rotate the Player Capsule horizontally (Yaw)
        if (playerCapsule != null)
        {
            playerCapsule.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }

        // 3. Keep Orientation aligned with the body's yaw rotation
        if (orientation != null)
        {
            orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }

    private void MovePlayer()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        // Movement relative to capsule forward/right vectors
        Vector3 moveDirection = (playerCapsule.forward * moveZ + playerCapsule.right * moveX).normalized;

        float currentSpeed = isSprinting ? moveSpeed * 2.5f : moveSpeed;

        playerCapsule.position += moveDirection * currentSpeed * Time.fixedDeltaTime;
    }
}
