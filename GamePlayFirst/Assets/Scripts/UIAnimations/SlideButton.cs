using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlideButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image buttonImg;

    public float slideDistance = 5f; // how far it slides out
    public float duration = 0.3f;

    private RectTransform buttonRect;
    private Vector2 originalPos;

    void Start()
    {
        if (buttonImg == null)
        {
            Debug.LogWarning("Button: Image missing!");
            return;
        }

        buttonRect = buttonImg.rectTransform;
        originalPos = buttonRect.anchoredPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector2 targetPos = buttonRect.anchoredPosition + new Vector2(slideDistance, 0);
        LeanTween.move(buttonRect, targetPos, duration).setEaseInCubic().setIgnoreTimeScale(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.move(buttonRect, originalPos, duration).setEaseInCubic().setIgnoreTimeScale(true);
    }

    public void BugFix()
    {
        LeanTween.move(buttonRect, originalPos, duration).setEaseInCubic().setIgnoreTimeScale(true);
    }
}
