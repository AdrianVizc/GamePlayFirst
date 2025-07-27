using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMenu : Menu
{
    [Space]

    [SerializeField] AudioMixer audioMixer;

    [Space]

    [SerializeField] private Slider masterSlider;
    [SerializeField] private TMP_Text masterSliderValueText;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TMP_Text musicSliderValueText;
    [SerializeField] private Slider UISlider;
    [SerializeField] private TMP_Text UISliderValueText;
    [SerializeField] private Slider environmentSlider;
    [SerializeField] private TMP_Text environmentSliderValueText;

    private void OnEnable()
    {
        SettingsMenu.Instance.ChangeBackground(1);
    }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        //Sets the sliders to previous setting
        masterSlider.value = ES3.Load(AudioManager.MIXER_MASTER, PersistentManager.Instance.GetFloatPref(AudioManager.MIXER_MASTER).GetDefaultValue());
        musicSlider.value = ES3.Load(AudioManager.MIXER_MUSIC, PersistentManager.Instance.GetFloatPref(AudioManager.MIXER_MUSIC).GetDefaultValue());
        UISlider.value = ES3.Load(AudioManager.MIXER_UI, PersistentManager.Instance.GetFloatPref(AudioManager.MIXER_UI).GetDefaultValue());
        environmentSlider.value = ES3.Load(AudioManager.MIXER_ENVIRONMENT, PersistentManager.Instance.GetFloatPref(AudioManager.MIXER_ENVIRONMENT).GetDefaultValue());

        SetMasterValueText();
        SetMusicValueText();
        SetUIValueText();
        SetEnvironmentValueText();
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

    #endregion   

    #region UISlider

    private void SetUIValueText() //Constantly update the text to = the slider value
    {
        UISliderValueText.text = (UISlider.value * 100).ToString("F0");
    }

    public void SetUIVolume(float val)
    {
        SetVolume(AudioManager.MIXER_UI, val);

        SetUIValueText();
    }

    #endregion

    #region EnvironmentSlider

    private void SetEnvironmentValueText() //Constantly update the text to = the slider value
    {
        environmentSliderValueText.text = (environmentSlider.value * 100).ToString("F0");
    }

    public void SetEnvironmentVolume(float val)
    {
        SetVolume(AudioManager.MIXER_ENVIRONMENT, val);

        SetEnvironmentValueText();
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

        var temp = PersistentManager.Instance.GetFloatPref(name);

        temp.SetValue(roundedValue); //Set value in PersistentManager
        temp.Save(); //Save value in PersistentManager
    }
}
