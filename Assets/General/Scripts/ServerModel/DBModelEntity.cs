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
        col.AddRange(dbSettings.localDbSetting.columns);
        col.RemoveAt(0);

        List<string> val = new List<string>();

        for (int i = 0; i < col.Count; i++)
        {
            val.Add(PlayerPrefs.GetString(col[i]));
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
        rows = GetAllUnSync();

        if(rows.Count < 1) { Debug.Log("no data to be sync"); StopAllCoroutines(); yield break; }

        Debug.Log("unsync data : " + rows.Count); int totalSent = 0;

        #region WWWForm & UnityWebRequest send data
        Debug.Log("Start sync");

        for (int u = 0; u < rows.Count; u++)
        {
            WWWForm form = new WWWForm();

           // Debug.Log("field to sent : " + dbSettings.localDbSetting.columnsToSync.Count);

            for (int i = 0; i < dbSettings.localDbSetting.columnsToSync.Count; i++)
            {
                string value = rows[u][dbSettings.localDbSetting.columnsToSync[i]].ToString();
                Debug.Log(dbSettings.localDbSetting.columnsToSync[i] + " : " + value);

                form.AddField(
                    dbSettings.localDbSetting.columnsToSync[i],
                   value);

            }

            Debug.Log(dbSettings.sendURL);
            using (UnityWebRequest www = UnityWebRequest.Post(dbSettings.sendURL, form))
            {

                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {

                    Debug.LogError("try sync but server fail");
                    Debug.LogError(www.error);
                    StopAllCoroutines();

                    yield break;
                }
                else
                {
                    //  yield return new WaitForEndOfFrame();
                    var jsonData = JsonUtility.FromJson<JSONResponse>(www.downloadHandler.text);

                    Debug.Log(www.downloadHandler.text);

                    if (jsonData.result != dbSettings.serverResponses.resultResponses[0])
                    {
                        #region Server Response and Message Feedback

                        string response = dbSettings.serverResponses.resultResponses.FirstOrDefault(r => r == jsonData.result);
                        int index = System.Array.IndexOf(dbSettings.serverResponses.resultResponses, response);
                        Debug.Log(index);
                        Debug.LogError(dbSettings.serverResponses.resultResponsesMessage[index]);

                        #endregion

                        StopAllCoroutines();

                        // show red bar on fail
                        yield break;
                    }

                    Debug.Log(dbSettings.serverResponses.resultResponsesMessage[0]);
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
