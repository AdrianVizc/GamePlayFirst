using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIPopup : MonoBehaviour
{
    [SerializeField] private Sprite[] listOfBackgrounds;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image textImage;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float waitBeforeFade = 1f;

    private void Start()
    {
        backgroundImage.sprite = listOfBackgrounds[Random.Range(0, listOfBackgrounds.Length)];

        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        Color originalColorBackground = backgroundImage.color;
        Color originalColorText = textImage.color;

        yield return new WaitForSeconds(waitBeforeFade);

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(originalColorBackground.a, 0f, elapsedTime / fadeDuration);
            backgroundImage.color = new Color(originalColorBackground.r, originalColorBackground.g, originalColorBackground.b, alpha);
            textImage.color = new Color(originalColorText.r, originalColorText.g, originalColorText.b, alpha);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        Destroy(gameObject);
    }
}
