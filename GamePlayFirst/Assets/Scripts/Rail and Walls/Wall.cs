using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWall : MonoBehaviour
{
    private bool TEMP_isGrounded;

    private void FixedUpdate()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("wall") && !TEMP_isGrounded)
        {

        }
    }
}
