﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerPrefsSaver : MonoBehaviour
{
    public string name_;

    public void Save(InputField inputField)
    {
        PlayerPrefs.SetString(name_, inputField.text.ToString());
    }

    public void Save(TMP_InputField inputField)
    {
        PlayerPrefs.SetString(name_, inputField.text.ToString());
    }

    public void Save(TextMeshProUGUI text_)
    {
        PlayerPrefs.SetString(name_, text_.text.ToString());
    }

    public void Save(string value)
    {
        PlayerPrefs.SetString(name_, value);
    }

    public void Save(ScriptableScore scoreCard)
    {
        PlayerPrefs.SetString(name_, scoreCard.score.ToString());
    }

    [ContextMenu("DateTime")]
    public void SaveDateTime()
    {
        PlayerPrefs.SetString(name_, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        Debug.Log(PlayerPrefs.GetString(name_));
       // Debug.Log(System.DateTime.UtcNow);
    }
}

