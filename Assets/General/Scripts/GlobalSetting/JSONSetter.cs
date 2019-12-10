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

    public void SaveSetting(JObject jsonObj, bool global = false)
    {
        string savePath = this.savePath;
        if(global) savePath = globalSavePath;
        //FileStream fs = File.Open(filePath + "\\" + "Setting.json", FileMode.OpenOrCreate);

        //File.WriteAllText(JsonfilePath, jsonObj.ToString());

        // write JSON directly to a file
        using (StreamWriter file = File.CreateText(savePath + "\\" + "Setting.json"))
        using (JsonTextWriter writer = new JsonTextWriter(file))
        {
            jsonObj.WriteTo(writer);
        }

    }

    public void UpdateSetting(string name, string value)
    {
        JObject jsonObj = LoadSetting();

        if(!jsonObj.ContainsKey(name))
        {
            jsonObj.Add(new JProperty(name, value));
        }
        else
        {
            jsonObj[name] = value;
        }

        SaveSetting(jsonObj);
    }

    public void UpdateSetting(JProperty[] jProperties)
    {
        JObject jsonObj = LoadSetting();

        foreach (JProperty p in jProperties)
        {
            if(!jsonObj.ContainsKey(p.Name))
            {
                jsonObj.Add(p);
                Debug.Log(p.Name + " is added");
            }
            else
            {
                jsonObj[p.Name] = p.Value;
            }
        }
        SaveSetting(jsonObj);
    }

    public void UpdateSetting(JArray jArray)
    {
        JObject jsonObj = LoadSetting();

        foreach (JProperty o in jArray)
        {
            if(!jsonObj.ContainsKey(o.Name))
            {
                jsonObj.Add(o);
                Debug.Log(o.Name + " is added");
            }
            else
            {
                jsonObj[o.Name] = o.Value;
            }
        }
        SaveSetting(jsonObj);
    }

    public void UpdateSettingGlobal(string name, string value)
    {
        JObject jsonObj = LoadSetting();

        if(!jsonObj.ContainsKey(name))
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
        if(global) savePath = globalSavePath;

         string json = File.ReadAllText(savePath + "\\" + "Setting.json");
        JObject jsonObj = JObject.Parse(json);

        return jsonObj;
    }
}
