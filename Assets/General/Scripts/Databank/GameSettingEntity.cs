using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

/// <summary>
/// Master class for Saving and Retrieving gameplay settings.
/// Tips: Inherit this class to access same game setting in different scenes, extend the variable in Settings{} class as you like
/// By default gameObject inherit this class will execute LoadGameSettingFromMaster() in Awake()
/// </summary>
public class GameSettingEntity : MonoBehaviour
{
    
    [Header("GameSetting - SAVE setting every new project")]
    public Settings gameSettings;
    public JSONSetter jsonSetter;

    /// <summary>
    /// Save the settings value in to file. This function eccesible by right click component in inspector and click "SaveSetting"
    /// </summary>
    [ContextMenu("SaveSetting")]
    public virtual void SaveSetting()
    {
        GameSetting.SaveSetting(gameSettings);

        /// Save to a global json file
        JProperty[] jProperties = new JProperty[2];
        jProperties[0] = new JProperty("scoreToWin", gameSettings.scoreToWin);
        jProperties[1] = new JProperty("DebugMode", gameSettings.DebugMode);
        jsonSetter.UpdateSetting(jProperties);
    }

    /// <summary>
    /// Load the settings value from file. This function eccesible by right click component in inspector and click "LoadSetting"
    /// </summary>
    [ContextMenu("LoadSetting")]
    public virtual void LoadSetting()
    {
        gameSettings.savePath = jsonSetter.savePath;
        gameSettings = GameSetting.LoadSetting(gameSettings);

        JObject globalSetting = jsonSetter.LoadSetting();
        gameSettings.scoreToWin = System.Int32.Parse(globalSetting["scoreToWin"].ToString());
        gameSettings.DebugMode = (bool)globalSetting["DebugMode"];
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
        LoadGameSettingFromMaster();
    }
}
