using System.Collections;
using UnityEngine;
using TMPro;

public class PulsingNumber : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] float pulseScale = 1.2f;
    [SerializeField] float pulseDuration = 0.1f;
    [SerializeField] float pulseDelay = 0.3f;

    private RectTransform textTransform;
    private int currentScore = 0;
    private bool isAnimating = false;
    Vector3 originalScale;

    // Example Call: AddScore(5);
    void Start()
    {
        textTransform = scoreText.GetComponent<RectTransform>();
        originalScale = textTransform.localScale;
        AddScore(5);
    }

    public void AddScore(int amount)
    {
        if (isAnimating) return;

        int targetScore = currentScore + amount;
        StartCoroutine(AnimateScore(currentScore, targetScore));
        currentScore = targetScore;
    }

    IEnumerator AnimateScore(int start, int end)
    {
        isAnimating = true;

        for (int i = start + 1; i <= end; i++)
        {
            scoreText.text = i.ToString();
            PulseText();
            yield return new WaitForSeconds(pulseDelay);
        }

        isAnimating = false;
    }

    void PulseText()
    {
        LeanTween.cancel(textTransform);

        LeanTween.scale(textTransform, originalScale * pulseScale, pulseDuration)
            .setEaseOutBack()
            .setOnComplete(() =>
            {
                LeanTween.scale(textTransform, originalScale, pulseDuration)
                    .setEaseInBack();
            });
    }
}