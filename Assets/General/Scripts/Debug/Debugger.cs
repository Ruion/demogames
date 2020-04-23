using UnityEngine;
using TMPro;
using System.IO;
using System;
using System.Collections.Generic;

namespace Universal
{
    /// <summary>
    /// Class to help debug exported app. It will show error message panel when app encounter error
    /// This class can be toogle by bool gameSettings.DebugMode inherit from GameSettingEntity
    /// Tips: Drag "Debugger" prefab to the first scene of the app and it will persist across scene
    /// </summary>
    public class Debugger : MonoBehaviour
    {
        public GameObject PopUp;
        public TextMeshProUGUI title;
        public TextMeshProUGUI msg;
        private string error;

        private GameSettingEntity gameSettingEntity;

        private void Start()
        {
        }

        private void OnEnable()
        {
            gameSettingEntity = FindObjectOfType<GameSettingEntity>();
            //  Application.logMessageReceived += HandleLog;
            //  Debug.Log(name + " : addded Debugger");
            if (gameSettingEntity.gameSettings.debugMode) { Application.logMessageReceived += HandleLog; Debug.Log(name + " : addded Debugger"); }
            else gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            if (gameSettingEntity.gameSettings.debugMode)
                Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error)
            {
                error = error + "\n" + " Error: " + logString;
                PopUp.SetActive(true);
                title.text = "Error";
                msg.text = error;
                Debug.Log(name + " : " + msg.text);
                JSONExtension.SaveSetting(gameSettingEntity.jsonSetter.Project_Folder + "\\ErrorLog", DateTime.Now.ToString(), logString);
            }
        }
    }
}

[System.Serializable]
public class ErrorLog
{
    public Dictionary<string, string> errorMessage = new Dictionary<string, string>();
}