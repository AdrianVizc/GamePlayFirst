using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Runtime.CompilerServices;


public class Stopwatch : MonoBehaviour
{
    public static Stopwatch instance;

    [SerializeField] public TMP_Text timerText;

    private bool stopwatchActive;
    private float currentTime;

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

    void Start()
    {
        currentTime = 0;
        stopwatchActive = true;
    }

    void Update()
    {
        if (stopwatchActive)
        {
            currentTime += Time.deltaTime;
        }
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        timerText.text = time.Minutes.ToString("D2") + ":" + time.Seconds.ToString("D2");
    }

    public void StopTimer()
    {
        stopwatchActive = false;
    }
}
