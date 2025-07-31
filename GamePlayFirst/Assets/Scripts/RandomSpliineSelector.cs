using UnityEngine;
using UnityEngine.Splines;

public class RandomSplineSelector : MonoBehaviour
{
    [SerializeField] private SplineAnimate splineAnimate;
    [SerializeField] private GameObject[] splineObjects;

    void Start()
    {
        if (splineObjects.Length == 0 || splineAnimate == null)
        {
            Debug.LogWarning("No splines assigned or SplineAnimate is missing.");
            return;
        }

        int randomIndex = Random.Range(0, splineObjects.Length);
        SplineContainer selectedSpline = splineObjects[randomIndex].GetComponent<SplineContainer>();

        if (selectedSpline != null)
        {
            splineAnimate.Container = selectedSpline;
            splineAnimate.Play(); // Optional: Start playing immediately
        }
        else
        {
            Debug.LogWarning("Selected object does not have a SplineContainer.");
        }
    }
}
