using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GlobalAndPersistent
{
    public class Movement : MonoBehaviour
    {
        [Header("Control Values")]
        [SerializeField] Transform playerCamera;
        [SerializeField] [Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
        [SerializeField] [Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
        [SerializeField] private float gravity = -30f;
        [SerializeField] public Transform groundCheck;
        [SerializeField] private LayerMask ground;
        [SerializeField] private bool cursorLock = true;
        [SerializeField] private bool Jumping;
        [SerializeField] private float mouseSensitivity = 3.3f;
        [SerializeField] private float speed = 6f;

        public float jumpHeight = 6f;
        public float sprintMultiplier;
        float velocityY;
        bool isGrounded;
        bool isSprinting;
        

        float cameraCap;
        Vector2 currentMouseDelta;
        Vector2 currentMouseDeltaVelocity;

        CharacterController controller;
        Vector2 currentDir;
        Vector2 currentDirVelocity;
        Vector3 velocity;

        [Header("UI Reference")]
        public PlayerHUD playerHUD;

        // Stamina values
        [Header("Stamina Values")]
        [SerializeField] private float maxStamina;
        [SerializeField] private float staminaRegenRate;
        [SerializeField] private float staminaDrainRate;
        [SerializeField] private float regenDelay;
        private bool isExhausted;
        private float currentStamina;
        private float regenTimer;

        [Header("Sense Ability Settings")]
        public UnityEvent onSense;
        private float currentCharge;
        [SerializeField] private float maxCharge = 100f;
        [SerializeField] private float senseRegenRate = 15f;   // Speed at which it recharges
        [SerializeField] private float senseDrainRate = 50f;   // Speed at which it drains when activated

        private bool isDischarging;

        // Start is called before the first frame update
        void Start()
        {
            controller = GetComponent<CharacterController>();

            if (cursorLock)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = true;
            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdateMouseLook();
            TriggerSenseAbility();

            if(playerHUD != null)
            {
                playerHUD.UpdateSenseDisplay(currentCharge, maxCharge);
            }
        }

        // sync with CineMachine camera update cycle
        private void FixedUpdate()
        {
            MovePlayer();
            ManageStamina();
        }


        // Grab mouse Input axis to move the camera
        void UpdateMouseLook()
        {
            Vector2 targetMouseDelta = new Vector2(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y")
        );

            currentMouseDelta = Vector2.SmoothDamp(
                currentMouseDelta,
                targetMouseDelta,
                ref currentMouseDeltaVelocity,
                mouseSmoothTime
            );

            // Vertical look
            cameraCap -= currentMouseDelta.y * mouseSensitivity;
            cameraCap = Mathf.Clamp(cameraCap, -90.0f, 90.0f);

            playerCamera.localEulerAngles = Vector3.right * cameraCap;

            // Horizontal look
            transform.Rotate(
                Vector3.up * currentMouseDelta.x * mouseSensitivity
            );
        }

        // player movement
        void MovePlayer()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, ground);

            Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            targetDir.Normalize();

            currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

            velocityY += gravity * 2f * Time.deltaTime;

            velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * speed + Vector3.up * velocityY;

            controller.Move(velocity * Time.deltaTime);

            Jump();

            if (isGrounded! && controller.velocity.y < -1f)
            {
                velocityY = -8f;
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

        void TriggerSenseAbility()
        {

            bool qPressed = Input.GetKeyDown(KeyCode.Q);

            if (qPressed) LogHandler.Log($"Q pressed. charge={currentCharge}, max={maxCharge}, discharging={isDischarging}");
       
            //Temporarliy removed conditions for testing, as well as onSense
            if (qPressed && currentCharge >= maxCharge && !isDischarging)
            {
                isDischarging = true;
                try { onSense.Invoke(); }
                catch (System.Exception e) { Debug.LogException(e); }
                LogHandler.Log("Sense ability activated! Discharging...");
            }

            // 2. State: Currently discharging smoothly down to zero
            if (isDischarging)
            {
                currentCharge -= senseDrainRate * Time.deltaTime;

                // Once fully drained, stop discharging so recharge can begin
                if (currentCharge <= 0f)
                {
                    currentCharge = 0f;
                    isDischarging = false;
                    Debug.Log("Fully discharged. Starting recharge.");
                }
            }
            // 3. State: Recharging smoothly back up to max Charge
            else if (currentCharge < maxCharge)
            {
                currentCharge += senseRegenRate * Time.deltaTime;
                currentCharge = Mathf.Min(currentCharge, maxCharge);
                
            }
        }

        // can now be toggled on and off in the inspector
        void Jump()
        {
            if(Jumping)
            {
                if (isGrounded && Input.GetButtonDown("Jump"))
                {
                    velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity);
                }

            }

        }

        //visualize ray
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, transform.forward * 5f);
        }
    }
}