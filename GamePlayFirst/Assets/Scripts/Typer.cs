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
    [SerializeField] Image backgroundImage;             // The Image component for the background
    [SerializeField] string nextSceneName;              // Name of the next scene to load after the last text
    private int currentTextIndex = 0;                   // Index of the current text in the list
    private bool isTyping = false;
    private bool skipTyping = false;

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
    }

    // Initialization
    void Start()
    {
        _text = GetComponent<Text>();
        _tmpProText = GetComponent<TMP_Text>();

        StartNextText(); // Start the typing effect for the first text
    }

    void StartNextText()
    {
        if (currentTextIndex >= textList.Count)
        {
            // If all texts are done, switch to the next scene
            SceneManager.LoadScene(nextSceneName);
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
                _text.text = writer; // Show full text
                break;
            }

            if (_text.text.Length > 0)
            {
                _text.text = _text.text.Substring(0, _text.text.Length - leadingChar.Length);
            }

            _text.text += c;
            _text.text += leadingChar;
            yield return new WaitForSeconds(timeBtwChars);
        }

        if (leadingChar != "")
        {
            _text.text = _text.text.Substring(0, _text.text.Length - leadingChar.Length);
        }

        yield return new WaitForSeconds(delayBetweenTextAndBackground); // Always delay

        isTyping = false;

        if (currentTextIndex == textList.Count - 1)
        {
            SceneManager.LoadScene(nextSceneName);
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

            if (_tmpProText.text.Length > 0)
            {
                _tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
            }

            _tmpProText.text += c;
            _tmpProText.text += leadingChar;
            yield return new WaitForSeconds(timeBtwChars);
        }

        if (leadingChar != "")
        {
            _tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
        }

        yield return new WaitForSeconds(delayBetweenTextAndBackground);

        isTyping = false;

        if (currentTextIndex == textList.Count - 1)
        {
            SceneManager.LoadScene(nextSceneName);
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
}
