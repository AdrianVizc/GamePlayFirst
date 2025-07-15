using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //Keyboard Controls only for now

    [SerializeField] private float maxSpeed;
    [SerializeField] private float startingSpeed;
    private float currentSpeed;
    private float lateralSpeed;
    private float brakeForce;
    private Rigidbody rb;
    private Camera mainCamera;
    private Vector3 moveDirection;
    private float horizontalInput;
    private float verticalInput;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        currentSpeed = startingSpeed;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        Move();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

    }
    private void Move()
    {
        //moveDirection = mainCamera.transform.forward * verticalInput + mainCamera.transform.right * horizontalInput;
        //moveDirection = new Vector3(moveDirection.x, 0, moveDirection.z);

        //rb.velocity
        Vector3 forwardForce = mainCamera.transform.forward * currentSpeed;

        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(forwardForce, ForceMode.Acceleration);
        }
    }
}
