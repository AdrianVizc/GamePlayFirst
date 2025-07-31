using System;
using System.Collections;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InGameCanvas : MonoBehaviour
{
    public static InGameCanvas instance;

    [SerializeField] private TMP_Text trickPoints;
    [SerializeField] private TMP_Text multiplier;
    [SerializeField] private TMP_Text combo;
    [SerializeField] private TMP_Text trickName;
    [SerializeField] private Image comboLine;
    [SerializeField] private ComboLineColors[] comboLineColors;
    [SerializeField] private float rainbowSpeed; //Smaller means color changes faster
    [SerializeField] private Material rainbowMaterial;
    [SerializeField] private Transform[] popupPositions;
    [SerializeField] private GameObject[] popupPrefab;
    [SerializeField] private int popupPointThreshold; //Number of Combos needed for popup to appear. Must be > 1
    [SerializeField] private float waitBeforeTrickPointFadeOut = 1; //Amount of time before it starts to disappear
    [SerializeField] private float trickPointFadeOut = 2; //Disappears over "trickPointFadeOut" seconds 
    [SerializeField] private TMP_Text trickPointsPopup;

    private Transform previousPos;
    private bool doOnce;
    private Coroutine fadeOutCoroutine;

    private Color32[] colors;

    [Serializable]
    public class ComboLineColors
    {
        public string name;
        public Sprite sprite;
        public int pointThreshold;
    }

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
        doOnce = false;

        Scene thisScene = SceneManager.GetSceneByName(PersistentManager.Instance.GetStringPref("PlayScene").Value);

        if (thisScene.IsValid())
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }      

        colors = new Color32[7]
        {
            new Color32(255, 0, 0, 255), //red
            new Color32(255, 165, 0, 255), //orange
            new Color32(255, 255, 0, 255), //yellow
            new Color32(0, 255, 0, 255), //green
            new Color32(0, 0, 255, 255), //blue
            new Color32(75, 0, 130, 255), //indigo
            new Color32(238, 130, 238, 255), //violet
       };        
    }

    private IEnumerator Cycle()
    {
        int i = 0;
        while (true)
        {
            for (float interpolant = 0f; interpolant < rainbowSpeed; interpolant += 0.001f)
            {
                rainbowMaterial.color = Color.Lerp(colors[i % colors.Length], colors[(i + 1) % colors.Length], interpolant);
                yield return null;
            }
            i++;
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

    public void UpdateCombo(float num)
    {
        combo.text = num.ToString();
    }

    public void UpdateTrickName(string name)
    {
        trickName.text = name;
    }

    public void UpdateComboLineColor(float num)
    {
        for (int i = 0; i < comboLineColors.Length; i++)
        {
            if (i + 1 < comboLineColors.Length) //Endcase for the last element
            {
                if (comboLineColors[i].pointThreshold <= num && num < comboLineColors[i + 1].pointThreshold)
                {
                    comboLine.material = null;
                    comboLine.sprite = comboLineColors[i].sprite;
                    break;
                }
            }
            else
            {
                ComboLineRainbow();
            }
        }            
    }

    public void UIPopUp(float num)
    {        
        if (num % popupPointThreshold == 0 && num != 0 && !doOnce)
        {
            doOnce = true;

            Transform chosenPos = popupPositions[Random.Range(0, popupPositions.Length)];

            while (previousPos == chosenPos)
            {
                chosenPos = popupPositions[Random.Range(0, popupPositions.Length)];
            }

            previousPos = chosenPos;

            Instantiate(popupPrefab[Random.Range(0, popupPrefab.Length)], chosenPos);
        }

        if (num % popupPointThreshold != 0)
        {
            doOnce = false;
        }
    }

    private void ComboLineRainbow()
    {
        ComboLineColors foundComboLine = Array.Find(comboLineColors, color => color.name == "White");

        comboLine.sprite = foundComboLine.sprite;

        comboLine.material = rainbowMaterial;        

        StartCoroutine(Cycle());
    }

    public void DisplayTrickPoints(float num)
    {
        if (num != 0)
        {
            AudioManager.instance.PlayEnvironmentSound("PointSound");

            if (fadeOutCoroutine != null)
            {
                StopCoroutine(fadeOutCoroutine);
            }

            trickPointsPopup.text = "+" + num;

            Color currentColor = trickPointsPopup.color;
            currentColor.a = 1f;
            trickPointsPopup.color = currentColor;

            fadeOutCoroutine = StartCoroutine(FadeOut());
        }              
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        TMP_Text originalText = trickPointsPopup;

        yield return new WaitForSeconds(waitBeforeTrickPointFadeOut);

        while (elapsedTime < trickPointFadeOut)
        {
            float alpha = Mathf.Lerp(originalText.color.a, 0f, elapsedTime / trickPointFadeOut);
            trickPointsPopup.color = new Color(originalText.color.r, originalText.color.g, originalText.color.b, alpha);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }
}
