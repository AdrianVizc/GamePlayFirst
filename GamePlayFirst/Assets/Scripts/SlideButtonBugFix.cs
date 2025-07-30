using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideButtonBugFix : MonoBehaviour
{
    [SerializeField] private SlideButton[] slideButtons;

    private void OnDisable()
    {
        foreach (SlideButton button in slideButtons)
        {
            button.BugFix();
        }
    }
}
