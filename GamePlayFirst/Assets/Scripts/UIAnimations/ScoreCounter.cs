using UnityEngine;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;

    [Header("Animation")]
    [SerializeField] float countDuration = 0.5f;
    [SerializeField] float pulseScale = 1.3f;
    [SerializeField] float pulseDuration = 0.2f;

    private RectTransform textTransform;
    private int currentScore = 0;
    private bool isAnimating = false;

    // Example: AddScore(5);
    void Start()
    {
        textTransform = scoreText.GetComponent<RectTransform>();
        AddScore(5);
    }

    public void AddScore(int amount)
    {
        if (isAnimating) return;

        int newScore = currentScore + amount;
        AnimateScore(currentScore, newScore);
        currentScore = newScore;
    }

    void AnimateScore(int from, int to)
    {
        isAnimating = true;

        LeanTween.value(gameObject, from, to, countDuration)
            .setEaseOutCubic()
            .setOnUpdate((float val) => {
                scoreText.text = Mathf.FloorToInt(val).ToString();
            })
            .setOnComplete(() => {
                PulseText(() => {
                    isAnimating = false;
                });
            });
    }

    void PulseText(System.Action onComplete)
    {
        Vector3 originalScale = textTransform.localScale;

        LeanTween.scale(textTransform, originalScale * pulseScale, pulseDuration)
            .setEaseOutBack()
            .setOnComplete(() => {
                LeanTween.scale(textTransform, originalScale, pulseDuration)
                    .setEaseInBack()
                    .setOnComplete(() => {
                        onComplete?.Invoke();
                    });
            });
    }
}
