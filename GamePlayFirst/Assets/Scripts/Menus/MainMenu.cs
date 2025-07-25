using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : Menu
{
    public static MainMenu Instance; //Code to make script into a singleton

    private void Awake()
    {
        if (Instance == null) //Code to make script into a singleton
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);

            return;
        }
    }

    private void Start()
    {
        Scene thisScene = SceneManager.GetSceneByName(PersistentManager.Instance.GetStringPref("PlayScene").Value);

        if (thisScene.IsValid())
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    public void Play()
    {
        SceneManager.LoadScene(PersistentManager.Instance.GetStringPref("PlayScene").Value);

        SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);
    }
}
