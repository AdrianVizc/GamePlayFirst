using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigRedButton : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        AudioManager.instance.PlayEnvironmentSound("BigRedButton");

        EndGameCanvas.instance.gameObject.SetActive(true);
        EndGameCanvas.instance.Startup();
    }
}
