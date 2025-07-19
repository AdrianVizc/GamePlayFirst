using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeForward : MonoBehaviour
{
    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 20f, Color.red);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Movement player = other.GetComponent<Movement>();
            if (player != null)
            {
                player.UpdateForwardDirection(this.transform.forward);
            }
        }
    }
}
