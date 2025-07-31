using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class PixelPerfectCameraSnap : MonoBehaviour
{
    public float pixelsPerUnit = 64f;

    private void LateUpdate()
    {
        float unitsPerPixel = 1f / pixelsPerUnit;
        Vector3 newPos = transform.position;

        newPos.x = Mathf.Round(newPos.x / unitsPerPixel) * unitsPerPixel;
        newPos.y = Mathf.Round(newPos.y / unitsPerPixel) * unitsPerPixel;
        newPos.z = Mathf.Round(newPos.z / unitsPerPixel) * unitsPerPixel;

        transform.position = newPos;
    }
}