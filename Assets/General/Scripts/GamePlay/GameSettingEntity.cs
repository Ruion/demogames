using UnityEngine;

/// <summary>
/// Master class for Saving and Retrieving gameplay settings.
/// Tips: Inherit this class to access same game setting in different scenes, extend the variable in Settings{} class as you like
/// By default gameObject inherit this class will execute LoadGameSettingFromMaster() in Awake()
/// </summary>
public class GameSettingEntity : MonoBehaviour
{
    [Header("GameSetting - SAVE setting every new project")]
    public Settings gameSettings;

    public string Server_URL { get { return jsonSetter.LoadGlobalSettingFile(@"C:\UID_Toolkit\Global.json")["Server_URL"].ToString(); } }

    public string Project_Folder
    {
        get
        {
            return @"C:\UID-APP\" + jsonSetter.LoadGlobalSettingFile(@"C:\UID_Toolkit\Global.json")["Project_Code"].ToString();
        }
    }

    public JSONSetter jsonSetter;

    /// <summary>
    /// Save the settings value in to file. This function eccesible by right click component in inspector and click "SaveSetting"
    /// </summary>
    [ContextMenu("SaveSetting")]
    public virtual void SaveSetting()
    {
        // GameSetting.SaveSetting(gameSettings);

        string filePath = jsonSetter.savePath + "\\Settings\\" + "Setting";

        JSONExtension.SaveValues(filePath, gameSettings);

        Debug.Log("Save setting success");
    }

    /// <summary>
    /// Load the settings value from file. This function eccesible by right click component in inspector and click "LoadSetting"
    /// </summary>
    [ContextMenu("LoadSetting")]
    public virtual void LoadSetting()
    {
        /*
        gameSettings.savePath = jsonSetter.savePath;
        gameSettings = GameSetting.LoadSetting(gameSettings);
        */

        string filePath = jsonSetter.savePath + "\\Settings\\" + "Setting";

        JSONExtension.LoadValues(filePath, gameSettings);

        Debug.Log(Server_URL);
    }

    /// <summary>
    /// Load settings from a gameObject call "GameSettingEntity_DoNotChangeName"
    /// </summary>
    [ContextMenu("LoadMasterSetting")]
    public virtual void LoadGameSettingFromMaster()
    {
        GameSettingEntity dm = GameObject.Find("GameSettingEntity_DoNotChangeName").GetComponent<GameSettingEntity>();
        if (dm == this) return;

        dm.LoadSetting();
        gameSettings = dm.gameSettings;
    }

    public virtual void Awake()
    {
        // BetterStreamingAssets.Initialize();
        if (jsonSetter == null) jsonSetter = FindObjectOfType<JSONSetter>();

        LoadSetting();
        LoadGameSettingFromMaster();
    }
}

/// <summary>
/// Settings to be used by gameplay objects
/// add variable like speed, hp, game time and make other class to use those values, to made tweaking and testing easier.
/// </summary>
[System.Serializable]
public class Settings
{
    //public string scoreName = "game_score";
    //public int scoreToWin = 3;
    public bool debugMode = false;

    public string serverDomainURL;
    public string source_identifier_code;

    //public string userPrimaryKeyName = "userPrimaryKey";
    public int checkInternetTimeOut = 5000;
}