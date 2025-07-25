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
    //[SerializeField] private float jumpTimer = 0.75f;
    //private float wallJumpCooldown;
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
    private bool wallFront;
    private bool wallHit;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    private RaycastHit forwardWallHit;

    private bool isGrounded;
    private Transform orientation;
    private Rigidbody rb;
    [HideInInspector] public bool isWallRunning;
    private bool isWallJumping;
    private bool wallDetected;
    private bool wallCooldownReady;

    private void Start()
    {
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        defaultFOV = virtualCamera.m_Lens.FieldOfView;
        defaultTilt = virtualCamera.GetComponent<CinemachineRecomposer>().m_Dutch;

        //wallJumpCooldown = 0.2f;
        wallJumpTimer = 0f;

        orientation = transform;
        isWallJumping = false;
        wallDetected = false;
        wallCooldownReady = true;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
            return; // Skip wall-run logic briefly after jumping
        }

        if (!isGrounded && wallDetected)
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

                EndWallRun(); // handles camera + state reset

                // Immediately hand control back to movement
                EnableMovement();

                // Block wall re-entry for a short cooldown
                StartCoroutine(WallCooldownDelay(0.2f));
            }
        }
        else if (isWallRunning && !isWallJumping && (isGrounded || !wallHit))
        {
            EndWallRun();
            EnableMovement();
        }
    }

    private void FixedUpdate()
    {
        isGrounded = GetComponent<Movement>().isGrounded;
    }

    private void CheckForWall()
    {
        float castRadius = 0.3f;

        wallRight = Physics.SphereCast(transform.position, castRadius, orientation.right, out rightWallhit, wallCheckDistance);
        wallLeft = Physics.SphereCast(transform.position, castRadius, -orientation.right, out leftWallhit, wallCheckDistance);
        if(!(wallRight || wallLeft))
        {
            wallFront = Physics.SphereCast(transform.position, castRadius, orientation.forward, out forwardWallHit, wallCheckDistance);
        }

        bool validWall = (wallRight && rightWallhit.collider.CompareTag("wall")) ||
                         (wallLeft && leftWallhit.collider.CompareTag("wall")) ||
                         (wallFront && forwardWallHit.collider.CompareTag("wall"));

        wallHit = validWall;
    }

    private void StartWallRun()
    {
        isWallRunning = true;
        GetComponent<Movement>().enabled = false;
        rb.useGravity = false;

        // Grab the correct wall normal
        Vector3 rawNormal;
        if (wallRight) rawNormal = rightWallhit.normal;
        else if (wallLeft) rawNormal = leftWallhit.normal;
        else if (wallFront) rawNormal = forwardWallHit.normal;
        else rawNormal = -transform.forward; // fallback

        storedWallNormal = new Vector3(rawNormal.x, 0f, rawNormal.z).normalized;

        StartCoroutine(ChangeFOV(virtualCamera, FOV, 0.3f));

        // Use side detection to determine camera tilt
        float sideDot = Vector3.Dot(storedWallNormal, transform.right);
        if (sideDot > 0.1f)
            StartCoroutine(ChangeTilt(virtualCamera, tilt, 0.3f));
        else if (sideDot < -0.1f)
            StartCoroutine(ChangeTilt(virtualCamera, -tilt, 0.3f));
        else
            StartCoroutine(ChangeTilt(virtualCamera, 0f, 0.3f)); // wall in front — no tilt
    }

    private void DoWallRun()
    {
        Vector3 wallDir = Vector3.Cross(Vector3.up, storedWallNormal);

        // Force wall run to always favor positive Z direction
        Vector3 worldForward = Vector3.forward;

        // Project the world forward onto the wall plane to make sure it's still aligned to the wall
        Vector3 projectedForward = Vector3.ProjectOnPlane(worldForward, storedWallNormal).normalized;

        // If the projection fails (wall is perpendicular to Z), fall back to default wallDir
        if (projectedForward.sqrMagnitude < 0.1f)
        {
            projectedForward = wallDir;
        }

        // Use forced forward-aligned direction
        wallDir = projectedForward;

        // Rotate player to face the direction of travel
        Quaternion targetRotation = Quaternion.LookRotation(wallDir.normalized, Vector3.up);
        transform.rotation = targetRotation;

        // Move the player
        rb.velocity = wallDir.normalized * wallRunSpeed;
    }

    private void EndWallRun()
    {
        wallHit = false;
        wallDetected = false;
        isWallJumping = false;
        isWallRunning = false;
        rb.useGravity = true;

        StartCoroutine(ChangeFOV(virtualCamera, defaultFOV, 0.3f));
        StartCoroutine(ChangeTilt(virtualCamera, defaultTilt, 0.3f));
    }

    private void EnableMovement()
    {
        if (!GetComponent<Movement>().enabled)
        {
            GetComponent<Movement>().enabled = true;
        }

        wallCooldownReady = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!wallCooldownReady)
        {
            return;
        }
        if (collision.gameObject.CompareTag("wall"))
        {
            wallDetected = true;
            wallCooldownReady = false;
        }
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
    private IEnumerator WallCooldownDelay(float delay)
    {
        wallCooldownReady = false;
        yield return new WaitForSeconds(delay);
        wallCooldownReady = true;
    }

}
