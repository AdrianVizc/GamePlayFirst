using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : Menu
{
    public static PauseMenu Instance; //Code to make script into a singleton

    [SerializeField] public GameObject background;

    private bool isPaused;

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
        isPaused = false;
    }

    private void Update()
    {
        PauseGame();
    }

    protected override void CloseMenus()
    {
        background.SetActive(false);
    }

    private void PauseGame()
    {
        Scene thisScene = SceneManager.GetSceneByName(PersistentManager.Instance.GetStringPref("PlayScene").Value);

        if (thisScene.IsValid() && !isPaused && Input.GetKeyDown(KeyCode.Escape))
        {
            background.SetActive(true);
            isPaused = true;

            Time.timeScale = 0f;
            AudioManager.instance.PauseSounds();

            Cursor.lockState = CursorLockMode.None;
        }
        else if (thisScene.IsValid() && isPaused && Input.GetKeyDown(KeyCode.Escape))
        {
            if(background.activeSelf)
            {
                background.SetActive(false);
            }
            else
            {
                SettingsMenu.Instance.BackButton();
                background.SetActive(false);
            }

            isPaused = false;

            Time.timeScale = 1f;
            AudioManager.instance.UnPauseSounds();

            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void ResumeButton()
    {
        if (background.activeSelf)
        {
            background.SetActive(false);
        }
        else
        {
            SettingsMenu.Instance.BackButton();
            background.SetActive(false);
        }

        isPaused = false;

        Time.timeScale = 1f;
        AudioManager.instance.UnPauseSounds();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("UIScene");

        Time.timeScale = 1;

        SceneManager.LoadScene("AdrianTest", LoadSceneMode.Additive);        
    }
}
