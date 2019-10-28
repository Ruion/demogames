using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GameSettingEntity : MonoBehaviour
{
    [Header("GameSetting - SAVE setting every new project")]
    public Settings gameSettings;

    [ContextMenu("SaveSetting")]
    public virtual void SaveSetting()
    {
        GameSetting.SaveSetting(gameSettings);
    }

    [ContextMenu("LoadSetting")]
    public virtual void LoadSetting()
    {
        gameSettings = GameSetting.LoadSetting(gameSettings.fileName);
        Refresh();
    }

    [ContextMenu("LoadMasterSetting")]
    public virtual void LoadGameSettingFromMaster()
    {
        GameSettingEntity dm = GameObject.Find("GameSettingEntity_DoNotChangeName").GetComponent<GameSettingEntity>();
        if (dm == this) return;

        dm.LoadSetting();
        gameSettings = dm.gameSettings;

    }

    public virtual void Refresh()
    {
        // get columns name from class fields
        if (gameSettings.sQliteDBSettings.columns.Count < 1)
        {
            try
            {
                Type t = Type.GetType(gameSettings.sQliteDBSettings.UniversalUserClassName, true);

                Debug.Log(t.Name);

                var obj = Activator.CreateInstance(t);

                FieldInfo[] fields = t.GetFields();

                foreach (FieldInfo p in fields)
                {
                    Debug.Log("column name : " + p.Name);
                    gameSettings.sQliteDBSettings.columns.Add(p.Name);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }
    }

    public virtual void Awake()
    {
        LoadGameSettingFromMaster();
    }
}
