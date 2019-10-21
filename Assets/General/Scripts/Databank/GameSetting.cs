using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class GameSetting
{
    // save the game setting file
    public static void SaveSetting(Settings setting)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.streamingAssetsPath + "/" + setting.fileName + ".setting";
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
    public static Settings LoadSetting(string fileName)
    {
        string path = Application.streamingAssetsPath + "/" + fileName + ".setting";

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

[System.Serializable]
public class Settings
{
    public string fileName = "game_setting.setting";
    public string dbName = "user_db";
    public string scoreName = "score";
    public string tableName = "user";
    public string serverAddress = "http://192.168.0.28/honda/submit-data.php";
    public string serverGetDataAddress = "http://192.168.0.28/honda/submit-data.php";

    public Settings(Settings setting)
    {
        fileName = setting.fileName;
        dbName = setting.dbName;
        scoreName = setting.scoreName;
        tableName = setting.tableName;
        serverAddress = setting.serverAddress;
        serverGetDataAddress = setting.serverGetDataAddress;
    }

    public Settings(){  }
}