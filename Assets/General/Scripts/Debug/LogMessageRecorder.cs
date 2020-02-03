using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class LogMessageRecorder : MonoBehaviour
{
    private GameSettingEntity gameSettingEntity;
    private void OnEnable()
    {
        gameSettingEntity = FindObjectOfType<GameSettingEntity>();
        Application.logMessageReceived += LogHandler;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= LogHandler;
    }

    void LogHandler(string logMsg, string stack, LogType logType)
    {
        string filePath = string.Format(
            gameSettingEntity.jsonSetter.savePath + "{0}{1}{2}",
            new System.Object[] { "\\DebugMessages\\" , System.DateTime.Now.ToShortDateString().Replace("/", "-"), "_DebugMessage.txt"});

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        StreamWriter stream = new StreamWriter(filePath, true);

        if (logType == LogType.Log)
        {
            stream.WriteLine("[" + System.DateTime.Now.ToString() + "] NORMAL -- " + logMsg);
        }
        else if(logType == LogType.Error || logType == LogType.Exception)
        {
            stream.WriteLine("[" + System.DateTime.Now.ToString() + "] ERROR -- " + logMsg);
        }
        
        stream.Close();
    }
}
