using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpinButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image buttonImg;

    public float rotateAngle = 370f; // degrees
    public float scaleUpFactor = 1.7f;
    public float rotateDuration = 0.5f;
    public float scaleDuration = 1f;

    private RectTransform buttonRect;
    private Vector3 originalScale;
    private bool isSpinning = false;
    private bool exitQueued = false;

    void Start()
    {
        if (buttonImg == null)
        {
            Debug.LogWarning("Button: Image missing!");
            return;
        }

        buttonRect = buttonImg.rectTransform;
        originalScale = buttonRect.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSpinning) return;

        isSpinning = true;
        exitQueued = false;

        // Spins the button
        LeanTween.value(gameObject, 0f, 375f, 1f)
            .setEaseOutSine()
            .setOnUpdate((float angle) =>
            {
                buttonRect.localRotation = Quaternion.Euler(0, 0, angle);
            })
            .setOnComplete(() =>
            {
                isSpinning = false;

                // if (exitQueued)
                // {
                //     ResetRotation();
                // }
            });

        // Scales the button up and down
        LeanTween.scale(buttonRect, originalScale * scaleUpFactor, scaleDuration).setEaseOutBack();
        LeanTween.scale(buttonRect, originalScale, scaleDuration).setEaseOutBack();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSpinning)
        {
            ResetRotation();
        }
        else if (!isSpinning && )
        {
            exitQueued = true;
        }
        // Expected Behavior: When done spinning, if the pointer is outside, then reset the position.
        // If it's currently spinning, DO NOT RESET UNLESS THE POINTER IS OUTSIDE THE BOX
        // One Case: When the button finishes spinning, we can check if the pointer is outside (exitQueued) and rotate back if so.
        // Problem: When the mouse goes outside due to the rotation of the recttransform, it calls the OnPointerExit which causes an early return animation.
        // It's discrete events, so this bool setting only happens once.
        // Try to find another way...
        // Remember thought that the parent box won't work since the Event Handers target UI elements
        // Another way: Check a certain radius that the mouse is away from and use this to update whether the button should spin out or not
    }

    public void ResetRotation()
    {
        LeanTween.rotateZ(buttonRect.gameObject, 0f, rotateDuration).setEaseOutSine();
    }
}
