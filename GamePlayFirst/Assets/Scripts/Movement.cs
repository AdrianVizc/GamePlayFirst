using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //Keyboard Controls only for now

    [SerializeField] private float maxSpeed;
    [SerializeField] private float minSpeed;
    [SerializeField] private float startingSpeed;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float lateralSpeed;
    [SerializeField] private float accelSpeed;
    [SerializeField] private float decelSpeed;
    [SerializeField] private float brakeForce;
    private Rigidbody rb;
    private Camera mainCamera;
    [SerializeField] private bool isAccelerating;
    [SerializeField] private bool isBraking;
    [SerializeField] private bool isTurning;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        currentSpeed = startingSpeed;
        rb.velocity = mainCamera.transform.forward * currentSpeed;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        GetInput();
        Move();
    }

    private void GetInput()
    {
        //horizontalInput = Input.GetAxis("Horizontal");
        //verticalInput = Input.GetAxis("Vertical");
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

        rb.velocity = mainCamera.transform.forward * currentSpeed;

        //Turning Logic
        if (isTurning)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            if (Mathf.Abs(horizontalInput) > 0.1f)
            {
                float turnAmount = horizontalInput * lateralSpeed * Time.fixedDeltaTime;
                transform.Rotate(0f, turnAmount, 0f);
                Debug.DrawRay(transform.position, transform.forward * 5f, Color.red);
            }
            Vector3 turnForce = mainCamera.transform.right * horizontalInput * lateralSpeed;
            rb.AddForce(turnForce, ForceMode.Acceleration);
        }
    }
}
