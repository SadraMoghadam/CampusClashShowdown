using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To introduce a variable that can be saved on player device, we should name it in this enumerator
/// </summary>
public enum PlayerPrefsKeys
{
    GameStarted,
    Level,
    GameTimer,
}

/// <summary>
/// All of the functions that are needed to delete, add, or update data or variable on player device
/// </summary>
public class PlayerPrefsManager : MonoBehaviour
{
    
    private void Start()
    {
    }

    public static void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
    
    public static void DeleteKey(PlayerPrefsKeys key)
    {
        PlayerPrefs.DeleteKey(key.ToString());
    }
    
    public static void SetBool(PlayerPrefsKeys key, bool value)
    {
        PlayerPrefs.SetInt(key.ToString(), value ? 1 : 0);
    }

    public static bool GetBool(PlayerPrefsKeys key, bool defaultValue = true)
    {
        int value = defaultValue ? 1 : 0;
        if (PlayerPrefs.HasKey(key.ToString()))
        {
            value = PlayerPrefs.GetInt(key.ToString());
        }
        else
        {
            PlayerPrefs.SetInt(key.ToString(), value);
        }

        return value == 1 ? true : false;
    }

    public static void SetFloat(PlayerPrefsKeys key, float value)
    {
        PlayerPrefs.SetFloat(key.ToString(), value);
    }

    public static float GetFloat(PlayerPrefsKeys key, float defaultValue)
    {
        float value = defaultValue;
        if (PlayerPrefs.HasKey(key.ToString()))
        {
            value = PlayerPrefs.GetFloat(key.ToString());
        }
        else
        {
            SetFloat(key, defaultValue);
        }

        return value;
    }

    public static void SetInt(PlayerPrefsKeys key, int value)
    {
        PlayerPrefs.SetInt(key.ToString(), value);
    }

    public static int GetInt(PlayerPrefsKeys key, int defaultValue)
    {
        int value = defaultValue;
        if (PlayerPrefs.HasKey(key.ToString()))
        {
            value = PlayerPrefs.GetInt(key.ToString());
        }
        else
        {
            SetInt(key, defaultValue);
        }

        return value;
    }

    public static void SetString(PlayerPrefsKeys key, string value)
    {
        PlayerPrefs.SetString(key.ToString(), value);
    }

    public static string GetString(PlayerPrefsKeys key, string defaultValue)
    {
        string value = defaultValue;
        if (PlayerPrefs.HasKey(key.ToString()))
        {
            value = PlayerPrefs.GetString(key.ToString());
        }
        else
        {
            SetString(key, defaultValue);
        }

        return value;
    }


    private static void SetVector3(string key, Vector3 value)
    {
        string x = key + "V3X";
        string y = key + "V3Y";
        string z = key + "V3Z";
        PlayerPrefs.SetFloat(x, value.x);
        PlayerPrefs.SetFloat(y, value.y);
        PlayerPrefs.SetFloat(z, value.z);
    }

    private static Vector3 GetVector3(string key)
    {
        Vector3 value;
        string x = key + "V3X";
        string y = key + "V3Y";
        string z = key + "V3Z";
        value.x = PlayerPrefs.GetFloat(x, 0);
        value.y = PlayerPrefs.GetFloat(y, 0);
        value.z = PlayerPrefs.GetFloat(z, 0);
        return value;
    }

    private static void SetTransform(PlayerPrefsKeys key, Transform value)
    {
        string position = key + "TP";
        string eulerAngles = key + "TE";
        // string scale = key + "TS";
        SetVector3(position, value.position);
        SetVector3(eulerAngles, value.eulerAngles);
        // SetVector3(scale, value.localScale);
    }
    
    private static void GetTransform(PlayerPrefsKeys key, Transform value)
    {
        string position = key + "TP";
        string eulerAngles = key + "TE";
        // string scale = key + "TS";
        value.transform.position = GetVector3(position);
        value.transform.eulerAngles = GetVector3(eulerAngles);
        // SetVector3(scale, value.localScale);
    }
}
