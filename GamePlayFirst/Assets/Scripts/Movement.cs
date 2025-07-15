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
    private Vector3 moveDirection;
    private float horizontalInput;
    private float verticalInput;
    private bool isAccelerating;
    private bool isBraking;
    private bool isTurning;

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

        //if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        //{
        //    isTurning = true;
        //}
    }
    private void Move()
    {
        //moveDirection = mainCamera.transform.forward * verticalInput + mainCamera.transform.right * horizontalInput;
        //moveDirection = new Vector3(moveDirection.x, 0, moveDirection.z);

        //rb.velocity

        //Vector3 forwardForce = mainCamera.transform.forward * currentSpeed;

        //if (rb.velocity.magnitude < maxSpeed)
        //{
        //    rb.AddForce(forwardForce, ForceMode.Acceleration);
        //}
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
    }
}
