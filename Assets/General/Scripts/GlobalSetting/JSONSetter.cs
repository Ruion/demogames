using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

public class JSONSetter : MonoBehaviour
{
    [FolderPath(AbsolutePath=true, UseBackslashes=true)]
    public string savePath;

    [FolderPath(AbsolutePath=true, UseBackslashes=true)]
    public string globalSavePath;

    private string fileName = "Setting.json";
    private string globalFileName = "GlobalSetting.json";

    private string slash = "\\";

    public void SaveSetting(JObject jsonObj, bool global = false)
    {
        string savePath = this.savePath;

        string fileName = this.fileName;

        if(global) 
        {
            savePath = globalSavePath;
            fileName = this.globalFileName;
        }

        // write JSON directly to a file
        using (StreamWriter file = File.CreateText(savePath + GetPlatFormSlash() + fileName))
        using (JsonTextWriter writer = new JsonTextWriter(file))
        {
            jsonObj.WriteTo(writer);
        }
    }

    public void UpdateSetting(string name, string value)
    {
        JObject jsonObj = LoadSetting();

        if(!jsonObj.ContainsKey(name) && !string.IsNullOrEmpty(value))
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

        if(!jsonObj.ContainsKey(name) && !string.IsNullOrEmpty(value))
        {
            jsonObj.Add(new JProperty(name, value));
        }
        else
        {
            jsonObj[name] = value;
        }

        SaveSetting(jsonObj, true);
    }

    public JObject LoadSetting(bool global = false)
    {
        string savePath = this.savePath;
        if(global) 
        {
            savePath = globalSavePath;
            fileName = this.globalFileName;
        }

        string json = File.ReadAllText(savePath + GetPlatFormSlash() + "Setting.json");
        JObject jsonObj = JObject.Parse(json);

        return jsonObj;
    }

    private string GetPlatFormSlash()
    {
        /*
        if(Application.platform == RuntimePlatform.Android)
        {
            slash = "/";
        }
        */

        if(Application.platform != RuntimePlatform.WindowsPlayer)
        {
            slash = "/";
        }
        return slash;
    }
}
