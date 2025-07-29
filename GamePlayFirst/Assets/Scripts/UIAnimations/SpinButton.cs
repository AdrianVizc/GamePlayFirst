using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpinButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] Image buttonImg;

    public float resetDistance = 200f; // In pixels. Adjust as needed to modify spin behavior.
    public float resetDuration = 0.3f;
    public float rotateAngle = 375f;
    public float scaleUpFactor = 1.7f;
    public float rotateDuration = 1f;
    public float scaleDuration = 1f;

    private RectTransform buttonRect;
    private Vector3 originalScale;

    private bool isSpinning = false;
    private bool hasSpun = false;
    private bool exitTriggered = false;

    void Start()
    {
        buttonRect = buttonImg.rectTransform;
        originalScale = buttonRect.localScale;
    }

    void Update()
    {
        if (buttonRect == null) return;

        // Check if cursor is outside a certain distance
        bool isOutside = !RectTransformUtility.RectangleContainsScreenPoint(buttonRect, Input.mousePosition, null)
                        && Vector2.Distance(buttonRect.position, Input.mousePosition) > resetDistance;

        // If user leaves the button, mark for reset after spin
        if (isOutside && hasSpun && !exitTriggered)
        {
            exitTriggered = true;
        }

        if (!isSpinning && exitTriggered && buttonRect.localRotation.eulerAngles.z != 0f)
        {
            ResetRotation();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSpinning && !hasSpun)
        {
            isSpinning = true;
            hasSpun = true;
            exitTriggered = false;
            Spin();
        }
    }

    void Spin()
    {
        LeanTween.value(gameObject, 0f, rotateAngle, rotateDuration)
            .setEaseOutSine()
            .setOnUpdate((float angle) =>
            {
                buttonRect.localRotation = Quaternion.Euler(0, 0, angle);
            })
            .setOnComplete(() =>
            {
                isSpinning = false;
                if (exitTriggered)
                {
                    ResetRotation();
                }
            });

        // Animate scale up and down
        LeanTween.scale(buttonRect, originalScale * scaleUpFactor, scaleDuration / 2)
            .setEaseOutBack()
            .setOnComplete(() =>
            {
                LeanTween.scale(buttonRect, originalScale, scaleDuration / 2).setEaseOutBack();
            });
    }

    void ResetRotation()
    {
        LeanTween.value(gameObject, buttonRect.localRotation.eulerAngles.z, 0f, resetDuration)
            .setEaseOutCubic()
            .setOnUpdate((float angle) =>
            {
                buttonRect.localRotation = Quaternion.Euler(0, 0, angle);
            })
            .setOnComplete(() =>
            {
                // Allow another spin on next hover
                hasSpun = false;
                exitTriggered = false;
            });
    }
}