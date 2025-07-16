using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class PlayerGrind : MonoBehaviour
{
    [Header("Rail Settings")]
    [SerializeField] private float grindSpeed = 10f;
    [SerializeField] float heightOffset; // How high player sits above rail
    [SerializeField] private float lerpSpeed = 10f; // How fast player rotates along rail
    [SerializeField] private float jumpForce = 7.5f;
    private bool onRail;
    private float timeForFullSpline;
    private float elapsedTime;
    private Rail currentRailScript;

    [Header("Jump Settings")]
    private float angleLimitThreshold = 20f;


    Rigidbody rb;
    private bool isJumping;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        timeForFullSpline = 0;
        elapsedTime = 0;
        isJumping = false;
    }

    private void Update()
    {
        if (onRail)
        {
            if(Input.GetKeyDown(KeyCode.Space) && !IsRailTooVertical())
            {
                isJumping = true;
            }
            MovePlayerAlongRail();
        }
    }

    private void MovePlayerAlongRail()
    {
        if(currentRailScript != null)
        {
            float progress = elapsedTime / timeForFullSpline;

            // If we’re past the ends of the spline, get off the rail
            if(isJumping)
            {
                isJumping = false;
                JumpOff();
                return;
            }
            if (progress < -1 || progress > 1)
            {
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
        // When player touches a rail, start grinding
        if (collision.gameObject.CompareTag("rail"))
        {
            onRail = true;
            currentRailScript = collision.gameObject.GetComponent<Rail>();
            CalculateAndSetRailPosition();
            GetComponent<Movement>().enabled = false;
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
        currentRailScript.CalculateDirection(forward, transform.forward);

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
        transform.position += Vector3.ProjectOnPlane(transform.forward, Vector3.up) * 1f; // Move player forward slightly
        EndRail();
    }

    private void JumpOff()
    {
        EndRail();

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        transform.rotation = Quaternion.identity;
    }

    private void EndRail()
    {
        onRail = false;
        currentRailScript = null;

        // Project forward onto horizontal plane for stable exit rotation
        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        if (flatForward.sqrMagnitude < 0.001f)
            flatForward = Vector3.forward; // fallback if too vertical
        flatForward.Normalize();

        // Set rotation flat on horizontal plane
        transform.rotation = Quaternion.LookRotation(flatForward, Vector3.up);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0); // Flatten pitch/roll

        // Zero velocities to stop spinning/moving
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;

        // Freeze rotation temporarily to prevent physics from spinning player
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        transform.position += flatForward * 1f; // Move player forward slightly

        GetComponent<Movement>().enabled = true;

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

    private IEnumerator UnfreezeRotation(Rigidbody rb, float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.constraints = RigidbodyConstraints.None;
    }
}
