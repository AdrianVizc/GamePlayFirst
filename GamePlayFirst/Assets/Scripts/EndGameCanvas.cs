using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameCanvas : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text totalScoreText;
    [SerializeField] private TMP_Text time;
    [SerializeField] private float duration = 0.3f;

    private void OnEnable()
    {
        pauseMenu.SetActive(false);        

        Stopwatch.instance.StopTimer();
        time.text = Stopwatch.instance.timerText.text;
        LeanTween.move(panel, gameObject.transform, duration).setEaseInCubic().setIgnoreTimeScale(true);
    }

    public void ContinueButton()
    {
        SceneManager.LoadScene(PersistentManager.Instance.GetStringPref("EndCutscene").Value);
    }

    private void DisplayTotalScore()
    {

    }
}
