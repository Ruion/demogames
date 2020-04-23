using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Data;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using System.IO;
using Sirenix.OdinInspector;

/// <summary>
/// Create/delete table, insert/update/delete data, and send data to server \n
/// Tips: Set the field variable in inspector, use button to help create, delete, update table \n
/// Local > set fileName in inspector, table columns and attributes \n
/// Server > set server url, get data url \n
/// Handler > toogle hasSync and drag message gameObjects into corresponsding field \n
/// Tips: \n
/// Insert player data > use PlayerPrefsSaver or PlayerPrefsSave_Group to save the value
/// with name same with table column, call SaveToLocal() to insert data into database
/// </summary>
public class DBModelEntity : DBModelMaster
{
    private DataRowCollection rows;

    [FoldoutGroup("OperationEvent")] public UnityEvent OnSaveToLocal;
    [FoldoutGroup("OperationEvent")] public UnityEvent OnSyncStart;
    [FoldoutGroup("OperationEvent")] public UnityEvent OnSyncEnd;
    private bool Saved = false;

    [FoldoutGroup("UWP APP Communication")]
    [Header("Fill in number if saving data from a file")]
    public int dataFilePathIndex;

    /// <summary>
    /// Save PlayerPrefs value into all table column set in inspector
    /// </summary>
    public override void SaveToLocal()
    {
        base.SaveToLocal();

        if (Saved)
        {
            Debug.Log(name + " : Data already saved to local");
            return;
        }

        Saved = true;

        List<string> col = new List<string>();
        List<string> val = new List<string>();

        for (int v = 1; v < dbSettings.columns.Count; v++)
        {
            col.Add(dbSettings.columns[v].name);
        }

        val.AddRange(col);

        for (int i = 0; i < col.Count; i++)
        {
            val[i] = PlayerPrefs.GetString(col[i]);
        }

        AddData(col, val);
        if (OnSaveToLocal.GetPersistentEventCount() > 0) OnSaveToLocal.Invoke();
    }

    /// <summary>
    /// Load setting like send url and call SyncToServer() coroutine to Send data to server
    /// </summary>
    public override void Sync()
    {
        base.Sync();

        StartCoroutine(SyncToServer());
    }

    /// <summary>
    /// Make web request and send data to server, data will continue to send regardless of any error encounter
    /// </summary>
    /// <returns></returns>
    private IEnumerator SyncToServer()
    {
        if (OnSyncStart.GetPersistentEventCount() > 0) OnSyncStart.Invoke();

        yield return StartCoroutine(CompareServerData());

        rows = GetAllCustomCondition();

        if (rows.Count < 1)
        {
            SyncEnded();
            StopAllCoroutines();
            Debug.Log(name + " no data to sync. Stop SyncToServer() coroutine");
            yield break;
        }

        //  Debug.Log(name + " : unsync data : " + rows.Count);
        int totalSent = 0;
        int totalNotSent = 0;

        Debug.Log(name + " : Start sync");
        ToogleHandler(blockDataHandler, true);

        // Get global event_code
        string source_identifier_code = JSONExtension.LoadSetting(dbSettings.folderPath + "\\Settings\\Setting", "source_identifier_code");

        for (int u = 0; u < rows.Count; u++)
        {
            yield return StartCoroutine(NetworkExtension.CheckForInternetConnectionRoutine());

            if (NetworkExtension.internet == false)
            {
                //No connection
                ToogleHandler(blockDataHandler, false);
                ToogleHandler(internetErrorHandler, false);
                Debug.Log(name + "SyncToServer() Failed. No internet. Stop SyncToServer() coroutine");
                SyncEnded();
                StopAllCoroutines();
                yield break;
            }

            #region WWW Form

            WWWForm form = new WWWForm();

            // Debug.Log("field to sent : " + dbSettings.columnsToSync.Count);
            // show value send to server - 1
            string values = "";
            entityId = int.Parse(rows[u]["id"].ToString());

            for (int i = 0; i < dbSettings.columns.Count; i++)
            {
                if (!dbSettings.columns[i].sync) continue;

                string value = rows[u][dbSettings.columns[i].name].ToString();

                if (dbSettings.columns[i].name == "source_identifier_code") value = source_identifier_code;

                // show value send to server - 2
                values += value + " | ";

                form.AddField(
                    dbSettings.columns[i].name,
                   value);
            }

            // show value send to server - 3
            // Debug.Log(values);

            #endregion WWW Form

            #region WebRequest

            using (UnityWebRequest www = UnityWebRequest.Post((dbSettings.sendURL + dbSettings.sendAPI).Replace(" ", string.Empty), form))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    ErrorAction(www, "server error \n Values : " + values);
                    totalNotSent++;
                    ToogleStatusBar(failBar, totalNotSent);
                    ToogleHandler(errorHandler, true);
                    ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET is_sync = 'fail' WHERE id = " + entityId);
                    //  SyncEnded();
                    //  yield break;
                }
                else
                {
                    //  yield return new WaitForEndOfFrame();
                    var jsonData = JsonUtility.FromJson<JSONResponse>(www.downloadHandler.text);

                    if (jsonData.result != "Success")
                    {
                        totalNotSent++;
                        ToogleStatusBar(failBar, totalNotSent);

                        if (jsonData.result.Contains("Duplicate"))
                            ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET is_sync = 'duplicate' WHERE id = " + entityId);
                        else
                            ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET is_sync = 'fail' WHERE id = " + entityId);
                    }
                    else
                    {
                        // update successfully sync is_sync to submitted
                        ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET is_sync = 'yes' WHERE id = " + entityId);

                        totalSent++;
                        ToogleStatusBar(successBar, totalSent);
                    }
                }
            }

            yield return new WaitForSeconds(1.2f);
        }

        #endregion WebRequest

        ToogleHandler(blockDataHandler, false);
        if (hasSync) failBar.GetComponent<StatusBar>().Finish();
        if (hasSync) successBar.GetComponent<StatusBar>().Finish();
        SyncEnded();
    }

    [Button(ButtonSizes.Small)]
    public void SyncToServerByBatch()
    {
        StartCoroutine(SyncToServerBatch());
    }

    private IEnumerator SyncToServerBatch()
    {
        yield return StartCoroutine(NetworkExtension.CheckForInternetConnectionRoutine());

        // if no internet connection, end this coroutine, call SyncEnded() to start over again
        if (NetworkExtension.internet == false)
        {
            //No connection
            Debug.Log(name + " No internet. Stop SyncToServer() coroutine");
            SyncEnded();
            yield break;
        }

        if (OnSyncStart.GetPersistentEventCount() > 0) OnSyncStart.Invoke();

        yield return StartCoroutine(CompareServerData());

        // if no data to sync, end this coroutine, call SyncEnded() to start over again
        rows = GetAllCustomCondition();
        if (rows.Count < 1)
        {
            SyncEnded();
            StopAllCoroutines();
            Debug.Log(name + " no data to sync. Stop SyncToServer() coroutine");
            yield break;
        }

        Debug.Log(name + " : Start sync");

        // Get global event_code
        string source_identifier_code = JSONExtension.LoadSetting(dbSettings.folderPath + "\\Settings\\Setting", "source_identifier_code");

        // start writing json , add { to start of string
        string jsonString = "{\"" + dbSettings.sendAPI + "\":[";

        for (int u = 0; u < rows.Count; u++)
        {
            entityId = int.Parse(rows[u]["id"].ToString());

            jsonString += "{";

            // select the column that will be sync
            List<TableColumn> syncColumn = dbSettings.columns.FindAll(x => x.sync == true);
            for (int i = 0; i < syncColumn.Count; i++)
            {
                string value = rows[u][syncColumn[i].name].ToString();

                jsonString += string.Format("\"{0}\" : \"{1}\",", new System.Object[] { syncColumn[i].name, value });
            }

            // remove "," at the end of string
            jsonString = jsonString.Remove(jsonString.Length - 1, 1);

            jsonString += "},";

            #region WebRequest

            //using (UnityWebRequest www = UnityWebRequest.Post((dbSettings.sendURL + dbSettings.sendAPI).Replace(" ", string.Empty), form))
            //{
            //    yield return www.SendWebRequest();

            //    if (www.isNetworkError || www.isHttpError)
            //    {
            //        ErrorAction(www, "server error \n Values : " + values);
            //        totalNotSent++;
            //        ToogleStatusBar(failBar, totalNotSent);
            //        ToogleHandler(errorHandler, true);
            //        ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET is_sync = 'fail' WHERE id = " + entityId);
            //        //  SyncEnded();
            //        //  yield break;
            //    }
            //    else
            //    {
            //        //  yield return new WaitForEndOfFrame();
            //        var jsonData = JsonUtility.FromJson<JSONResponse>(www.downloadHandler.text);

            //        if (jsonData.result != "Success")
            //        {
            //            totalNotSent++;
            //            ToogleStatusBar(failBar, totalNotSent);

            //            if (jsonData.result.Contains("Duplicate"))
            //                ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET is_sync = 'duplicate' WHERE id = " + entityId);
            //            else
            //                ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET is_sync = 'fail' WHERE id = " + entityId);
            //        }
            //        else
            //        {
            //            // update successfully sync is_sync to submitted
            //            ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET is_sync = 'yes' WHERE id = " + entityId);

            //            totalSent++;
            //            ToogleStatusBar(successBar, totalSent);
            //        }
            //    }
            //}
        }

        // end writing json, remove "," at the end of string
        jsonString = jsonString.Remove(jsonString.Length - 1, 1);

        // add } to end of string
        jsonString += "]}";
        Debug.Log(name + " - batch json data to sync : " + jsonString);

        #endregion WebRequest

        if (hasSync) successBar.GetComponent<StatusBar>().Finish();
        SyncEnded();
    }

    private void SyncEnded()
    {
        rows.Clear();
        Close();
        if (OnSyncEnd.GetPersistentEventCount() > 0) OnSyncEnd.Invoke();
    }

    /// <summary>
    /// Show error message and handler when encounter error sending data to server
    /// </summary>
    /// <param name="www"></param>
    /// <param name="errorMessage"></param>
    private void ErrorAction(UnityWebRequest www, string errorMessage)
    {
        ToogleHandler(blockDataHandler, false);
        ToogleHandler(failBar, true);

        Debug.LogError(name + " :\n" + errorMessage + "\n" + www.error + "\n" + " server url: " + dbSettings.sendURL + dbSettings.sendAPI);
    }

    [FoldoutGroup("UWP APP Communication")]
    [Button(ButtonSizes.Medium)]
    public void SaveToDBFromFile()
    {
        if (Saved)
            return;

        Saved = true;

        string appLaunchFilePath = Path.Combine(dbSettings.folderPath, "AppLaunchNumberFilePath.txt");

        // Get data file path from AppLaunchNumberFilePath.txt
        string[] dataFilePath = File.ReadAllLines(appLaunchFilePath);

        // Read the text lines from the selected file path
        string[] datas = File.ReadAllLines(dataFilePath[dataFilePathIndex]);

        Debug.Log(name + " - data from file " + dataFilePath[dataFilePathIndex] + "\n" + string.Join("\n", datas));

        List<string> col = new List<string>();
        List<string> val = new List<string>();

        for (int v = 1; v < dbSettings.columns.Count; v++)
        {
            col.Add(dbSettings.columns[v].name);
        }

        val.AddRange(col);

        for (int i = 0; i < col.Count; i++)
        {
            val[i] = datas[i];
        }

        AddData(col, val);

        if (OnSaveToLocal.GetPersistentEventCount() > 0) OnSaveToLocal.Invoke();
    }
}