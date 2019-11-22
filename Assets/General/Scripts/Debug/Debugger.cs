using UnityEngine;
using TMPro;

namespace Universal{
/// <summary>
/// Class to help debug exported app. It will show error message panel when app encounter error
/// This class can be toogle by bool gameSettings.DebugMode inherit from GameSettingEntity
/// Tips: Drag "Debugger" prefab to the first scene of the app and it will persist across scene
/// </summary>
public class Debugger : GameSettingEntity
{
    public GameObject PopUp;
    public TextMeshProUGUI title;
    public TextMeshProUGUI msg;
    string error;

    public override void Awake()
    {
        base.Awake();
        if (!gameSettings.DebugMode) gameObject.SetActive(false);
    }

    // Use this for initialization
    void Start()
    {}

    void OnEnable()
    {
        if (!gameSettings.DebugMode) gameObject.SetActive(false);

        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error)
        {
            error = error + "\n" + " Error: " + logString;
            PopUp.SetActive(true);
            title.text = "Error";
            msg.text = error;
        }

    }
}
}