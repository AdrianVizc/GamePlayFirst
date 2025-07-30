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
    [SerializeField] AudioSource railGrindSource;
    [SerializeField] AudioSource skateSource;
    [SerializeField] AudioSource wallGrindSource;

    [Space]

    [SerializeField] Sound[] musicClips;
    [SerializeField] Sound[] UIClips;
    [SerializeField] Sound[] EnvironmentClips;

    [HideInInspector]
    public bool railGrindPlayOnce;
    public bool skateGrindPlayOnce;
    public bool wallGrindPlayOnce;

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

        railGrindPlayOnce = false;
        skateGrindPlayOnce = false;
        wallGrindPlayOnce = false;
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
                musicSource.Stop();
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
        Sound sound = Array.Find(UIClips, s => s.name == name);

        if (sound != null)
        {
            UISource.PlayOneShot(sound.clip);
        }
    }

    //Play sound
    public void PlayEnvironmentSound(string name)
    {
        Sound sound = Array.Find(EnvironmentClips, s => s.name == name);

        switch (name)
        {
            case "RailGrind":

                railGrindSource.clip = sound.clip;
                railGrindSource.loop = true;
                railGrindSource.Play();

                break;

            case "Skate":

                skateSource.clip = sound.clip;
                skateSource.loop = true;
                skateSource.Play();

                break;
            case "WallGrind":

                wallGrindSource.clip = sound.clip;
                wallGrindSource.loop = true;
                wallGrindSource.Play();

                break;

            default:

                environmentSource.PlayOneShot(sound.clip);

                break;
        }
    }

    public void StopEnvironmentSound(string name)
    {
        Sound sound = Array.Find(EnvironmentClips, s => s.name == name);

        switch (name)
        {
            case "RailGrind":

                railGrindSource.Stop();

                break;

            case "Skate":

                skateSource.Stop();

                break;
            case "WallGrind":

                wallGrindSource.Stop();

                break;
        }
    }

    public void PauseSkateSound()
    {
        skateSource.Pause();
    }
    public void UnPauseSkateSound()
    {
        skateSource.UnPause();
    }

    public void PauseSounds()
    {
        environmentSource.Pause();
        railGrindSource.Pause();
        skateSource.Pause();
        wallGrindSource.Pause();
    }

    public void UnPauseSounds()
    {
        environmentSource.UnPause();
        railGrindSource.UnPause();
        skateSource.UnPause();
        wallGrindSource.UnPause();
    }
}