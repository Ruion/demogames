using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RemoteConfig;
using System.Reflection;
using Sirenix.OdinInspector;
using System;

public class RemoteConfigManager : MonoBehaviour
{
    public struct userAttributes { }

    public struct appAttributes { }

    public GameSettingEntity gse;

    private void Awake()
    {
        gse.LoadSetting();
        ConfigManager.FetchCompleted += ChangeSettings;
        ConfigManager.FetchConfigs<userAttributes, appAttributes>(new userAttributes(), new appAttributes());
    }

    private void ChangeSettings(ConfigResponse response)
    {
        Type type = gse.gameSettings.GetType();
        FieldInfo[] properties = type.GetFields();

        bool isChanged = false;

        foreach (var p in properties)
        {
            if (!ConfigManager.appConfig.HasKey(p.Name))
                continue;

            if (p.FieldType == typeof(string))
            {
                if (p.GetValue(gse.gameSettings).ToString() != ConfigManager.appConfig.GetString(p.Name))
                {
                    p.SetValue(gse.gameSettings, ConfigManager.appConfig.GetString(p.Name));
                    isChanged = true;
                }
            }

            if (p.FieldType == typeof(int))
            {
                if ((int)p.GetValue(gse.gameSettings) != ConfigManager.appConfig.GetInt(p.Name))
                {
                    p.SetValue(gse.gameSettings, ConfigManager.appConfig.GetInt(p.Name));
                    isChanged = true;
                }
            }

            if (p.FieldType == typeof(float))
            {
                if ((float)p.GetValue(gse.gameSettings) != ConfigManager.appConfig.GetFloat(p.Name))
                {
                    p.SetValue(gse.gameSettings, ConfigManager.appConfig.GetFloat(p.Name));
                    isChanged = true;
                }
            }

            if (p.FieldType == typeof(bool))
            {
                if ((bool)p.GetValue(gse.gameSettings) != ConfigManager.appConfig.GetBool(p.Name))
                {
                    p.SetValue(gse.gameSettings, ConfigManager.appConfig.GetBool(p.Name));
                    isChanged = true;
                }
            }

            if (isChanged)
                JSONExtension.SaveSetting(FindObjectOfType<GameSettingEntity>().SettingFilePath, p.Name, ConfigManager.appConfig.GetString(p.Name));
        }
    }

    [Button]
    public void FetchSettings()
    {
        ConfigManager.FetchConfigs<userAttributes, appAttributes>(new userAttributes(), new appAttributes());
    }

    private void OnDestroy()
    {
        ConfigManager.FetchCompleted -= ChangeSettings;
    }
}