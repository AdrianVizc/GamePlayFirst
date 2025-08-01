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
    private Animator animator;
    private bool isBumping;
    private bool zeroVel;
    private bool isCollidingIgnored;

    // private float currentSpeed;
    // private const float MAX_SPEED = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();
        animator = GetComponentInChildren<Animator>();
        zeroVel = false;
        isCollidingIgnored = false;
    }

    void FixedUpdate()
    {
        Vector3 bottom = transform.position + Vector3.down * 0.75f;
        Vector3 top = transform.position + Vector3.up * 1.2f;

        if (isBumping && zeroVel)
        {
            movement.currentSpeed = 0;
            isBumping = false;
            zeroVel = false;
            movement.enabled = true;
            AudioManager.instance.UnPauseSkateSound();
        }
        else if (!isCollidingIgnored && Physics.CapsuleCast(bottom, top, raycastCheckRadius, transform.forward, out RaycastHit hit, raycastCheckDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.CompareTag("rail") || 
                (hit.collider.CompareTag("wall") && !movement.isGrounded) &&
                !hit.collider.CompareTag("enemy"))
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
            //animator.SetTrigger("getHurt");

            StartCoroutine(IgnoreCollideTemporarily(0.1f));
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

    private IEnumerator IgnoreCollideTemporarily(float duration)
    {
        isCollidingIgnored = true;
        yield return new WaitForSeconds(duration); // ignore rail tag for x sec
        isCollidingIgnored = false;
    }
}
