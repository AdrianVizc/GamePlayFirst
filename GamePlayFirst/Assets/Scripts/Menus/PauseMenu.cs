using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class PauseMenu : Menu
{
    public static PauseMenu Instance; //Code to make script into a singleton

    private void Awake()
    {
        if (Instance == null) //Code to make script into a singleton
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);

            return;
        }
    }

    private void Update() //temp for testing
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PersistentManager.Instance.GetBoolPref("isGamePaused").Value)
        {
            var temp = PersistentManager.Instance.GetBoolPref("isGamePaused");
            temp.SetValue(false);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && !PersistentManager.Instance.GetBoolPref("isGamePaused").Value)
        {
            var temp = PersistentManager.Instance.GetBoolPref("isGamePaused");
            temp.SetValue(true);
        }
        Debug.Log(PersistentManager.Instance.GetBoolPref("isGamePaused").Value);
    }
}
