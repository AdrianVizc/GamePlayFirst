using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Required for scene switching

public class Typer : MonoBehaviour
{
    Text _text;
    TMP_Text _tmpProText;
    string writer;

    [SerializeField] float delayBeforeStart = 0f;
    [SerializeField] float timeBtwChars = 0.1f;
    [SerializeField] float delayBetweenTextAndBackground = 1f; // Delay between switching text and background
    [SerializeField] string leadingChar = "";
    [SerializeField] bool leadingCharBeforeDelay = false;

    [SerializeField] List<string> textList;             // List of texts to type out
    [SerializeField] List<Sprite> backgroundList;       // List of backgrounds to switch between
    [SerializeField] Image backgroundImage;
    [SerializeField] float escapeHoldDuration = 2f;
    private int currentTextIndex = 0;                   // Index of the current text in the list
    private bool isTyping = false;
    private bool skipTyping = false;
    private bool skipAll = false;
    private float escapeHoldTimer = 0f;
    private Scene thisScene;
    [SerializeField] private GameObject mainMenuButton;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            if (isTyping)
            {
                skipTyping = true; // Request full text instantly
            }
            else
            {
                StartNextText(); // Go to next text
            }
        }

        if (skipAll)
            return; // Already skipping everything, ignore input

        if (Input.GetKey(KeyCode.Escape))
        {
            escapeHoldTimer += Time.deltaTime;

            if (escapeHoldTimer >= escapeHoldDuration)
            {
                skipAll = true;
                SkipAllTexts();
            }
        }
        else
        {
            // Reset timer if Escape released early
            escapeHoldTimer = 0f;
        }
    }

    // Initialization
    void Start()
    {        
        thisScene = SceneManager.GetSceneByName(PersistentManager.Instance.GetStringPref("EndCutscene").Value);
        if(thisScene.IsValid())
        {
            mainMenuButton.SetActive(false);
        }

        _text = GetComponent<Text>();
        _tmpProText = GetComponent<TMP_Text>();

        StartNextText(); // Start the typing effect for the first text
    }

    void StartNextText()
    {
        if (currentTextIndex >= textList.Count)
        {
            // If all texts are done, switch to the next scene
            if (thisScene.IsValid())
            {
                mainMenuButton.SetActive(true);
            }
            else
            {
                SceneManager.LoadScene(PersistentManager.Instance.GetStringPref("PlayScene").Value);

                SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);
            }
                
            return; // Exit method
        }

        writer = textList[currentTextIndex]; // Get the current text from the list

        if (_text != null)
        {
            _text.text = ""; // Clear the UI Text field
            StartCoroutine(TypeWriterText());
        }

        if (_tmpProText != null)
        {
            _tmpProText.text = ""; // Clear the TMP field
            StartCoroutine(TypeWriterTMP());
        }
    }

    IEnumerator TypeWriterText()
    {
        isTyping = true;
        skipTyping = false;

        _text.text = leadingCharBeforeDelay ? leadingChar : "";
        yield return new WaitForSeconds(delayBeforeStart);

        foreach (char c in writer)
        {
            if (skipTyping)
            {
                _text.text = writer;
                break;
            }

            // Remove the trailing leadingChar, add the next char, then re-add it
            if (_text.text.Length > 0 && leadingChar.Length > 0)
            {
                _text.text = _text.text.Substring(0, _text.text.Length - leadingChar.Length);
            }

            _text.text += c + leadingChar;
            yield return new WaitForSeconds(timeBtwChars);
        }

        // Remove the trailing leadingChar only if it's there
        if (!string.IsNullOrEmpty(leadingChar) && _text.text.EndsWith(leadingChar) && _text.text.Length > writer.Length)
        {
            _text.text = _text.text.Substring(0, _text.text.Length - leadingChar.Length);
        }

        yield return new WaitForSeconds(delayBetweenTextAndBackground);
        isTyping = false;

        if (currentTextIndex == textList.Count - 1)
        {
            if (thisScene.IsValid())
            {
                mainMenuButton.SetActive(true);
            }
            else
            {
                SceneManager.LoadScene(PersistentManager.Instance.GetStringPref("PlayScene").Value);

                SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);
            }
        }
        else
        {
            currentTextIndex++;
            StartNextBackgroundAndText();
        }
    }

    IEnumerator TypeWriterTMP()
    {
        isTyping = true;
        skipTyping = false;

        _tmpProText.text = leadingCharBeforeDelay ? leadingChar : "";
        yield return new WaitForSeconds(delayBeforeStart);

        foreach (char c in writer)
        {
            if (skipTyping)
            {
                _tmpProText.text = writer;
                break;
            }

            // Remove the trailing leadingChar, add the next char, then re-add it
            if (_tmpProText.text.Length > 0 && leadingChar.Length > 0)
            {
                _tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
            }

            _tmpProText.text += c + leadingChar;
            yield return new WaitForSeconds(timeBtwChars);
        }

        // Remove the trailing leadingChar only if it's there
        if (!string.IsNullOrEmpty(leadingChar) && _tmpProText.text.EndsWith(leadingChar) && _tmpProText.text.Length > writer.Length)
        {
            _tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
        }

        yield return new WaitForSeconds(delayBetweenTextAndBackground);
        
        isTyping = false;

        if (currentTextIndex == textList.Count - 1)
        {
            if (thisScene.IsValid())
            {
                mainMenuButton.SetActive(true);
            }
            else
            {
                SceneManager.LoadScene(PersistentManager.Instance.GetStringPref("PlayScene").Value);

                SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);
            }
        }
        else
        {
            currentTextIndex++;
            StartNextBackgroundAndText();
        }
    }

    void StartNextBackgroundAndText()
    {
        // Change the background image if there are background images provided
        if (backgroundList.Count > currentTextIndex && backgroundImage != null)
        {
            backgroundImage.sprite = backgroundList[currentTextIndex]; // Switch background
        }

        // Start typing the next text
        StartNextText();
    }

    void SkipAllTexts()
    {
        // Stop any ongoing typing coroutines:
        StopAllCoroutines();

        // Show the last text fully (from textList)
        string lastText = textList[textList.Count - 1];

        if (_text != null)
        {
            _text.text = lastText;
        }

        if (_tmpProText != null)
        {
            _tmpProText.text = lastText;
        }

        // Set currentTextIndex to the last text index to prevent further typing
        currentTextIndex = textList.Count;

        // Optionally change to the last background
        if (backgroundList.Count > 0 && backgroundImage != null)
        {
            backgroundImage.sprite = backgroundList[backgroundList.Count - 1];
        }

        isTyping = false;
        skipTyping = false;

        // Immediately load the final scene if you want
        if (thisScene.IsValid())
        {
            mainMenuButton.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene(PersistentManager.Instance.GetStringPref("PlayScene").Value);

            SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);
        }
    }

    public void MainMenuButton()
    {
        PauseMenu.Instance.MainMenu();
    }
}
