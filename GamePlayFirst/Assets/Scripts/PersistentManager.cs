using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class PersistentManager : MonoBehaviour
{
    public static PersistentManager Instance; //Code to make script into a singleton

    [SerializeField] private bool skipStartupAnimation;
    [SerializeField] private VideoPlayer videoPlayer;

    [Space]
    
    [SerializeField] public FixedResolution[] resolutions;
    [Serializable]
    public struct FixedResolution
    {
        public int width;
        public int height;
        public string resolutionName;
    }

    [SerializeField] Prefs<int>[] intPrefs;
    [SerializeField] Prefs<float>[] floatPrefs;
    [SerializeField] Prefs<bool>[] boolPrefs;
    [SerializeField] Prefs<string>[] stringPrefs;
    Dictionary<string, int> nameToIntPref = new();
    Dictionary<string, int> nameToFloatPref = new();
    Dictionary<string, int> nameToBoolPref = new();
    Dictionary<string, int> nameToStringPref = new();

    public FullScreenMode screenMode { get; private set; }
    public int resolutionNumber { get; private set; }

    private const int RESOLUTION_ELEMENT = 3;

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

        DontDestroyOnLoad(gameObject);        
    }

    void Start()
    {
        if (!skipStartupAnimation)
        {
            StartCoroutine(PlayBeginningAnimationThenStartup());
        }
        else
        {
            Startup();
        }
    }

    IEnumerator PlayBeginningAnimationThenStartup()
    {
        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        PlayBeginningAnimation();

        while (videoPlayer.isPlaying)
        {
            yield return null;
        }
        videoPlayer.gameObject.SetActive(false);

        Startup();
    }

    #region Startup
    private void Startup()
    {
        SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);

        SetupPrefs();

        SetupVsyncSetting(ES3.Load("VsyncToggle", GetIntPref("VsyncToggle").GetDefaultValue())); //This chunk loads previous settings
        SetupAntiAliasingSetting(ES3.Load("AntiAliasing", GetIntPref("AntiAliasing").GetDefaultValue()));
        resolutionNumber = ES3.Load("resolutionNumber", GetNativeResolution());
        screenMode = ES3.Load("ScreenMode", FullScreenMode.FullScreenWindow);
        FinalizeViewSwitch();
    }

    private int GetNativeResolution() //Checks if user's native resolution = any of the dropdown settings. If not, defaults to RESOLUTION_ELEMENT
    {
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (Screen.currentResolution.width == resolutions[i].width && Screen.currentResolution.height == resolutions[i].height)
            {
                return i;
            }
        }
        return RESOLUTION_ELEMENT;
    }

    private void PlayBeginningAnimation()
    {
        videoPlayer.Play();
    }

    void SetupPrefs()
    {
        for (int i = 0; i < intPrefs.Length; i++)
        {
            intPrefs[i].Load();
            nameToIntPref.Add(intPrefs[i].GetName(), i);
        }
        for (int i = 0; i < floatPrefs.Length; i++)
        {
            floatPrefs[i].Load();
            nameToFloatPref.Add(floatPrefs[i].GetName(), i);
        }
        for (int i = 0; i < boolPrefs.Length; i++)
        {
            boolPrefs[i].Load();
            nameToBoolPref.Add(boolPrefs[i].GetName(), i);
        }
        for (int i = 0; i < stringPrefs.Length; i++)
        {
            stringPrefs[i].Load();
            nameToStringPref.Add(stringPrefs[i].GetName(), i);
        }
    }

    public Prefs<int> GetIntPref(string name)
    {
        return intPrefs[nameToIntPref[name]];
    }

    public Prefs<float> GetFloatPref(string name)
    {
        return floatPrefs[nameToFloatPref[name]];
    }

    public Prefs<bool> GetBoolPref(string name)
    {
        return boolPrefs[nameToBoolPref[name]];
    }

    public Prefs<string> GetStringPref(string name)
    {
        return stringPrefs[nameToStringPref[name]];
    }
    #endregion

    public void SetupVsyncSetting(int val)
    {
        QualitySettings.vSyncCount = val; //0 is off , 1 is on (On means the game's frames = monitor's refresh rate)

        Debug.Log("Vsync: " + QualitySettings.vSyncCount); 
    }

    public void SetupAntiAliasingSetting(int val)
    {
        QualitySettings.antiAliasing = val; //0 is off. 2x, 4x, and 8x, are on with increasing quality

        Debug.Log("Anti-Aliasing: " + QualitySettings.antiAliasing);
    }

    public void SwitchScreenMode(int val)
    {
        switch (val)
        {
            case 0:
                screenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                screenMode = FullScreenMode.FullScreenWindow;
                break;
            case 3:
                screenMode = FullScreenMode.Windowed;
                break;
        }

        ES3.Save("ScreenMode", screenMode);

        FinalizeViewSwitch();
    }

    public void SwitchResolutionNumber(int val)
    {
        resolutionNumber = val;

        ES3.Save("resolutionNumber", val);

        FinalizeViewSwitch();
    }

    void FinalizeViewSwitch()
    {
        Screen.fullScreenMode = screenMode;
        Debug.Log("Display Mode: " + screenMode);

        Screen.SetResolution(resolutions[resolutionNumber].width, resolutions[resolutionNumber].height, screenMode);
        Debug.Log("Resolution: " + resolutions[resolutionNumber].width + " x " + resolutions[resolutionNumber].height);
    }
}
