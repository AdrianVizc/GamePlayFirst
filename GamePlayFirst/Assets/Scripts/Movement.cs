using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //NOTE: Keyboard Controls only for now
    [Header("Forward Speed")]
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float startingSpeed;
    [SerializeField] public float currentSpeed;
    [SerializeField] private float accelSpeed;
    [SerializeField] private float decelSpeed;
    [SerializeField] private float brakeForce;
    [SerializeField] private float jumpForce;
    [SerializeField] private bool isAccelerating;
    [SerializeField] private bool isBraking;

    [Header("Turn Speed")]
    [SerializeField] private float lateralSpeed;
    [SerializeField] private bool isTurning;
    [SerializeField] private float maxTurnAngle;
    [SerializeField] private float maxDot;
    private Vector3 adaptiveForward;
    private bool isCorrecting = false;
    private float steerBackStrength = 0f;
    [SerializeField] private float steerBackMaxStrength = 1f;
    [SerializeField] private float steerBackRampUpSpeed = 1f;
    [SerializeField] private float steerBackDecaySpeed = 0.5f;

    [Header("Jumping")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] public bool isGrounded;
    [SerializeField] public bool canDoubleJump;

    [Header("Sideways Dash")]
    [SerializeField] public bool canDash;
    [SerializeField] private float dashForce = 5f;
    [SerializeField] private float upForce = 2f;
    [SerializeField] private bool isDashing;
    private float dashTimer = 0f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private bool isRecovering;
    private float recoveryTimer = 0f;
    [SerializeField] private float recoveryDuration = 0.5f;
    [SerializeField] private float recoverySmoothingFactor = 2f;

    [Header("Other Components")]
    private Rigidbody rb;
    private Camera mainCamera;
    private PlayerGrind rail;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        rail = GetComponent<PlayerGrind>();
        rb.freezeRotation = true;
        mainCamera = Camera.main;
        currentSpeed = startingSpeed;
        adaptiveForward = transform.forward;
        rb.velocity = mainCamera.transform.forward * currentSpeed;
        maxDot = Mathf.Cos(maxTurnAngle * Mathf.Deg2Rad);
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        GroundCheck();

        if (isDashing)
        {
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                isRecovering = true;
                recoveryTimer = recoveryDuration;
            }
        }

        if (isRecovering)
        {
            recoveryTimer -= Time.fixedDeltaTime;
            if (recoveryTimer <= 0f)
            {
                isRecovering = false;
            }
        }
        
        Move();
    }

    private void GetInput()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        isAccelerating = vertical > 0.1f;
        isBraking = vertical < -0.1f;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            if (isGrounded)
            {
                Jump();
                canDoubleJump = true;
                canDash = true;
            }
            else if (canDoubleJump)
            {
                DoubleJump();
            }
        }

        if (canDash && !isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Q) || (Input.GetKeyDown(KeyCode.Joystick1Button1) && horizontal < -0.1f))
            {
                ActivateDash(-1);
            }
            else if (Input.GetKeyDown(KeyCode.E) || (Input.GetKeyDown(KeyCode.Joystick1Button1) && horizontal > 0.1f))
            {
                ActivateDash(1);
            }
        }
    }
    private void Move()
    {
        if (!isDashing)
        {
            //Forward Movement Logic
            if (isAccelerating)
            {
                if (currentSpeed < maxSpeed)
                {
                    currentSpeed += accelSpeed * Time.fixedDeltaTime;
                }
            }
            else if (isBraking)
            {
                if (currentSpeed > minSpeed)
                {
                    currentSpeed -= brakeForce * Time.fixedDeltaTime;
                }
            }
            else
            {
                if (currentSpeed > minSpeed)
                {
                    currentSpeed -= decelSpeed * Time.fixedDeltaTime;
                }
            }

            currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);

            //Turn Logic
            float horizontalInput = Input.GetAxis("Horizontal");
            float turnAmount = horizontalInput * lateralSpeed * Time.fixedDeltaTime;

            //Predict where the player wants to turn based on their input
            Quaternion intendedRotation = Quaternion.Euler(0f, turnAmount, 0f) * transform.rotation;
            Vector3 intendedForward = intendedRotation * Vector3.forward;
            intendedForward.y = 0;
            intendedForward.Normalize();

            //Check if the turn is "valid" or allowed
            float dotProduct = Vector3.Dot(adaptiveForward, intendedForward);
            bool isOutOfBounds = dotProduct < maxDot;

            //Logic for steering back on course if trying to turn out of bounds
            if (isOutOfBounds)
            {
                isCorrecting = true;
                steerBackStrength += steerBackRampUpSpeed * Time.fixedDeltaTime;
                steerBackStrength = Mathf.Clamp(steerBackStrength, 0f, steerBackMaxStrength);
            }
            else
            {
                steerBackStrength -= steerBackDecaySpeed * Time.fixedDeltaTime;
                if (steerBackStrength <= 0.01f)
                {
                    steerBackStrength = 0f;
                    isCorrecting = false;
                }
            }

            //Applying Rotations
            Quaternion finalRotation;

            if (isCorrecting && steerBackStrength > 0f)
            {
                Quaternion correctionRotation = Quaternion.LookRotation(adaptiveForward);
                finalRotation = Quaternion.Slerp(intendedRotation, correctionRotation, steerBackStrength);
            }
            else
            {
                finalRotation = intendedRotation;
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, lateralSpeed * Time.fixedDeltaTime);

            //Applying Forward Movement
            Vector3 forward = transform.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 newVelocity = forward * currentSpeed;
            newVelocity.y = rb.velocity.y; //preserve important y components, namely gravity

            if (isRecovering)
            {
                rb.velocity = Vector3.Lerp(rb.velocity, newVelocity, Time.fixedDeltaTime * recoverySmoothingFactor);
            }
            else
            {
                rb.velocity = newVelocity;
            }

            //Apply Lateral Force
            Vector3 turnForce = mainCamera.transform.right * horizontalInput * lateralSpeed;
            rb.AddForce(turnForce, ForceMode.Acceleration);
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

            canDoubleJump = true;
        }
    }

    private void DoubleJump()
    {
        if (!isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

            canDoubleJump = false;
        }
    }

    private void GroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.1f, groundLayer);
    }

    public void UpdateForwardDirection(Vector3 newForward)
    {
        adaptiveForward = newForward;
    }

    private void ActivateDash(int direction)
    {
        Debug.Log("Dashing");
        Vector3 dashDirection = mainCamera.transform.right * direction; //-1 will make this LEFT, 1 will keep this RIGHT
        dashDirection.y = 0;
        dashDirection.Normalize();

        //rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        //rb.AddForce((dashDirection * dashForce) + (Vector3.up * upForce), ForceMode.Impulse);
        Vector3 dashVelocity = dashDirection * dashForce + Vector3.up * upForce;
        rb.velocity = rb.velocity * 0.5f + dashVelocity * 0.5f;

        isDashing = true;
        dashTimer = dashDuration;
        canDash = false;
        
        //StartCoroutine(SlowMoDash());
    }

    private IEnumerator SlowMoDash()
    {
        Time.timeScale = 0.5f;
        yield return new WaitForSecondsRealtime(0.2f);
        Time.timeScale = 1f;
    }
}
