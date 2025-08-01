using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class PlayerGrind : MonoBehaviour
{
    [HideInInspector] public bool onRail;

    [Header("Rail Settings")]
    [SerializeField] private float grindSpeed = 10f;
    [SerializeField] float heightOffset; // How high player sits above rail
    [SerializeField] private float lerpSpeed = 10f; // How fast player rotates along rail
    [SerializeField] private float jumpForce = 7.5f;
    private float timeForFullSpline;
    private float elapsedTime;
    private Rail currentRailScript;

    [Header("Jump Settings")]
    private float angleLimitThreshold = 20f;

    [Header("Camera Settings")]
    [SerializeField] private float FOV = 70f;

    Rigidbody rb;
    private bool isJumping;
    private bool isRailTagIgnored;
    private Movement movement;
    private Animator animator;
    private ParticleSystem sparksVFX;

    private CinemachineVirtualCamera virtualCamera;
    private float defaultFOV;

    private GameObject[] listOfRideables = new GameObject[2];

    private void Start()
    {
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        defaultFOV = virtualCamera.m_Lens.FieldOfView;
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();
        animator = GetComponentInChildren<Animator>();
        timeForFullSpline = 0;
        elapsedTime = 0;
        isJumping = false;
        isRailTagIgnored = false;
        sparksVFX = transform.Find("Sparks").gameObject.GetComponent<ParticleSystem>();
        sparksVFX.Stop();
    }

    private void Update()
    {
        if (onRail)
        {
            movement.canDash = true;
            if (Input.GetButtonDown("Jump") && !IsRailTooVertical())
            {
                isJumping = true;
                movement.canDoubleJump = true;
            }

            animator.SetBool("isRailVert", IsRailTooVertical());
            MovePlayerAlongRail();
        }
    }

    private void MovePlayerAlongRail()
    {
        if (!AudioManager.instance.railGrindPlayOnce)
        {
            AudioManager.instance.PlayEnvironmentSound("RailGrind");
            AudioManager.instance.railGrindPlayOnce = true;
        }

        if (currentRailScript != null)
        {
            float progress = elapsedTime / timeForFullSpline;

            // If we’re past the ends of the spline, get off the rail
            if(isJumping)
            {
                isJumping = false;
                StartCoroutine(ChangeFOV(virtualCamera, defaultFOV, 0.3f));
                JumpOff();
                return;
            }
            if (progress < -1 || progress > 1)
            {
                StartCoroutine(ChangeFOV(virtualCamera, defaultFOV, 0.3f));
                ThrowOffRail();
                return;
            }

            float nextTimeNormalized;
            if(currentRailScript.normalDir)
            {
                nextTimeNormalized = (elapsedTime + Time.deltaTime) / timeForFullSpline;
            }
            else
            {
                nextTimeNormalized = (elapsedTime - Time.deltaTime) / timeForFullSpline;
            }

            float3 pos, tangent, up;
            float3 nextPosFloat, nextTan, nextUp;

            // Get current and next spline points for position and orientation
            SplineUtility.Evaluate(currentRailScript.railSpline.Spline, progress, out pos, out tangent, out up);
            SplineUtility.Evaluate(currentRailScript.railSpline.Spline, nextTimeNormalized, out nextPosFloat, out nextTan, out nextUp);

            Vector3 worldPos = currentRailScript.LocalToWorldConversion(pos);
            Vector3 nextPos = currentRailScript.LocalToWorldConversion(nextPosFloat);

            // Position player slightly above rail, then rotate smoothly toward movement direction
            transform.position = worldPos + (transform.up * heightOffset);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(nextPos - worldPos), lerpSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up, up) * transform.rotation, lerpSpeed * Time.deltaTime);
            
            if(currentRailScript.normalDir)
            {
                elapsedTime += Time.deltaTime;
            }
            else
            {
                elapsedTime -= Time.deltaTime;
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        // If list of rideables are full and the game object is new, empty the array
        if (listOfRideables[1] != null && (collision.gameObject != listOfRideables[1]))
        {
            System.Array.Clear(listOfRideables, 0, listOfRideables.Length);
        }

        // If list of rideables are empty, set index 1 to game object
        if (listOfRideables[0] == null)
        {
            listOfRideables[0] = collision.gameObject;
        }
        else
        {
            //If index 1 is full, set index 2 to current game object
            listOfRideables[1] = collision.gameObject;

            if (listOfRideables[0] != listOfRideables[1])
            {
                System.Array.Clear(listOfRideables, 0, listOfRideables.Length);
            }
            else if (isRailTagIgnored)
            {
                return;
            }
        }

        // When player touches a rail, start grinding
        if (collision.gameObject.CompareTag("rail"))
        {
            sparksVFX.Play();
            onRail = true;
            animator.SetBool("onRail", true);
            animator.SetBool("isGrounded", false);
            AudioManager.instance.PlayEnvironmentSound("RailEnter");
            AudioManager.instance.StopEnvironmentSound("Skate");

            currentRailScript = collision.gameObject.GetComponent<Rail>();
            CalculateAndSetRailPosition();
            GetComponent<Movement>().enabled = false;

            StartCoroutine(ChangeFOV(virtualCamera, FOV, 0.3f));
        }
    }

    private void CalculateAndSetRailPosition()
    {
        timeForFullSpline = currentRailScript.totalSplineLength / grindSpeed;

        Vector3 splinePoint;
        // Find closest point on spline to player and get normalized time along rail
        float normalizedTime = currentRailScript.CalculateTargetRailPoint(transform.position, out splinePoint);

        elapsedTime = timeForFullSpline * normalizedTime;

        float3 pos, forward, up;
        SplineUtility.Evaluate(currentRailScript.railSpline.Spline, normalizedTime, out pos, out forward, out up);

        // Decide which direction along rail to grind based on player facing
        // Force direction toward world +Z
        Vector3 worldTangent = currentRailScript.transform.TransformDirection(forward);
        currentRailScript.normalDir = Vector3.Dot(worldTangent.normalized, Vector3.forward) >= 0f;

        // Snap player position and rotation exactly on spline start point
        transform.position = splinePoint + (Vector3.up * heightOffset);

        Quaternion targetRot = Quaternion.LookRotation(forward, up);
        transform.rotation = targetRot;

        // Clear any velocity so player doesn’t carry over momentum
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void ThrowOffRail()
    {
        sparksVFX.Stop();
        animator.SetBool("onRail", false);
        movement.canDoubleJump = true;
        transform.position += Vector3.ProjectOnPlane(transform.forward, Vector3.up) * 1f; // Move player forward slightly
        EndRail();

        transform.rotation = Quaternion.identity;

        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        movement.TemporarilyDisableRotation(0.1f);
    }

    private void JumpOff()
    {
        sparksVFX.Stop();
        animator.SetBool("onRail", false);
        animator.SetBool("isJumping", true);
        AudioManager.instance.railGrindPlayOnce = false;
        AudioManager.instance.PlayEnvironmentSound("Jump");
        AudioManager.instance.StopEnvironmentSound("RailGrind");

        onRail = false;
        isJumping = false;

        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Reset rotation to flat to remove skew from grinding
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        movement.TemporarilyDisableRotation(0.1f);

        // Move slightly up before jumping, to clear the rail
        transform.position += Vector3.up * 0.5f;

        // Stop any existing movement
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Apply vertical jump force only
        Vector3 finalJump = transform.up * jumpForce;

        // Optional: add slight push in forward direction too
        // float forwardBoost = 2f;
        finalJump += transform.forward;// * forwardBoost;

        // Get the current rail normal
        float progress = elapsedTime / timeForFullSpline;
        float3 pos, tangent, up;
        SplineUtility.Evaluate(currentRailScript.railSpline.Spline, progress, out pos, out tangent, out up);

        // Convert spline-space normal to world space
        Vector3 worldNormal = currentRailScript.LocalToWorldConversion(up + pos) - currentRailScript.LocalToWorldConversion(pos);
        worldNormal.Normalize();

        // Apply a small outward push along the normal
        float normalPushForce = 2f; // tweak this value as needed
        finalJump += worldNormal * normalPushForce;

        rb.AddForce(finalJump, ForceMode.VelocityChange);

        StartCoroutine(IgnoreRailTemporarily(0.5f));
        StartCoroutine(ForceUprightRotationForFrames()); // safety net to reapply upward rotation

        // Re-enable movement
        GetComponent<Movement>().enabled = true;

        currentRailScript = null;
    }

    private IEnumerator IgnoreRailTemporarily(float duration)
    {
        isRailTagIgnored = true;
        yield return new WaitForSeconds(duration); // ignore rail tag for x sec
        isRailTagIgnored = false;
    }

    private IEnumerator ForceUprightRotationForFrames(float duration = 0.1f)
    {
        float timer = 0f;
        while (timer < duration)
        {
            transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private void EndRail()
    {
        AudioManager.instance.railGrindPlayOnce = false;
        AudioManager.instance.StopEnvironmentSound("RailGrind");

        onRail = false;
        currentRailScript = null;

        // Project forward onto horizontal plane for stable exit rotation
        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        if (flatForward.sqrMagnitude < 0.001f)
            flatForward = Vector3.forward; // fallback if too vertical
        flatForward.Normalize();

        // Set rotation flat on horizontal plane
        //transform.rotation = Quaternion.LookRotation(flatForward, Vector3.up);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0); // Flatten pitch/roll
        movement.TemporarilyDisableRotation(0.2f);

        // Zero velocities to stop spinning/moving
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;

        // Freeze rotation temporarily to prevent physics from spinning player
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        transform.position += flatForward * 1f; // Move player forward slightly

        GetComponent<Movement>().enabled = true;

        movement.slidOffWallRail = true;

        // unfreeze rotation after a short delay
        //StartCoroutine(UnfreezeRotation(rb, 0.1f));
    }
    private bool IsRailTooVertical()
    {
        if (currentRailScript == null || timeForFullSpline == 0) return true;

        float progress = elapsedTime / timeForFullSpline;
        float3 pos, tangent, up;
        SplineUtility.Evaluate(currentRailScript.railSpline.Spline, progress, out pos, out tangent, out up);

        Vector3 worldTangent = currentRailScript.LocalToWorldConversion(pos + tangent) - currentRailScript.LocalToWorldConversion(pos);
        worldTangent.Normalize();

        float angle = Vector3.Angle(worldTangent, Vector3.up);

        // If the angle is close to 0 or 180, it's very vertical
        return angle < (0 + angleLimitThreshold) || angle > (180 - angleLimitThreshold);
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

    private IEnumerator UnfreezeRotation(Rigidbody rb, float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.constraints = RigidbodyConstraints.None;
    }
}
