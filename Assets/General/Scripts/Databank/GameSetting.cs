using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

public static class GameSetting
{
    // save the game setting file
    public static void SaveSetting(Settings setting)
    {
        /* old binary formatter
        BinaryFormatter formatter = new BinaryFormatter();
        string path = setting.savePath + "\\" + setting.fileName + ".setting";
        FileStream stream = new FileStream(path, FileMode.Create);

        Settings data = new Settings(setting);
        */

        // save to .json file
      /*
        JsonSerializer serializer = new JsonSerializer();
        serializer.Converters.Add(new JavaScriptDateTimeConverter());
        serializer.NullValueHandling = NullValueHandling.Ignore;

        using (StreamWriter file = File.CreateText(path.Replace("setting", "json")))
        using (JsonTextWriter writer = new JsonTextWriter(file))
        {
            serializer.Serialize(writer, setting);
        }
    */
        try
        {
            var json = JsonConvert.SerializeObject(setting, Formatting.Indented);
            File.WriteAllText(setting.savePath + "\\" + setting.fileName + ".json", json);
            Debug.Log("Save setting success");
        }
        catch(System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        /*
        try
        {
            formatter.Serialize(stream, data);
            stream.Close();
            Debug.Log("Save setting success");

        }catch(System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        */
    }

    // load the game setting file
    public static Settings LoadSetting(Settings setting_)
    {
        string path = setting_.savePath + "\\" + setting_.fileName + ".setting";

        if (File.Exists(path))
        {
            /* old binary formatter method
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Settings setting = (Settings)formatter.Deserialize(stream);
            stream.Close();
            */

            Debug.Log(path.Replace("setting", "json"));
            Settings setting = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(path.Replace("setting", "json")));

            return setting;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}

