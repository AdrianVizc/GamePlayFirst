using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Prefs<T>
{
    //This script is for the SavedValues GameObject to display the values in the editor as well as save and load them

    [SerializeField] private string prefName;
    public T Value { get; private set; }  //Current value meant to be changed
    [SerializeField] private T DefaultValue;

    public virtual void Load()
    {
        Value = ES3.Load(prefName, DefaultValue);
    }

    public virtual void Save()
    {
        ES3.Save(prefName, Value);
    }

    public virtual T GetDefaultValue()
    {
        return DefaultValue;
    }

    public void SetValue(T val)
    {
        Value = val;
    }

    public string GetName()
    {
        return prefName;
    }
}
