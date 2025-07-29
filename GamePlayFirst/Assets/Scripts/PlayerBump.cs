using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBump : MonoBehaviour
{
    [Header("Bump Settings")]
    [SerializeField] private float bumpForce = 7.5f;
    private float raycastCheckDistance = 0.25f;
    [SerializeField] private float raycastCheckRadius = 0.4f;

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
            AudioManager.instance.UnPauseSkateSound();
        }
        else if (Physics.SphereCast(transform.position, raycastCheckRadius, transform.forward, out RaycastHit hit, raycastCheckDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.CompareTag("rail") || (hit.collider.CompareTag("wall") && !movement.isGrounded))
            {
                return;
            }

            AudioManager.instance.PlayEnvironmentSound("Bonk");
            AudioManager.instance.PauseSkateSound();

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

        ApplyExtraFallGravity();
    }

    private void ApplyExtraFallGravity()
    {
        if (!movement.isGrounded && rb.velocity.y < -0.1)
        {
            Vector3 addedGravity = Physics.gravity * (movement.gravityMultiplier - 1f);
            rb.AddForce(addedGravity, ForceMode.Acceleration);
        }
    }
}
