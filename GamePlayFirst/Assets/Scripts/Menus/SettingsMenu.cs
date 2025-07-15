using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : Menu
{
    public static SettingsMenu Instance; //Code to make script into a singleton

    const int FIRST_SETTINGS_MENU = 0;

    private void OnEnable()
    {
        menusToOpen[FIRST_SETTINGS_MENU].gameObject.SetActive(true);
    }

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
}
