using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : Menu
{
    public static SettingsMenu Instance; //Code to make script into a singleton

    [SerializeField] private Sprite[] backgroundSprites;
    [SerializeField] private Image background;

    const int FIRST_SETTINGS_MENU = 0;

    private void OnEnable()
    {
        menusToOpen[FIRST_SETTINGS_MENU].gameObject.SetActive(true);
        background.sprite = backgroundSprites[FIRST_SETTINGS_MENU];
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

    public void ChangeBackground(int val)
    {
        background.sprite = backgroundSprites[val];
    }
}
