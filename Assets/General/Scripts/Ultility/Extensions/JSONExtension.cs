using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

public static class JSONExtension
{
    /// <summary>
    /// Save an object into json file
    /// </summary>
    public static void SaveObject(string filePath, System.Object Object)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath + ".json", JsonConvert.SerializeObject(Object, Formatting.Indented));
    }

    #region Essential

    /// <summary>
    /// Save single property name into .json file
    /// </summary>
    public static void SaveSetting(string filePath, string pName, string pValue)
    {
        // Usage
        // SaveSetting("file path without extension", "PropertyName", "Value");
        // SaveSetting("C:\\UID-TOOLKIT\\Settings", "ProjectFolder", "C:\\UID-APP\\200306-MY-VictoriaSecret");

        // Create folder at the given path
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        if (!filePath.Contains(".json"))
            filePath += ".json";

        // Create a .json file with "{}" text in the file
        if (!File.Exists(filePath)) File.WriteAllText(filePath, "{}");

        // Load the json file
        JObject jsonObj = LoadJson(filePath);

        // Save the value into "propertyName" to file if not exist
        if (!jsonObj.ContainsKey(pName))
        {
            jsonObj.Add(new JProperty(pName, pValue));
        }
        else
        {
            // Save the value into "propertyName" to file if exist
            jsonObj[pName] = pValue;
        }

        // Write the json text into a file
        File.WriteAllText(filePath, JsonConvert.SerializeObject(jsonObj, Formatting.Indented));

        // Pure C# delete this line
        Debug.Log(string.Format("Save to file {0}", filePath));
    }

    // Load single property from .json file
    public static string LoadSetting(string filePath, string pName)
    {
        // Usage
        // string source_identifier_code = JSONExtension.LoadSetting("file path without extension", "property name");
        // string example1 = JSONExtension.LoadSetting("C:\\UID-TOOLKIT\\Settings", "ProjectFolder");

        JObject jsonObj = LoadJson(filePath);

        if (!jsonObj.ContainsKey(pName))
        {
            Debug.LogError("Property not exist in setting : " + pName);
            return null;
        }
        else
        {
            return jsonObj[pName].ToString();
        }
    }

    // Load a .json file's text and parse to JSON format
    public static JObject LoadJson(string filePath)
    {
        if (!filePath.Contains(".json"))
        {
            filePath += ".json";
        }

        // Read the file text
        string json = File.ReadAllText(filePath);

        // Deserialize into json format
        JObject jsonObj = JObject.Parse(json);

        return jsonObj;
    }

    #endregion Essential

    #region Utilities

    // Save object properties into json file
    public static void SaveValues(string filePath, System.Object Object)
    {
        FieldInfo[] pros = Object.GetType().GetFields();
        for (int i = 0; i < pros.Length; i++)
        {
            SaveSetting(filePath, pros[i].Name, pros[i].GetValue(Object).ToString());
        }
    }

    // Load object properties from json file
    public static void LoadValues(string filePath, System.Object Object)
    {
        FieldInfo[] fields = Object.GetType().GetFields();

        for (int i = 0; i < fields.Length; i++)
        {
            if (fields[i].FieldType == typeof(string))
            {
                fields[i].SetValue(Object, LoadSetting(filePath, fields[i].Name));
            }
            else if (fields[i].FieldType == typeof(int))
            {
                fields[i].SetValue(Object, System.Int32.Parse(LoadSetting(filePath, fields[i].Name)));
            }
            else if (fields[i].FieldType == typeof(bool))
            {
                fields[i].SetValue(Object, bool.Parse(LoadSetting(filePath, fields[i].Name)));
            }
        }
    }

    public static JObject JObjectFromString(this string jsonString)
    {
        if (!jsonString.StartsWith("{"))
        {
            Debug.Log("string not end with '{'");
            return null;
        }

        if (!jsonString.EndsWith("}"))
        {
            Debug.Log("string not end with '}'");
            return null;
        }

        return JObject.Parse(jsonString);
    }

    #endregion Utilities
}