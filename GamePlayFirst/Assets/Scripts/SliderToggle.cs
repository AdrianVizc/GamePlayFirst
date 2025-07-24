using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderToggle : MonoBehaviour, IPointerClickHandler
{
    //For the slider to function as a toggle, the "Interactable" bool in the inspector must be false

    [SerializeField] private Slider slider;
    [SerializeField] private Image handleImage;

    private Color lightGray = new Color(0.8f, 0.8f, 0.8f, 1); //light gray

    private void Update()
    {
        if (slider.value == 0)
        {
            handleImage.color = lightGray;
        }
        else
        {
            handleImage.color = Color.white;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (slider.value == 0)
        {
            slider.value = 1;
        }
        else
        {
            slider.value = 0;
        }
    }
}
