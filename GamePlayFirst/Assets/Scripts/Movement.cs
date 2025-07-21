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
    private bool isCorrecting = false;
    private float steerBackStrength = 0f;
    [SerializeField] private float steerBackMaxStrength = 1f;
    [SerializeField] private float steerBackRampUpSpeed = 1f;
    [SerializeField] private float steerBackDecaySpeed = 0.5f;

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
        rb.velocity = newVelocity;

        //Apply Lateral Force
        Vector3 turnForce = mainCamera.transform.right * horizontalInput * lateralSpeed;
        rb.AddForce(turnForce, ForceMode.Acceleration);
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
