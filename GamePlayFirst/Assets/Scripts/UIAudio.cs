using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.instance.PlayUISound("UIClick");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.instance.PlayUISound("UIHover");
    }
}
