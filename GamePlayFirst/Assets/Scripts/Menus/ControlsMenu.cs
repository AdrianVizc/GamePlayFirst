using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsMenu : Menu
{
    private void OnEnable()
    {
        SettingsMenu.Instance.ChangeBackground(2);
    }
}
