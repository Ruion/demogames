using System.Collections.Generic;
using UnityEngine;
using DataBank;

[RequireComponent(typeof(UniversalUserDB))]
public class UserLocalModel : ServerModelMaster
{

    private UniversalUserDB udb;

    List<DataBank.UniversalUserEntity> unSyncUsers = new List<DataBank.UniversalUserEntity>();

    public void StartUp()
    {
        SetUpDb();

        #region Function for compare data with server - DISABLED
        // localEmailList = udb.GetAllUserEmail();

        //  DoGetDataFromServer();
        #endregion

        udb.Close();
    }

    private void SetUpDb()
    {
        if (udb == null) udb = GetComponent<UniversalUserDB>();
        udb.ConnectDbCustom();
    }

    #region Save Data
    public override void SaveToLocal()
    {
        LoadGameSettingFromMaster();

        List<string> col = new List<string>();
        col.AddRange(gameSettings.sQliteDBSettings.columns);
        col.RemoveAt(0);

        List<string> val = new List<string>();

        for (int i = 0; i < col.Count; i++)
        {
            val.Add(PlayerPrefs.GetString(col[i]));
        }

        SetUpDb();

        udb.AddData(col, val);

        udb.Close();

    }

    // to be change
    public void SaveScoreToLocal()
    {
        LoadGameSettingFromMaster();

        List<string> col = new List<string>();
        col.AddRange(gameSettings.sQliteDBSettings.columns);
        col.RemoveAt(0);

        List<string> val = new List<string>();

        for (int i = 0; i < col.Count; i++)
        {
            val.Add(PlayerPrefs.GetString(col[i]));
        }

        SetUpDb();

        udb.AddData(col, val);

        udb.Close();

    }

    #endregion

}
