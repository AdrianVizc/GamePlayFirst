using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static PersistentManager;

public class GraphicsMenu : Menu
{
    public static GraphicsMenu Instance;

    [Space]

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown windowDropdown;
    [SerializeField] private TMP_Dropdown antiAliasingDropdown;
    [SerializeField] private TMP_Text vsyncText;
    [SerializeField] private Slider vsyncSlider;
    //[SerializeField] private TMP_Text hudText;
    //[SerializeField] private Slider hudSlider;
    //[SerializeField] private Slider showControlsSlider;
    //[SerializeField] private TMP_Text showControlsText;
    //[SerializeField] GameObject controlsGameObj;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);

            return;
        }
    }

    private void OnEnable()
    {
        PopulateResolutionDropDown();

        LoadResolution();
        LoadScreenMode();
        LoadAntiAliasing();
        LoadVsync();
        //LoadControls();
    }

    private void Update()
    {
        SetVsyncText();
        //SetControlsText();
    }

    #region Resolution

    private void LoadResolution() //Updates the value when the player opens the game
    {
        resolutionDropdown.value = PersistentManager.Instance.resolutionNumber;
    }

    private void PopulateResolutionDropDown() //Fills the dropdown with the Resolutions serialized field in PersistentManager. Add more options in that serialized field to add more dropdown options
    {
        var resolutionNames = new List<string>();

        foreach (FixedResolution resolution in PersistentManager.Instance.resolutions)
        {
            resolutionNames.Add(resolution.resolutionName);
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionNames);
    }

    public void SwitchResolution() //Functiion when playing moves the slider
    {
        PersistentManager.Instance.SwitchResolutionNumber(resolutionDropdown.value);
    }

    #endregion

    #region ViewMode

    private void LoadScreenMode() //Updates the value when the player opens the game
    {
        FullScreenMode mode = PersistentManager.Instance.screenMode;

        if (mode == FullScreenMode.ExclusiveFullScreen)
        {
            windowDropdown.value = 0;
        }
        else if (mode == FullScreenMode.FullScreenWindow)
        {
            windowDropdown.value = 1;
        }
        else
        {
            windowDropdown.value = 3;
        }
    }

    public void SwitchScreenMode(int val) //Function to connect to Dropdown
    {
        PersistentManager.Instance.SwitchScreenMode(val);
    }

    #endregion

    #region AntiAliasing

    private void LoadAntiAliasing() //Updates the value when the player opens the game
    {
        antiAliasingDropdown.value = AAToSliderValue(ES3.Load("AntiAliasing", PersistentManager.Instance.GetIntPref("AntiAliasing").GetDefaultValue()));
    }

    private int AAToSliderValue(int val) //PersistentManager AntiAliasing value is will only be 0, 2, 4, 8. Need to convert this for slider values 0, 1, 2, 3
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
        PersistentManager.Instance.SetupAntiAliasingSetting(SliderToAAValue(val));

        var temp = PersistentManager.Instance.GetIntPref("AntiAliasing"); //Saves value to PersistentManager

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
        vsyncSlider.value = ES3.Load("VsyncToggle", PersistentManager.Instance.GetIntPref("VsyncToggle").GetDefaultValue());
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
        PersistentManager.Instance.SetupVsyncSetting((int)val);

        var temp = PersistentManager.Instance.GetIntPref("VsyncToggle"); //Saves value to PersistentManager

        temp.SetValue((int)val);
        temp.Save();
    }

    #endregion

    /*#region ShowControls

    private void LoadControls() //Updates the slider value when the player opens the game
    {
        showControlsSlider.value = ES3.Load("ShowControlsToggle", PersistentManager.Instance.GetIntPref("ShowControlsToggle").GetDefaultValue());
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
        var temp = PersistentManager.Instance.GetIntPref("ShowControlsToggle"); //Saves value to PersistentManager

        temp.SetValue((int)val);
        temp.Save();
    }

    #endregion*/
    
}