using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrickPopUp : MonoBehaviour
{
    [SerializeField] Image popupText;
    [SerializeField] Image popupBackground;

    RectTransform textRect;
    RectTransform bgRect;
    // Start is called before the first frame update
    void Start()
    {
        textRect = popupText.rectTransform;
        bgRect = popupBackground.rectTransform;
        
        BounceBG();
        BounceText();
    }

    void BounceBG()
    {
        bgRect.localScale = Vector3.zero;
        LeanTween.scale(bgRect, Vector3.one, 0.4f).setEaseOutBack(); // Idk if I need this?

        LTSeq seq = LeanTween.sequence();

        // Bounce scale values
        Vector3 normal = Vector3.one;
        Vector3 overshoot = new Vector3(1.2f, 1.2f, 1);
        Vector3 undershoot = new Vector3(0.9f, 0.9f, 1);

        // Scale up (first bounce)
        seq.append(LeanTween.scale(bgRect, overshoot, 0.15f).setEaseOutQuad());
        seq.append(LeanTween.scale(bgRect, undershoot, 0.1f).setEaseInQuad());

        // Second bounce
        seq.append(LeanTween.scale(bgRect, overshoot, 0.1f).setEaseOutQuad());
        seq.append(LeanTween.scale(bgRect, undershoot, 0.1f).setEaseInQuad());

        // Third bounce
        seq.append(LeanTween.scale(bgRect, normal, 0.1f).setEaseOutQuad());

        // Wait briefly before shrinking
        seq.append(0.2f);

        // Scale down to 0
        seq.append(LeanTween.scale(bgRect, Vector3.zero, 0.3f).setEaseInBack());
    }

    void BounceText()
    {
        textRect.localScale = Vector3.zero;

        LTSeq seq = LeanTween.sequence();

        // Bounce scale values
        Vector3 normal = new Vector3(0.3f, 0.3f, 1);
        Vector3 overshoot = new Vector3(1.2f, 1.2f, 1);
        Vector3 undershoot = new Vector3(0.9f, 0.9f, 1);

        // Scale up (first bounce)
        seq.append(LeanTween.scale(textRect, overshoot, 0.15f).setEaseOutQuad());
        seq.append(LeanTween.scale(textRect, undershoot, 0.1f).setEaseInQuad());

        // Second bounce
        seq.append(LeanTween.scale(textRect, overshoot, 0.1f).setEaseOutQuad());
        seq.append(LeanTween.scale(textRect, undershoot, 0.1f).setEaseInQuad());

        // Third bounce
        seq.append(LeanTween.scale(textRect, normal, 0.1f).setEaseOutQuad());

        // Wait briefly before shrinking
        // seq.append(0.2f);

        // Scale down to 0
        seq.append(LeanTween.scale(textRect, Vector3.zero, 0.3f).setEaseInBack());
    }
}
