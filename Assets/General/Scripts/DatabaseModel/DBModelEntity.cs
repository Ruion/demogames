﻿using System.Collections.Generic;
using UnityEngine;
using System.Data;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;

public class DBModelEntity : DBModelMaster
{
    DataRowCollection rows;

    public override void SaveToLocal() {
        base.SaveToLocal();

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
        Debug.Log(gameObject.name + " Data save to local");
    }

    public override void Sync()
    {
        LoadSetting();

        base.Sync();
        StartCoroutine(SyncToServer());
    }

    private IEnumerator SyncToServer()
    {
        rows = GetAllCustomCondition();

        if(rows.Count < 1) { Debug.Log("no data to be sync"); StopAllCoroutines(); yield break; }

        Debug.Log("unsync data : " + rows.Count); int totalSent = 0;

        #region WWWForm & UnityWebRequest send data
        Debug.Log("Start sync");

        for (int u = 0; u < rows.Count; u++)
        {
            WWWForm form = new WWWForm();

            // Debug.Log("field to sent : " + dbSettings.columnsToSync.Count);
            string values = "";

            for (int i = 0; i < dbSettings.columns.Count; i++)
            {
                if (!dbSettings.columns[i].sync) continue;

                string value = rows[u][dbSettings.columns[i].name].ToString();

                values += value + " | ";

                form.AddField(
                    dbSettings.columns[i].name,
                   value);

            }

            Debug.Log(values);

            using (UnityWebRequest www = UnityWebRequest.Post(dbSettings.sendURL, form))
            {

                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {

                    Debug.LogError("try sync but server fail");
                    Debug.LogError(www.error + "\n"+ "send URL " + dbSettings.sendURL);
                    StopAllCoroutines();

                    yield break;
                }
                else
                {
                    //  yield return new WaitForEndOfFrame();
                    var jsonData = JsonUtility.FromJson<JSONResponse>(www.downloadHandler.text);

                    Debug.Log(www.downloadHandler.text);

                    if (jsonData.result != dbSettings.serverResponsesArray[0].resultResponse)
                    {
                        #region Server Response and Message Feedback

                        //  string response = dbSettings.serverResponses.resultResponses.FirstOrDefault(r => r == jsonData.result);
                        ServerResponses response = dbSettings.serverResponsesArray.FirstOrDefault(r => r.resultResponse == jsonData.result);
                        int index = System.Array.IndexOf(dbSettings.serverResponsesArray, response);
                        Debug.Log(index);
                        Debug.LogError(response.resultResponseMessage);

                        #endregion

                        StopAllCoroutines();

                        // show red bar on fail
                        yield break;
                    }

                    Debug.Log(dbSettings.serverResponsesArray[0].resultResponseMessage);
                    UpdateSyncData(rows[0]["id"].ToString());
                    totalSent++;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
        #endregion

        Debug.Log("total sent : " + totalSent);
        rows.Clear();
        Close();
    }
}
