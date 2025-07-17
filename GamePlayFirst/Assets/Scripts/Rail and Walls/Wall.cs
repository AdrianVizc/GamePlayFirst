using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWall : MonoBehaviour
{
    [Header("Wall Detection")]
    [SerializeField] private float wallCheckDistance = 0.7f;

    [Header("Wall Running")]
    [SerializeField] private float wallRunSpeed = 10f;

    [Header("Wall Jumping")]
    [SerializeField] private float wallJumpAwayForce = 6f;
    [SerializeField] private float wallJumpUpForce = 8f;
    [SerializeField] private float jumpTimer = 0.75f;
    private float wallJumpCooldown;
    private float wallJumpTimer;
    private Vector3 storedWallNormal;

    [Header("Camera Settings")]
    [SerializeField] private float FOV = 70f;
    [SerializeField] private float tilt = 5f;
    private float defaultFOV;
    private float defaultTilt;
    private CinemachineVirtualCamera virtualCamera;

    private bool wallLeft;
    private bool wallRight;
    private bool wallHit;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;

    private bool isGrounded;
    private Transform orientation;
    private Rigidbody rb;
    [HideInInspector] public bool isWallRunning;
    private bool isWallJumping;

    private void Start()
    {
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        defaultFOV = virtualCamera.m_Lens.FieldOfView;
        defaultTilt = virtualCamera.GetComponent<CinemachineRecomposer>().m_Dutch;

        wallJumpCooldown = 0.2f;
        wallJumpTimer = 0f;

        orientation = transform;
        isWallJumping = false;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
            return; // Skip wall-run logic briefly after jumping
        }

        if (!isGrounded)
        {
            CheckForWall();
        }

        if (wallHit && !isGrounded)
        {
            if (!isWallRunning)
            {
                StartWallRun();
            }

            DoWallRun();

            // Jump off wall if player presses jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isWallJumping = true;
                rb.useGravity = true;

                Vector3 currentVelocity = rb.velocity;

                // Extract forward velocity relative to player orientation
                float forwardSpeed = Vector3.Dot(currentVelocity, transform.forward);

                // Calculate jump velocity
                Vector3 jumpVelocity = storedWallNormal * wallJumpAwayForce + Vector3.up * wallJumpUpForce;

                // Add forward momentum
                jumpVelocity += transform.forward * forwardSpeed;

                // Set new velocity
                rb.velocity = jumpVelocity;

                StartCoroutine(ChangeFOV(virtualCamera, defaultFOV, 0.3f));
                StartCoroutine(ChangeTilt(virtualCamera, defaultTilt, 0.3f));

                wallJumpTimer = wallJumpCooldown;
                Invoke(nameof(EndWallRun), jumpTimer);
            }
        }
        else if (isWallRunning && !isWallJumping && (isGrounded || !wallHit))
        {
            EndWallRun();
        }
    }

    private void FixedUpdate()
    {
        isGrounded = GetComponent<Movement>().isGrounded;
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, wallCheckDistance);

        wallHit = (wallRight && rightWallhit.collider.CompareTag("wall")) ||
                  (wallLeft && leftWallhit.collider.CompareTag("wall"));
    }

    private void StartWallRun()
    {
        isWallRunning = true;
        GetComponent<Movement>().enabled = false;
        rb.useGravity = false;

        Vector3 rawNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;
        storedWallNormal = new Vector3(rawNormal.x, 0f, rawNormal.z).normalized;

        StartCoroutine(ChangeFOV(virtualCamera, FOV, 0.3f));

        if (wallRight)
        {
            StartCoroutine(ChangeTilt(virtualCamera, tilt, 0.3f));
        }
        else if (wallLeft)
        {
            StartCoroutine(ChangeTilt(virtualCamera, -tilt, 0.3f));
        }
    }

    private void DoWallRun()
    {
        Vector3 wallDir = Vector3.Cross(Vector3.up, storedWallNormal);

        // Ensure wallDir points roughly forward relative to player's current forward
        if (Vector3.Dot(wallDir, transform.forward) < 0)
        {
            wallDir = -wallDir;
        }

        // Rotate player to face along the wall direction
        Quaternion targetRotation = Quaternion.LookRotation(wallDir.normalized, Vector3.up);
        transform.rotation = targetRotation;

        // Move player along the wall, no vertical slide
        rb.velocity = wallDir.normalized * wallRunSpeed;
    }

    private void EndWallRun()
    {
        isWallJumping = false;
        isWallRunning = false;
        GetComponent<Movement>().enabled = true;
        rb.useGravity = true;

        StartCoroutine(ChangeFOV(virtualCamera, defaultFOV, 0.3f));
        StartCoroutine(ChangeTilt(virtualCamera, defaultTilt, 0.3f));

    }

    IEnumerator ChangeFOV(CinemachineVirtualCamera cam, float endFOV, float duration)
    {
        float startFOV = cam.m_Lens.FieldOfView;
        float time = 0;
        while (time < duration)
        {
            cam.m_Lens.FieldOfView = Mathf.Lerp(startFOV, endFOV, time / duration);
            yield return null;
            time += Time.deltaTime;
        }
    }

    IEnumerator ChangeTilt(CinemachineVirtualCamera cam, float endTilt, float duration)
    {
        CinemachineRecomposer recomposer = cam.GetComponent<CinemachineRecomposer>();
        if (recomposer == null) yield break;

        float startDutch = recomposer.m_Dutch;
        float time = 0f;

        while (time < duration)
        {
            recomposer.m_Dutch = Mathf.Lerp(startDutch, endTilt, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        recomposer.m_Dutch = endTilt;
    }
}
