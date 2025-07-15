using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentManager : MonoBehaviour
{
    public static PersistentManager Instance; //Code to make script into a singleton

    [SerializeField] private bool skipStartupAnimation;

    [Space]

    [SerializeField] Prefs<int>[] intPrefs;
    [SerializeField] Prefs<float>[] floatPrefs;
    [SerializeField] Prefs<bool>[] boolPrefs;
    [SerializeField] Prefs<string>[] stringPrefs;
    Dictionary<string, int> nameToIntPref = new();
    Dictionary<string, int> nameToFloatPref = new();
    Dictionary<string, int> nameToBoolPref = new();
    Dictionary<string, int> nameToStringPref = new();

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
        if (skipStartupAnimation)
        {
            PlayBeginningAnimation();
            return;
        }

        Startup();
    }

    #region Startup
    private void Startup()
    {
        SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);

        SetupPrefs();
    }

    private void PlayBeginningAnimation()
    {

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
}
