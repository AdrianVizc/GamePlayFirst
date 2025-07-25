using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public const string MIXER_MASTER = "MasterVol";
    public const string MIXER_MUSIC = "MusicVol";
    public const string MIXER_UI = "UIVol";
    public const string MIXER_ENVIRONMENT = "EnvironmentVol";

    [SerializeField] AudioMixer audioMixer;

    [Space]

    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource UISource;
    [SerializeField] AudioSource environmentSource;

    [Space]

    [SerializeField] Sound[] musicClips;
    [SerializeField] Sound[] UIClips;
    [SerializeField] Sound[] EnvironmentClips;

    [Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
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

    public void LoadVolume() //Used to set audio mixer as the game opens. Otherwise, the AudioMenu script will only change the mixer when you open that menu
    {
        string[] MIXER_PARAMETERS = { MIXER_MASTER, MIXER_MUSIC, MIXER_UI, MIXER_ENVIRONMENT };
        foreach (string volumeParameter in MIXER_PARAMETERS)
        {
            float volume = ES3.Load(volumeParameter, PersistentManager.Instance.GetFloatPref(volumeParameter).GetDefaultValue());
            float roundedValue = Mathf.Round(volume * 100f) / 100f; //Value at 2 decimal
            float dB;

            if (roundedValue == 0) //Prevents Log(0) which is undefined
            {
                dB = -80f;
            }
            else
            {
                dB = Mathf.Log10(roundedValue) * 20f;
            }

            audioMixer.SetFloat(volumeParameter, dB);
        }
    }

    //Play sound on musicSource
    public void PlayMusicSound(string name, bool isBGM)
    {
        Sound sound = Array.Find(musicClips, s => s.name == name);

        if (sound != null) 
        {
            if (isBGM)
            {
                musicSource.clip = sound.clip;
                musicSource.loop = true;
                musicSource.Play();
            }
            else
            {
                musicSource.PlayOneShot(sound.clip);
            }
        }
    }

    //Play sound on UISource
    public void PlayUISound(string name)
    {
        Sound sound = Array.Find(musicClips, s => s.name == name);

        if (sound != null)
        {
            UISource.PlayOneShot(sound.clip);
        }
    }

    //Play sound on environmentSource
    public void PlayEnvironmentSound(string name)
    {
        Sound sound = Array.Find(musicClips, s => s.name == name);

        if (sound != null)
        {
            environmentSource.PlayOneShot(sound.clip);
        }
    }
}