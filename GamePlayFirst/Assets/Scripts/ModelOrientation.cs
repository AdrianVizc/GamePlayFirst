using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelOrientation : MonoBehaviour
{
    private Movement movement;

    private void Start()
    {
        movement = transform.parent.GetComponent<Movement>();
    }

    // Late update to let it run after all other update and fixedupdate functions are done
    private void LateUpdate()
    {
        if(movement.isGrounded)
        {
            // Cast downward to find ground normal
            if (Physics.Raycast(transform.parent.position, Vector3.down, out RaycastHit hit, 1.5f, movement.groundLayer))
            {
                Vector3 groundNormal = hit.normal;

                // Get current Y rotation (we want to keep facing direction)
                float currentYRotation = transform.eulerAngles.y;

                // Create a rotation aligned with the ground normal
                Quaternion alignToGround = Quaternion.FromToRotation(Vector3.up, groundNormal);

                // Apply alignment while keeping Y rotation the same
                Quaternion targetRotation = alignToGround * Quaternion.Euler(0f, currentYRotation, 0f);

                // Smooth it if you want, otherwise direct:
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.deltaTime);
            }
        }
        else
        {
            // If not grounded, reset X/Z rotation to upright
            Quaternion uprightRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, uprightRotation, 10f * Time.deltaTime);
        }
    }
}
