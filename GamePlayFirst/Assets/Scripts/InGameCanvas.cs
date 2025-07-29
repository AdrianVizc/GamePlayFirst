using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameCanvas : MonoBehaviour
{
    public static InGameCanvas instance;

    [SerializeField] private TMP_Text trickPoints;
    [SerializeField] private TMP_Text multiplier;
    [SerializeField] private int combo;

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
        Scene thisScene = SceneManager.GetSceneByName(PersistentManager.Instance.GetStringPref("PlayScene").Value);

        if (thisScene.IsValid())
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void UpdateTrickPoints(float num)
    {
        trickPoints.text = num.ToString();
    }

    public void UpdateMultiplier(float num)
    {
        multiplier.text = num.ToString();
    }

    public void UpdateCombo()
    {

    }
}
