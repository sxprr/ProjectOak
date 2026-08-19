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

    /*There's probably a neater way to implement WASD movement.
      But this will do for the prototype phase.
    */
    private void HandleMovement()
    {
        //shift key that allows the player to increase thier speed for a few seconds

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            playerCapsule.position += playerCapsule.TransformDirection(Vector3.forward) * Time.deltaTime * moveSpeed * 2.5f;
        }
        else if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.LeftShift))
        {
            playerCapsule.position += playerCapsule.TransformDirection(Vector3.forward) * Time.deltaTime * moveSpeed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position -= playerCapsule.TransformDirection(Vector3.forward) * Time.deltaTime * moveSpeed;
        }

        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            playerCapsule.position += playerCapsule.TransformDirection(Vector3.left) * Time.deltaTime * moveSpeed;
        }
        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
           playerCapsule.position -= playerCapsule.TransformDirection(Vector3.left) * Time.deltaTime * moveSpeed;

        }

        // The following code will be used for debugging
        if (Input.anyKeyDown)
        {
            // 1. Log what was actually pressed 
            string pressedKey = "Unknown/Mouse";
            foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(k))
                {
                    pressedKey = k.ToString();
                    break;
                }
            }

            //KeyCode expectedKey = 
            LogHandler.Log($"<color=cyan>[INPUT]</color> Pressed: <b>{pressedKey}</b> ");

            LogHandler.Log("The player capsule is located at: " + playerCapsule.position);


        }
    }
}

