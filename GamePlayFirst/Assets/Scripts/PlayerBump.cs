using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBump : MonoBehaviour
{
    [Header("Bump Settings")]
    [SerializeField] private float bumpForce = 7.5f;
    private float raycastCheckDistance = 0.25f;
    private float raycastCheckRadius = 0.3f;

    private Rigidbody rb;
    private Movement movement;
    private bool isBumping;
    private bool zeroVel;

    // private float currentSpeed;
    // private const float MAX_SPEED = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();
        zeroVel = false;
    }

    void FixedUpdate()
    {
        if (isBumping && zeroVel)
        {
            movement.currentSpeed = 0;
            isBumping = false;
            zeroVel = false;
            movement.enabled = true;
            // if (currentSpeed < movement.currentSpeed)
            // {
            //     movement.currentSpeed = currentSpeed;
            // 
            //     //rb.velocity = newVelocity;
            // 
            // }
            // else
            // {
            //     currentSpeed = 0f;
            //     isBumping = false;
            //     zeroVel = false;
            //     movement.enabled = true;
            // }
        }
        else if (Physics.SphereCast(transform.position, raycastCheckRadius, transform.forward, out RaycastHit hit, raycastCheckDistance))
        {
            if (hit.collider.CompareTag("rail") || (hit.collider.CompareTag("wall") && !movement.isGrounded))
            {
                return;
            }

            movement.enabled = false;
            Vector3 bumpDir = -transform.forward;
            rb.velocity *= 0.2f;
            rb.velocity = bumpDir * bumpForce;

            isBumping = true;
        }

        if(rb.velocity.z == 0)
        {
            zeroVel = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Do something
    }
}
