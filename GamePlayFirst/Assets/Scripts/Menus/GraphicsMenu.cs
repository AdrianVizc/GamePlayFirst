using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsMenu : Menu
{
    /*public static SettingsGraphics instance;

    [Space]

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown windowDropdown;
    [SerializeField] private TMP_Dropdown antiAliasingDropdown;
    [SerializeField] private TMP_Text vsyncText;
    [SerializeField] private Slider vsyncSlider;
    [SerializeField] private TMP_Text hudText;
    [SerializeField] private Slider hudSlider;
    [SerializeField] private Slider bloomSlider;
    [SerializeField] TMP_InputField bloomSliderValueText;
    [SerializeField] private Slider showControlsSlider;
    [SerializeField] private TMP_Text showControlsText;
    [SerializeField] GameObject controlsGameObj;

    //[Space]

    //[SerializeField] private List<GameObject> hudUIGameObjects;

    public override void ReturnToPrevious()
    {
        UIManager.instance.SettingsMenu.ClearArrows();
        base.ReturnToPrevious();
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

            return;
        }
    }

    private void OnEnable()
    {
        PopulateResolutionDropDown();

        LoadResolution();
        LoadView();
        LoadAntiAliasing();
        LoadVsync();
        LoadHUD();
        LoadBloom();
        LoadControls();
    }

    private void Update()
    {
        SetHudText();
        SetVsyncText();
        SetControlsText();
    }

    #region Resolution

    private void LoadResolution() //Updates the value when the player opens the game
    {
        resolutionDropdown.value = PersistentManager.instance.currentResolutionNumber;
    }

    private void PopulateResolutionDropDown() //Fills the dropdown with the Resolutions serialized field in PersistentManager. Add more options in that serialized field to add more dropdown options
    {
        var resolutionNames = new List<string>();

        foreach (FixedResolution resolution in PersistentManager.instance.Resolutions)
        {
            resolutionNames.Add(resolution.resolutionName);
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionNames);
    }

    public void SwitchResolution() //Functiion when playing moves the slider
    {
        PersistentManager.instance.SwitchResolutionNumber(resolutionDropdown.value);

        //In PersistentManager, SwitchResolutionNumber(int val) switches currentViewMode to Windowed if the value of the entered setting is not the native screen resolution
        //This section keeps the UI updated with it
        if (PersistentManager.instance.currentViewMode == FullScreenMode.Windowed)
        {
            windowDropdown.value = 1;
        }
    }

    #endregion

    #region ViewMode

    private void LoadView() //Updates the value when the player opens the game
    {
        FullScreenMode mode = PersistentManager.instance.currentViewMode;



        if (mode == FullScreenMode.Windowed)
        {
            windowDropdown.value = 1; //1 = windowed
        }
        else
        {
            windowDropdown.value = 0; //0 = fullscreen
        }
    }

    public void SwitchView(int val) //Currently this is only because the only options are fullscreen or windowed
    {
        PersistentManager.instance.SwitchViewMode(val);
    }

    #endregion

    #region AntiAliasing

    private void LoadAntiAliasing() //Updates the value when the player opens the game
    {
        antiAliasingDropdown.value = AAToSliderValue(ES3.Load("antiAliasing", PersistentManager.instance.GetIntPref("antiAliasing").GetDefaultValue()));
    }

    private int AAToSliderValue(int val) //PersistentManager antiAliasing value is will only be 0, 2, 4, 8. Need to convert this for slider values 0, 1, 2, 3
    {
        if (val == 2)
        {
            val = 1;
        }
        else if (val == 4)
        {
            val = 2;
        }
        else if (val == 8)
        {
            val = 3;
        }

        return val; //Returns 0 if 0
    }

    public void SwitchAA(int val) //Method for dropdown
    {
        PersistentManager.instance.SetupAntiAliasingSetting(SliderToAAValue(val));

        var temp = PersistentManager.instance.GetIntPref("antiAliasing"); //Saves value to PersistentManager

        temp.SetValue(SliderToAAValue(val));
        temp.Save();
    }

    private int SliderToAAValue(int val) //Slider will give values 0, 1, 2, 3. Need to turn this into 0, 2, 4, 8 for Anti-Aliasing
    {
        switch (val)
        {
            case 1:
                return 2;
            case 2:
                return 4;
            case 3:
                return 8;
            default:
                return val; //Returns 0 if 0
        }
    }

    #endregion

    #region Vsync

    private void LoadVsync() //Updates the slider value when the player opens the game
    {
        vsyncSlider.value = ES3.Load("VsyncToggle", PersistentManager.instance.GetIntPref("VsyncToggle").GetDefaultValue());
    }

    private void SetVsyncText() //Constantly update the text to = the slider value
    {
        if (vsyncSlider.value == 0)
        {
            vsyncText.text = "Off";
        }
        else
        {
            vsyncText.text = "On";
        }
    }

    public void ToggleVsync(float val) //For Slider interaction when the player clicks the game object
    {
        PersistentManager.instance.SetupVsyncSetting((int)val);

        var temp = PersistentManager.instance.GetIntPref("VsyncToggle"); //Saves value to PersistentManager

        temp.SetValue((int)val);
        temp.Save();
    }

    #endregion

    #region HudUI

    private void LoadHUD() //Updates the slider value when the player opens the game
    {
        hudSlider.value = ES3.Load("HUDToggle", PersistentManager.instance.GetIntPref("HUDToggle").GetDefaultValue());

        controlsGameObj.SetActive(hudSlider.value == 1);
    }

    private void SetHudText() //Constantly update the text to = the slider value
    {
        if (hudSlider.value == 0)
        {
            hudText.text = "Off";
        }
        else
        {
            hudText.text = "On";
        }
    }

    //This might change because it might need to be in the PersistentManager because this script is initially inactive
    public void ToggleHUD(float val) //Function when player turns HUD on and off
    {
        var temp = PersistentManager.instance.GetIntPref("HUDToggle"); //Saves value to PersistentManager

        temp.SetValue((int)val);
        temp.Save();

        controlsGameObj.SetActive(hudSlider.value == 1);
    }

    #endregion


    #region Bloom

    private void LoadBloom()
    {
        bloomSlider.value = PersistentManager.instance.GetFloatPref("bloom").Value;
        SetBloomText();
    }

    public void SaveBloomSlderValue(float val)
    {
        float roundedValue = Mathf.Round(val * 100f) / 100f; //Value at 2 decimal

        var temp = PersistentManager.instance.GetFloatPref("bloom");

        temp.SetValue(roundedValue);
        temp.Save();

        PostProcessingManager.instance.SetBloomVal(roundedValue);

        SetBloomText();
    }

    private void SetBloomText()
    {
        bloomSliderValueText.text = PersistentManager.instance.GetFloatPref("bloom").Value.ToString();
    }

    public void ManualEditBloomValue(string val)
    {
        float value = float.Parse(val);

        value = Mathf.Clamp(value, 0f, 100f);
        var temp = PersistentManager.instance.GetFloatPref("bloom");
        temp.SetValue(value);
        temp.Save();

        PostProcessingManager.instance.SetBloomVal(value);

        LoadBloom();
        SetBloomText();
    }


    #endregion

    #region ShowControls

    private void LoadControls() //Updates the slider value when the player opens the game
    {
        showControlsSlider.value = ES3.Load("ShowControlsToggle", PersistentManager.instance.GetIntPref("ShowControlsToggle").GetDefaultValue());
    }

    private void SetControlsText() //Constantly update the text to = the slider value
    {
        if (showControlsSlider.value == 0)
        {
            showControlsText.text = "Off";
        }
        else
        {
            showControlsText.text = "On";
        }
    }

    public void ToggleControls(float val) //For Slider interaction when the player clicks the game object
    {
        var temp = PersistentManager.instance.GetIntPref("ShowControlsToggle"); //Saves value to PersistentManager

        temp.SetValue((int)val);
        temp.Save();
    }

    #endregion
    */
}