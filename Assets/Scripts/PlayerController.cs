using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // handling player movement
    public float sensX, sensY;
    float xRotation, yRotation;
    public Transform orientation;
    public Transform playerCapsule;
    public float moveSpeed = 5f;


    void Update()
    {
        // 1. Handle Camera Look
        HandleLook();
        
    }

    private void FixedUpdate()
    {
        //2. Handle the movement of the player capsule on fixed update
        MovePlayer();
    }

    private void HandleLook()
    {

        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;

        // CHANGE: Subtract mouseY instead of adding it
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply rotations
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);


    }
    
    // better method
    private void MovePlayer()
    {
        // 1. Read continuous input values (-1, 0, or 1)
        float moveX = Input.GetAxisRaw("Horizontal"); // A (-1) / D (+1)
        float moveZ = Input.GetAxisRaw("Vertical");   // S (-1) / W (+1)

        // 2. Check continuous key hold for sprinting
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        // 3. Calculate direction relative to capsule orientation and normalize diagonal speed
        Vector3 moveDirection = (playerCapsule.forward * moveZ + playerCapsule.right * moveX).normalized;

        // 4. Determine final speed modifier
        float currentSpeed = isSprinting ? moveSpeed * 2.5f : moveSpeed;

        // 5. Apply smooth movement per frame
        playerCapsule.position += moveDirection * currentSpeed * Time.deltaTime;

       
    }
}

