using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Runtime.CompilerServices;


public class Stopwatch : MonoBehaviour
{
    private bool stopwatchActive;
    private float currentTime;
    [SerializeField] private TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {
        currentTime = 0;
        stopwatchActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (stopwatchActive)
        {
            currentTime = currentTime + Time.deltaTime;
        }
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        text.text = time.Minutes.ToString() + ":" + time.Seconds.ToString() + ":" + time.Milliseconds.ToString();
    }
    //Stop timer when game ends
    public void StopTimer()
    {
        stopwatchActive = false;
    }
}
