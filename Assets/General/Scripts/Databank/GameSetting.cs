using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public static class GameSetting
{
    // save the game setting file
    public static void SaveSetting(Settings setting)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = setting.savePath + "\\" + setting.fileName + ".setting";
        FileStream stream = new FileStream(path, FileMode.Create);

        Settings data = new Settings(setting);

        try
        {
            formatter.Serialize(stream, data);
            stream.Close();
            Debug.Log("Save setting success");

        }catch(System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    // load the game setting file
    public static Settings LoadSetting(Settings setting_)
    {
        string path = setting_.savePath + "\\" + setting_.fileName + ".setting";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Settings setting = (Settings)formatter.Deserialize(stream);
            stream.Close();

            return setting;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}


/// <summary>
/// Settings to be used by gameplay objects
/// add variable like speed, hp, game time and make other class to use those values, to made tweaking and testing easier.
/// </summary>
[System.Serializable]
public class Settings
{
    [FolderPath(AbsolutePath=true, UseBackslashes=true)] public string savePath;
    public string fileName = "game_setting";
    public string scoreName = "game_score";
    public int scoreToWin = 3;
    public bool DebugMode = false;
 
    public Settings(Settings setting)
    {
        savePath = setting.savePath;
        DebugMode = setting.DebugMode;

        fileName = setting.fileName;
        scoreName = setting.scoreName;
        scoreToWin = setting.scoreToWin;
    }
}
