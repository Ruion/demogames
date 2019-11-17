using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Sirenix.OdinInspector;

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
