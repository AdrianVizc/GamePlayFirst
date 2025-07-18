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
    [SerializeField] private float currentSpeed;
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
    //private Vector3 initialForward;
    private Vector3 adaptiveForward;
    [SerializeField] private float forwardAdjustSpeed = 2f;
    [SerializeField] private float turnAssistStrength = 1.5f;

    [Header("Jumping")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] public bool isGrounded;

    [Header("Other Components")]
    private Rigidbody rb;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
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

    }

    private void FixedUpdate()
    {
        GroundCheck();
        GetInput();
        //UpdateAdaptiveForward();
        Move();
    }

    private void GetInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            isAccelerating = true;
        }
        else
        {
            isAccelerating = false;
        }

        if (Input.GetKey(KeyCode.S))
        {
            isBraking = true;
        }
        else
        {
            isBraking = false;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            isTurning = true;
        }
        else
        {
            isTurning = false;
        }

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }
    private void Move()
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

        //rb.velocity = mainCamera.transform.forward * currentSpeed;

        //Determining the forward direction & calculating velocity
        Vector3 forward = mainCamera.transform.forward;
        forward.y = 0;
        forward.Normalize();

        //This is so we preserve the vertical velocity component instead of overriding it each frame.
        //Doing rb.velocity = mainCamera.transform.forward * currentSpeed technically will override & reset the y velocity to 0 every frame.
        //Which will mess with the gravity when jumping (making it feel very floaty)
        Vector3 newVelocity = transform.forward * currentSpeed;
        newVelocity.y = rb.velocity.y; 
        rb.velocity = newVelocity;

        //Turning Logic
        if (isTurning)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            if (Mathf.Abs(horizontalInput) > 0.1f)
            {
                float turnAmount = horizontalInput * lateralSpeed * Time.fixedDeltaTime;

                //Get the vector for where the player is trying to turn (with respect to the current orientation)
                Quaternion nextRotation = Quaternion.Euler(0f, turnAmount, 0f) * transform.rotation;
                Vector3 nextForward = nextRotation * Vector3.forward; //Get the forward vector for that direction
                nextForward.y = 0; //We don't care about any vertical components
                nextForward.Normalize(); //Normalize prior to dot product operations

                float dotProduct = Vector3.Dot(adaptiveForward, nextForward);
                float driftAmount = Mathf.Clamp01(1f - dotProduct);

                //If within our specified range, allow the turn (rotation) to happen
                if (dotProduct >= maxDot)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, nextRotation, lateralSpeed * Time.fixedDeltaTime);
                }
                else
                {
                    Quaternion targetRotation = Quaternion.LookRotation(adaptiveForward);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, driftAmount * forwardAdjustSpeed * Time.fixedDeltaTime);
                }
            }
            Vector3 turnForce = mainCamera.transform.right * horizontalInput * lateralSpeed;
            rb.AddForce(turnForce, ForceMode.Acceleration);

            Debug.DrawRay(transform.position, adaptiveForward * 20f, Color.red);
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void GroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);
    }

    public void UpdateForwardDirection(Vector3 newForward)
    {
        adaptiveForward = newForward;
    }
}
