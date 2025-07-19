using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class WallSide : MonoBehaviour
{
    [Header("Which Sides are Rideable?")]
    [SerializeField] private bool leftWall;
    [SerializeField] private bool rightWall;

    private GameObject leftWallObj;
    private GameObject rightWallObj;

    private void Start()
    {
        leftWallObj = transform.Find("WallFacadeLeft").gameObject;
        rightWallObj = transform.Find("WallFacadeRight").gameObject;

        leftWallObj.SetActive(false);
        rightWallObj.SetActive(false);

        if (leftWall)
        {
            leftWallObj.SetActive(true);
        }
        if (rightWall)
        {
            rightWallObj.SetActive(true);
        }
    }
}
