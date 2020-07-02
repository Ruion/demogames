using UnityEngine;
using Unity.RemoteConfig;
using System.Reflection;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using Newtonsoft.Json.Linq;

public class RemoteConfigSetting : MonoBehaviour
{
    public struct userAttributes { }

    public struct appAttributes { }

    public MonoBehaviour obj;
    public string prefix;

    private void OnEnable()
    {
        // fetch setting from file
        LoadSettingFromFile();

        ConfigManager.FetchCompleted += ChangeSettings;
        ConfigManager.FetchConfigs<userAttributes, appAttributes>(new userAttributes(), new appAttributes());
    }

    private void Start()
    {
    }

    [Button]
    private void LoadSettingFromFile()
    {
        // get class field variable
        Type type = obj.GetType();
        FieldInfo[] properties = type.GetFields();

        // get settins from JObject
        JObject json = JSONExtension.LoadJson(FindObjectOfType<GameSettingEntity>().SettingFilePath);

        foreach (JProperty j in json.Properties())
        {
            FieldInfo p = properties.Where(x => prefix + x.Name == j.Name).FirstOrDefault();
            if (p != null)
            {
                if (p.FieldType == typeof(string))
                {
                    p.SetValue(obj, j.Value.ToString());
                }

                if (p.FieldType == typeof(int))
                {
                    p.SetValue(obj, System.Convert.ToInt32(j.Value));
                }

                if (p.FieldType == typeof(float))
                {
                    p.SetValue(obj, (float)j.Value);
                }

                if (p.FieldType == typeof(bool))
                {
                    p.SetValue(obj, (bool)j.Value);
                }
            }
        }
    }

    private void ChangeSettings(ConfigResponse response)
    {
        Type type = obj.GetType();
        FieldInfo[] properties = type.GetFields();

        bool isChanged = false;

        foreach (var p in properties)
        {
            if (!ConfigManager.appConfig.HasKey(prefix + p.Name))
                continue;

            //Debug.Log(p.Name);
            if (p.FieldType == typeof(string))
            {
                if (p.GetValue(obj).ToString() != ConfigManager.appConfig.GetString(prefix + p.Name))
                {
                    p.SetValue(obj, ConfigManager.appConfig.GetString(prefix + p.Name));
                    isChanged = true;
                }
            }

            if (p.FieldType == typeof(int))
            {
                if ((int)p.GetValue(obj) != ConfigManager.appConfig.GetInt(prefix + p.Name))
                {
                    p.SetValue(obj, ConfigManager.appConfig.GetInt(prefix + p.Name));
                    isChanged = true;
                }
            }

            if (p.FieldType == typeof(float))
            {
                if ((float)p.GetValue(obj) != ConfigManager.appConfig.GetFloat(prefix + p.Name))
                {
                    p.SetValue(obj, ConfigManager.appConfig.GetFloat(prefix + p.Name));
                    isChanged = true;
                }
            }

            if (p.FieldType == typeof(bool))
            {
                if ((bool)p.GetValue(obj) != ConfigManager.appConfig.GetBool(prefix + p.Name))
                {
                    p.SetValue(obj, ConfigManager.appConfig.GetBool(prefix + p.Name));
                    isChanged = true;
                }
            }

            if (isChanged)
                JSONExtension.SaveSetting(FindObjectOfType<GameSettingEntity>().SettingFilePath, prefix + p.Name, ConfigManager.appConfig.GetString(prefix + p.Name));
        }

        if (obj == null) return;
        obj.enabled = false;
        obj.enabled = true;
    }

    public void FetchSettings()
    {
        ConfigManager.FetchConfigs<userAttributes, appAttributes>(new userAttributes(), new appAttributes());
    }

    private void OnDisable()
    {
        ConfigManager.FetchCompleted -= ChangeSettings;
    }
}