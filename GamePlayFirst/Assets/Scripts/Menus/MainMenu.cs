using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Menu
{
    public static MainMenu Instance; //Code to make script into a singleton

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

    public void Play()
    {
        Debug.Log("Play Game");
    }
}
