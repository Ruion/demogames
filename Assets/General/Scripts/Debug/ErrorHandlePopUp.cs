using UnityEngine;
using TMPro;

public class ErrorHandlePopUp : GameSettingEntity
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