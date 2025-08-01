using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpinButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] Image buttonImg;

    [Header("Bounding Box")]
    public float resetWidth = 200f;
    public float resetHeight = 100f;

    [Header("Animation")]
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

        CheckExitBoundary();

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

    void CheckExitBoundary()
    {
        // Check if cursor is outside a certain distance
        Vector2 diff = (Vector2)Input.mousePosition - (Vector2)buttonRect.position;
        bool isOutside = Mathf.Abs(diff.x) > resetWidth / 2f || Mathf.Abs(diff.y) > resetHeight / 2f;

        if (isOutside && hasSpun && !exitTriggered)
        {
            exitTriggered = true;
        }
    }
}