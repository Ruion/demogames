using System.Collections.Generic;
using UnityEngine;
using System.Data;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;

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
        yield return StartCoroutine(CompareServerData());

        rows = GetAllCustomCondition();

        if(rows.Count < 1) { Debug.Log("no data to be sync"); StopAllCoroutines(); yield break; }

        Debug.Log("unsync data : " + rows.Count); int totalSent = 0; int totalNotSent = 0;

        Debug.Log("Start sync");
        ToogleHandler(blockDataHandler, true);

       
        for (int u = 0; u < rows.Count; u++)
        {
            #region WWW Form
            WWWForm form = new WWWForm();

            // Debug.Log("field to sent : " + dbSettings.columnsToSync.Count);
            // string values = "";
            entityId = int.Parse(rows[u]["id"].ToString());

            for (int i = 0; i < dbSettings.columns.Count; i++)
            {
                if (!dbSettings.columns[i].sync) continue;

                string value = rows[u][dbSettings.columns[i].name].ToString();

               // values += value + " | ";

                form.AddField(
                    dbSettings.columns[i].name,
                   value);
            }
            // Debug.Log(values);
            #endregion

            #region WebRequest
            using (UnityWebRequest www = UnityWebRequest.Post(dbSettings.sendURL, form))
            {

                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    ErrorAction(www, "server error");
                    totalNotSent++;
                    ToogleStatusBar(failBar, totalNotSent);
                }
                else
                {
                    //  yield return new WaitForEndOfFrame();
                    var jsonData = JsonUtility.FromJson<JSONResponse>(www.downloadHandler.text);

                    if (jsonData.result != dbSettings.serverResponsesArray[0].resultResponse)
                    {
                        //  string response = dbSettings.serverResponses.resultResponses.FirstOrDefault(r => r == jsonData.result);
                        ServerResponses response = dbSettings.serverResponsesArray.FirstOrDefault(r => r.resultResponse == jsonData.result);
                        int index = System.Array.IndexOf(dbSettings.serverResponsesArray, response);
                        Debug.Log(index);
                        Debug.LogError(response.resultResponseMessage);

                        totalNotSent++;
                        ToogleStatusBar(failBar, totalNotSent);
                    }
                    else
                    {
                        Debug.Log(dbSettings.serverResponsesArray[0].resultResponseMessage);

                        // update successfully sync online_status to submitted
                        ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET online_status = 'submitted' WHERE id = " + entityId);

                        totalSent++;
                        ToogleStatusBar(successBar, totalSent);
                    }
                }
            }

            yield return new WaitForSeconds(1f);
        }
        #endregion

        ToogleHandler(blockDataHandler, false);
        failBar.GetComponent<StatusBar>().Finish();
        successBar.GetComponent<StatusBar>().Finish();
        rows.Clear();
        Close();
    }

    private void ErrorAction(UnityWebRequest www, string errorMessage)
    {
        ToogleHandler(blockDataHandler, false);
        ToogleHandler(failBar, true);

        Debug.LogError(errorMessage + "\n" + www.error + "\n" + " server url: " + dbSettings.sendURL);
    }


}

