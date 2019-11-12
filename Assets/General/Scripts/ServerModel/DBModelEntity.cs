using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBModelEntity : DBModelMaster
{
    public virtual void SaveToLocal() {
        LoadSetting();

        List<string> col = new List<string>();
        col.AddRange(dbSettings.localDbSetting.columns);
        col.RemoveAt(0);

        List<string> val = new List<string>();

        for (int i = 0; i < col.Count; i++)
        {
            val.Add(PlayerPrefs.GetString(col[i]));
        }

       AddData(col, val);

    }
    public virtual void SyncToServer() { }
}
