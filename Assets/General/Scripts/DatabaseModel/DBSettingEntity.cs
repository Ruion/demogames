using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Base class for DBModelMaster
/// Tips: Inherit this class to allow accessing database setting
/// </summary>
public class DBSettingEntity : SerializedMonoBehaviour
{
    public DBEntitySetting dbSettings;

    #region Basics
    public virtual void Awake()
    {
        LoadSetting();
    }

    [Button(ButtonSizes.Large), GUIColor(.44f, .53f, .98f)][ButtonGroup("Setting")]
    public virtual void SaveSetting()
    {
        DBSetting.SaveSetting(dbSettings);
        LoadSetting();
    }

    [Button(ButtonSizes.Large), GUIColor(.44f, .53f, .98f)][ButtonGroup("Setting")]
    public virtual void LoadSetting()
    {
        dbSettings = DBSetting.LoadSetting(dbSettings.fileName);
    }
    #endregion

}
