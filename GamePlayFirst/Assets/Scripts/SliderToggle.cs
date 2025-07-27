using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderToggle : MonoBehaviour, IPointerClickHandler
{
    //For the slider to function as a toggle, the "Interactable" bool in the inspector must be false

    [SerializeField] private Slider slider;

    private void Awake()
    {
        slider.value = ES3.Load("VsyncToggle", PersistentManager.Instance.GetIntPref("VsyncToggle").GetDefaultValue());
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
