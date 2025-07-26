using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabSelect : MonoBehaviour
{
    [SerializeField] private Image respectiveImage;
    [SerializeField] private HoverOver hoverOverScript;

    [Space]

    [SerializeField] Sprite unselectedSprite;
    [SerializeField] Sprite selectedSprite;

    private void OnEnable()
    {
        respectiveImage.sprite = selectedSprite;

        hoverOverScript.enabled = false;
    }

    private void OnDisable()
    {
        respectiveImage.sprite = unselectedSprite;

        hoverOverScript.enabled = true;
    }
}
