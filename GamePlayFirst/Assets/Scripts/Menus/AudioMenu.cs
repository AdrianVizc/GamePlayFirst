using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMenu : Menu
{
    /*[Space]

    [SerializeField] AudioMixer audioMixer;

    [Space]

    [SerializeField] private Slider masterSlider;
    [SerializeField] private TMP_InputField masterSliderValueText;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TMP_InputField musicSliderValueText;
    [SerializeField] private Slider noteSlider;
    [SerializeField] private TMP_InputField noteSliderValueText;
    [SerializeField] private Slider ambienceSlider;
    [SerializeField] private TMP_InputField ambienceSliderValueText;
    [SerializeField] private Slider UISlider;
    [SerializeField] private TMP_InputField UISliderValueText;

    private void Awake()
    {
        Initialize();
    }


    private void Initialize()
    {
        //Sets the sliders to previous setting
        masterSlider.value = ES3.Load(AudioManager.MIXER_MASTER, PersistentManager.instance.GetFloatPref(AudioManager.MIXER_MASTER).GetDefaultValue());
        musicSlider.value = ES3.Load(AudioManager.MIXER_MUSIC, PersistentManager.instance.GetFloatPref(AudioManager.MIXER_MUSIC).GetDefaultValue());
        noteSlider.value = ES3.Load(AudioManager.MIXER_NOTE, PersistentManager.instance.GetFloatPref(AudioManager.MIXER_NOTE).GetDefaultValue());
        ambienceSlider.value = ES3.Load(AudioManager.MIXER_AMBIENCE, PersistentManager.instance.GetFloatPref(AudioManager.MIXER_AMBIENCE).GetDefaultValue());
        UISlider.value = ES3.Load(AudioManager.MIXER_SFX, PersistentManager.instance.GetFloatPref(AudioManager.MIXER_SFX).GetDefaultValue());

        SetMasterValueText();
        SetMusicValueText();
        SetNoteValueText();
        SetAmbienceValueText();
        SetUIValueText();
    }

    #region MasterSlider

    private void SetMasterValueText() //Constantly update the text to = the slider value
    {
        masterSliderValueText.text = (masterSlider.value * 100).ToString("F0");
    }

    public void SetMasterVolume(float val)
    {
        SetVolume(AudioManager.MIXER_MASTER, val);

        SetMasterValueText();
    }

    public void ManualEditMasterVolume(string val)
    {
        float value = float.Parse(val);

        value = Mathf.Clamp(value, 0f, 100f);

        value /= 100f;

        SetVolume(AudioManager.MIXER_MASTER, value);

        masterSlider.value = value;
    }

    #endregion

    #region MusicSlider

    private void SetMusicValueText() //Constantly update the text to = the slider value
    {
        musicSliderValueText.text = (musicSlider.value * 100).ToString("F0");
    }

    public void SetMusicVolume(float val)
    {
        SetVolume(AudioManager.MIXER_MUSIC, val);

        SetMusicValueText();
    }

    public void ManualEditMusicVolume(string val)
    {
        float value = float.Parse(val);

        value = Mathf.Clamp(value, 0f, 100f);

        value /= 100f;

        SetVolume(AudioManager.MIXER_MUSIC, value);

        musicSlider.value = value;
    }

    #endregion

    #region NoteSlider

    private void SetNoteValueText() //Constantly update the text to = the slider value
    {
        noteSliderValueText.text = (noteSlider.value * 100).ToString("F0");
    }

    public void SetNoteVolume(float val)
    {
        SetVolume(AudioManager.MIXER_NOTE, val);

        SetNoteValueText();
    }

    public void ManualEditNoteVolume(string val)
    {
        float value = float.Parse(val);

        value = Mathf.Clamp(value, 0f, 100f);

        value /= 100f;

        SetVolume(AudioManager.MIXER_NOTE, value);

        noteSlider.value = value;
    }


    #endregion

    #region AmbienceSlider

    private void SetAmbienceValueText() //Constantly update the text to = the slider value
    {
        ambienceSliderValueText.text = (ambienceSlider.value * 100).ToString("F0");
    }

    public void SetAmbienceVolume(float val)
    {
        SetVolume(AudioManager.MIXER_AMBIENCE, val);

        SetAmbienceValueText();
    }

    public void ManualEditAmbienceVolume(string val)
    {
        float value = float.Parse(val);

        value = Mathf.Clamp(value, 0f, 100f);

        value /= 100f;

        SetVolume(AudioManager.MIXER_AMBIENCE, value);

        ambienceSlider.value = value;
    }


    #endregion

    #region UISlider

    private void SetUIValueText() //Constantly update the text to = the slider value
    {
        UISliderValueText.text = (UISlider.value * 100).ToString("F0");
    }

    public void SetUIVolume(float val)
    {
        SetVolume(AudioManager.MIXER_SFX, val);

        SetUIValueText();
    }

    public void ManualEditUIValueVolume(string val)
    {
        float value = float.Parse(val);

        value = Mathf.Clamp(value, 0f, 100f);

        value /= 100f;

        SetVolume(AudioManager.MIXER_SFX, value);

        UISlider.value = value;
    }


    #endregion

    private void SetVolume(string name, float sliderValue)
    {
        float roundedValue = Mathf.Round(sliderValue * 100f) / 100f; //Value at 2 decimal
        float dB;

        if (roundedValue == 0) //Prevents Log(0) which is undefined
        {
            dB = -80f;
        }
        else
        {
            dB = Mathf.Log10(roundedValue) * 20f;
        }

        //If the Log function doesn't sound good/accurate, use this instead
        //roundedValue *= 100;
        //roundedValue -= 80;
        //dB = roundedValue;*

        audioMixer.SetFloat(name, dB); //Sets the Audio Mixer values        

        var temp = PersistentManager.instance.GetFloatPref(name);

        temp.SetValue(roundedValue); //Set value in PersistentManager
        temp.Save(); //Save value in PersistentManager
    }*/
}
