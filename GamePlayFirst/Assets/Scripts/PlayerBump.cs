using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBump : MonoBehaviour
{
    [Header("Bump Settings")]
    [SerializeField] private float bumpForce = 5f;
    [SerializeField] private float raycastCheckDistance = 0.5f;
    [SerializeField] private float raycastCheckRadius = 0.3f;

    private Rigidbody rb;
    private Movement movement;
    private bool isBumping;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();
    }

    void FixedUpdate()
    {
        if (isBumping) return;

        if (Physics.SphereCast(transform.position, raycastCheckRadius, transform.forward, out RaycastHit hit, raycastCheckDistance))
        {
            if (hit.collider.CompareTag("rail") || (hit.collider.CompareTag("wall") && !movement.isGrounded))
            {
                return;
            }

            StartCoroutine(BumpBack());
        }
    }

    IEnumerator BumpBack()
    {
        isBumping = true;
        movement.enabled = false;

        Vector3 bumpDir = -transform.forward;
        rb.velocity *= 0.2f;
        rb.velocity = bumpDir * bumpForce;

        yield return new WaitForSeconds(0.1f);

        movement.enabled = true;
        isBumping = false;
    }
}
