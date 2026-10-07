using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GlobalAndPersistent
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] Transform playerCamera;
        [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
        [SerializeField][Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
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
        private bool isExhausted;
        private float currentStamina;
        private float staminaDrainRate;
        private float regenTimer;
        private float maxStamina;
        private float regenDelay;
        private float staminaRegenRate;

        // sense values

        public UnityEvent onSense;
        private float currentCharge;
        private float senseRegenTimer;
        private float senseRegenRate;
        private bool senseFull;
        private float maxCharge;


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

        private void FixedUpdate()
        {
            MovePlayer();
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

        
        
        void TriggerSenseAbility()
        {
            // ult button, assigned to a boolean.
            //bool ultPressed = Input.GetKey(KeyCode.Q);

            if(Input.GetKey(KeyCode.Q))
            {
                onSense.Invoke();
            }

            /*
            //Player can only press ult when the meter is full.
            //when we press the button, it should go straight to zero.
            if(ultPressed && senseFull && currentCharge >= maxCharge)
            {
                
                senseRegenTimer = 0f;
                currentCharge = 0f;

                if(currentCharge <= 0f)
                {
                    //start charging up as normal
                    currentCharge += senseRegenRate * Time.deltaTime;

                }
            }
            else
            {
                //if we aren't full,
                if(currentCharge < maxCharge)
                {
                    
                }
            }
            */
            

            
        }

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