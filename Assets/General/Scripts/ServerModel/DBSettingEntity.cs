using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class DBSettingEntity : MonoBehaviour
{
    [Header("DBSetting - SAVE setting every new project")]
    public DBEntitySetting dbSettings;

    #region Basics
    public virtual void Awake()
    {
       // LoadDBSettingFromMaster();
    }

    [ContextMenu("SaveSetting")]
    public virtual void SaveSetting()
    {
        DBSetting.SaveSetting(dbSettings);
    }

    [ContextMenu("LoadSetting")]
    public virtual void LoadSetting()
    {
        dbSettings = DBSetting.LoadSetting(dbSettings.fileName);
    }

    [ContextMenu("LoadMasterSetting")]
    public virtual void LoadDBSettingFromMaster()
    {
        DBSettingEntity dm = GameObject.Find("DBSettingEntityMaster_DoNotChangeName").GetComponent<DBSettingEntity>();
        if (dm == this) return;

        dm.LoadSetting();
        dbSettings = dm.dbSettings;

    }
    #endregion

    #region Local DB

    #endregion
}
