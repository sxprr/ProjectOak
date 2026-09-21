using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    public float sensX = 100f;
    public float sensY = 100f;

    [Header("Transform References")]
    public Transform orientation;
    public Transform playerCapsule;
    public Transform cameraHolder;
    public Transform playerNose;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 2.5f;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 25f;
    public float staminaRegenRate = 15f;
    public float regenDelay = 2.0f;

    [Header("UI Reference")]
    public PlayerHUD playerHUD;

    private float xRotation;
    private float yRotation;

    private float currentStamina;
    private float regenTimer;
    private bool isExhausted;
    private bool isSprinting;

    private void Start()
    {
        currentStamina = maxStamina;
    }

    void Update()
    {
        HandleLook();
        ManageStamina();

        if (playerHUD != null)
        {
            playerHUD.UpdateStaminaDisplay(currentStamina, maxStamina);
        }
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

        if (cameraHolder != null)
        {
            cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        if (playerCapsule != null)
        {
            playerCapsule.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }

        if (orientation != null)
        {
            orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }

        if(playerNose != null)
        {
            playerNose.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }

    private void ManageStamina()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        bool isMoving = Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f;

        bool shiftHeld = Input.GetKey(KeyCode.LeftShift);

        // Player only sprints if holding shift, moving, and not exhausted
        isSprinting = shiftHeld && isMoving && !isExhausted && currentStamina > 0f;

        if (isSprinting)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            regenTimer = 0f;

            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                isExhausted = true; // Lock sprinting until recovered
            }
        }
        else
        {
            if (currentStamina < maxStamina)
            {
                regenTimer += Time.deltaTime;

                if (regenTimer >= regenDelay)
                {
                    currentStamina += staminaRegenRate * Time.deltaTime;

                    // Unlock exhaustion once stamina recovers partially or fully
                    if (isExhausted && currentStamina >= maxStamina * 0.25f)
                    {
                        isExhausted = false;
                    }
                }
            }

            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }
    }

    private void MovePlayer()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = (playerCapsule.forward * moveZ  +  playerCapsule.right * moveX).normalized;

        float currentSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;

        playerCapsule.position +=  moveDirection * currentSpeed * Time.fixedDeltaTime;

        Debug.Log($"Input - X: {moveX}, Z: {moveZ}");
    }
}

