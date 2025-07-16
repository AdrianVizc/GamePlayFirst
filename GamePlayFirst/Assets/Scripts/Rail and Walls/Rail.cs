using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class Rail : MonoBehaviour
{
    public bool normalDir; // Dictates which direction you go down spline (0 to 1 or 1 to 0)
    public SplineContainer railSpline;
    public float totalSplineLength;

    private void Start()
    {
        railSpline = GetComponent<SplineContainer>();
        totalSplineLength = railSpline.CalculateLength();
    }

    // Converts local float3 positions to Vector3 world positions
    public Vector3 LocalToWorldConversion(float3 localPoint)
    {
        return transform.TransformPoint(localPoint);
    }

    // Converts Vector3 world positions to local float3 positions
    public float3 WorldToLocalConversion(Vector3 worldPoint)
    {
        return transform.InverseTransformPoint(worldPoint);
    }

    // Calculates the normalized time value for the rail's spline
    // Outputs the normalized time and outputs the position where the player moves to (worldPosOnSpline)
    public float CalculateTargetRailPoint(Vector3 playerPos, out Vector3 worldPosOnSpline)
    {
        float3 nearestPoint;
        float time;

        float3 localPlayerPos = WorldToLocalConversion(playerPos);
        SplineUtility.GetNearestPoint(railSpline.Spline, localPlayerPos, out nearestPoint, out time);

        worldPosOnSpline = LocalToWorldConversion(nearestPoint);
        return time;
    }

    // Compares the forward of the rail and forward of the player
    // Calculates the angle between the two forwards and normalize it
    // Sees how large the angle is between the twoe
    public void CalculateDirection(float3 railForward, Vector3 playerForward)
    {
        float angle = Vector3.Angle(railForward, playerForward.normalized);
        if(angle > 90f)
        {
            normalDir = false;
        }
        else
        {
            normalDir = true;
        }

    }
}
