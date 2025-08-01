using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigRedButton : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            AudioManager.instance.PlayEnvironmentSound("BigRedButton");

            EndGameCanvas.instance.gameObject.SetActive(true);
            EndGameCanvas.instance.Startup();
        }        
    }
}
