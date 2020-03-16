using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

public class JSONSetter : MonoBehaviour
{
    [FolderPath(AbsolutePath = true, UseBackslashes = true)]
    public string savePath { get { return @"C:\UID-APP\" + LoadGlobalSettingFile(@"C:\UID_Toolkit\Global.json")["Project_Code"].ToString(); } }

    [FolderPath(AbsolutePath = true, UseBackslashes = true)]
    public string globalSavePath;

    private string fileName = "Setting.json";
    private string globalFileName = "GlobalSetting.json";

    public void SaveSetting(JObject jsonObj)
    {
        string savePath = this.savePath;

        string fileName = this.fileName;

        Directory.CreateDirectory(Path.GetDirectoryName(savePath + "\\Settings\\" + fileName));
        // write JSON directly to a file
        using (StreamWriter file = File.CreateText(savePath + "\\Settings\\" + fileName))
        using (JsonTextWriter writer = new JsonTextWriter(file))
        {
            jsonObj.WriteTo(writer);
        }
    }

    public void UpdateSetting(string name, string value)
    {
        JObject jsonObj = LoadSetting();

        if (!jsonObj.ContainsKey(name) && !string.IsNullOrEmpty(value))
        {
            jsonObj.Add(new JProperty(name, value));
        }
        else
        {
            jsonObj[name] = value;
        }

        SaveSetting(jsonObj);
    }

    public void UpdateSettingGlobal(string name, string value)
    {
        JObject jsonObj = LoadSetting();

        if (!jsonObj.ContainsKey(name) && !string.IsNullOrEmpty(value))
        {
            jsonObj.Add(new JProperty(name, value));
        }
        else
        {
            jsonObj[name] = value;
        }

        SaveSetting(jsonObj);
    }

    public JObject LoadSetting()
    {
        string savePath = this.savePath;

        string json = File.ReadAllText(savePath + "\\Settings\\" + "Setting.json");
        JObject jsonObj = JObject.Parse(json);

        return jsonObj;
    }

    public JObject LoadGlobalSettingFile(string filePath)
    {
        string json = File.ReadAllText(filePath);
        JObject jsonObj = JObject.Parse(json);

        return jsonObj;
    }
}