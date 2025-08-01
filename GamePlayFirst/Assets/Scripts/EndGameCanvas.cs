using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameCanvas : MonoBehaviour
{
    public static EndGameCanvas instance;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text totalScoreText;
    [SerializeField] private TMP_Text time;
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private float smallDelayBeforeTotalScore = 0.5f;
    [SerializeField] private float effectSpeed;

    private float currentDisplayScore = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Startup()
    {
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;

        AudioManager.instance.EndGameStopSounds();

        pauseMenu.SetActive(false);

        Stopwatch.instance.StopTimer();
        time.text = Stopwatch.instance.timerText.text;
        LeanTween.move(panel, gameObject.transform, duration).setEaseInCubic().setIgnoreTimeScale(true);
        StartCoroutine(DisplayTotalScore(ScoreCombo.Instance.totalScore));
    }

    public void ContinueButton()
    {
        Time.timeScale = 1f;

        AudioManager.instance.StopUISound("PointAddUp");
        AudioManager.instance.StopUISound("TotalPoints");

        SceneManager.LoadScene(PersistentManager.Instance.GetStringPref("EndCutscene").Value);
    }

    private IEnumerator DisplayTotalScore(float totalScore)
    {
        yield return new WaitForSecondsRealtime(duration + smallDelayBeforeTotalScore);

        AudioManager.instance.PlayUISound("PointAddUp");

        while (currentDisplayScore < totalScore)
        {
            currentDisplayScore += Time.unscaledDeltaTime + effectSpeed; // or whatever to get the speed you like
            currentDisplayScore = Mathf.Clamp(currentDisplayScore, 0f, totalScore);
            totalScoreText.text = currentDisplayScore.ToString("F0");
            yield return null;
        }

        AudioManager.instance.StopUISound("PointAddUp");
        AudioManager.instance.PlayUISound("TotalPoints");
    }
}
