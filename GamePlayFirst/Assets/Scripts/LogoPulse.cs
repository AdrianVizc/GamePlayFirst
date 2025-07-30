using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoPulse : MonoBehaviour
{
    [SerializeField] private float pulseSpeed;
    [SerializeField] private float maxScale;
    [SerializeField] private float minScale;

    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        float scale = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
        transform.localScale = initialScale * scale;
    }
}
