using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] private GameObject awesomePopup;
    [SerializeField] private GameObject wowPopup;
    [SerializeField] private GameObject coolPopup;
    [SerializeField] private Sprite[] awesomeBackground;
    [SerializeField] private Sprite[] wowBackground;
    [SerializeField] private Sprite[] coolBackground;

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
        Scene thisScene = SceneManager.GetSceneByName(PersistentManager.Instance.GetStringPref("PlayScene").Value);

        if (thisScene.IsValid())
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }

        awesomePopup.SetActive(false);
        wowPopup.SetActive(false);
        coolPopup.SetActive(false);        

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

    public void UIPopUp()
    {

    }

    private void ComboLineRainbow()
    {
        ComboLineColors foundComboLine = Array.Find(comboLineColors, color => color.name == "White");

        comboLine.sprite = foundComboLine.sprite;

        comboLine.material = rainbowMaterial;
        

        StartCoroutine(Cycle());
    }
}
