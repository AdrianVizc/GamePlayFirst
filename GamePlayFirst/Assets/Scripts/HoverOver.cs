using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image imageComponent;
    [SerializeField] private Sprite unselectedSprite;
    [SerializeField] private Sprite hoverSprite;

    private void OnEnable()
    {
        imageComponent.sprite = unselectedSprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        imageComponent.sprite = hoverSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        imageComponent.sprite = unselectedSprite;
    }
}
