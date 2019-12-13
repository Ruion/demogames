using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

public static class JSONExtension
{
    public static void SaveSetting(System.Object obj, string filePath)
    {
        try
            {
            // Debug.Log(Path.GetDirectoryName(filePath));
             Directory.CreateDirectory(Path.GetDirectoryName(filePath));
             var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
                File.WriteAllText(filePath + ".json", json);
                Debug.Log("Save setting success");
            }
            catch(System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
    }

    public static System.Object LoadSetting(System.Object obj, string filePath)
    {
        string path = filePath + ".json";

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        if (File.Exists(path))
        {
            System.Object jsonObj = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(path));

            return jsonObj;
        }
        else
        {
            Debug.LogError("file not found in " + path);
            return null;
        }
    }
}
