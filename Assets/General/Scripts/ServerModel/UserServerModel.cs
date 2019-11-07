﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using DataBank;
using TMPro;
using System.Data;
using System.Reflection;

[RequireComponent(typeof(UniversalUserDB))]
public class UserServerModel : ServerModelMaster
{

    #region fields
    [Header("Handler")]
    public Text sentText;
    private int totalSent;
    public GameObject emptyHandler;
    public GameObject internetErrorHandler;
    public GameObject errorHandler;
    public TextMeshProUGUI errorCodeText;
    public GameObject loadingHandler;
    public GameObject successSendDataHandler;
    public GameObject blockDataHandler;

    private UniversalUserDB udb;
    #endregion

    List<UniversalUserEntity> unSyncUsers = new List<UniversalUserEntity>();

    public List<string> localEmailList = new List<string>();
    public List<string> emailList = new List<string>();

    private OnlineServerModel osm;

    private void Start()
    {
        HideAllHandler();

        SetUpDb();

        localEmailList = udb.GetAllUserEmail();

        DoGetDataFromServer();

        udb.Close();
    }

    private void SetUpDb()
    {
        if (udb == null) udb = GetComponent<UniversalUserDB>();
        udb.ConnectDbCustom();
    }

    [ContextMenu("Get Existing Email")]
    public void GetExistingEmail()
    {
        SetUpDb();
        localEmailList = new List<string>();
        localEmailList = udb.GetAllUserEmail();

        udb.Close();
    }

    [ContextMenu("HideHandler")]
    public void HideAllHandler()
    {
        emptyHandler.SetActive(false);
        internetErrorHandler.SetActive(false);
        errorHandler.SetActive(false);
        loadingHandler.SetActive(false);
        successSendDataHandler.SetActive(false);
        blockDataHandler.SetActive(false);
    }

    public void ClearData()
    {
        SetUpDb();
        udb.DeleteAllData();
        udb.Close();
    }

    #region Save Data
    public override void SaveToLocal()
    {
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

    #region Sync Data
    [ContextMenu("Sync")]
    public void SendDataToDatabase()
    {
        StartCoroutine(DataToSend());
    }

    IEnumerator DataToSend()
    {
        HideAllHandler();

        blockDataHandler.SetActive(true);

        string HtmlText = GetHtmlFromUri("http://google.com");
        if (HtmlText == "")
        {
            //No connection
            internetErrorHandler.SetActive(true);
            yield break;
        }
        

        // Get unSync user
        unSyncUsers = new List<UniversalUserEntity>();

        SetUpDb();
        unSyncUsers = udb.GetAllUnSyncUser();

        CompareLocalAndServerData();

        Debug.Log("unsync users : " + unSyncUsers.Count);

        if (unSyncUsers == null || unSyncUsers.Count < 1)
        {
            HideAllHandler();
            emptyHandler.SetActive(true);
            yield break;
        }

        
        totalSent = 0;

        List<string> colToSend = new List<string>();
        colToSend.AddRange(gameSettings.sQliteDBSettings.columns);

        for (int i = 0; i < gameSettings.sQliteDBSettings.columnsToSkipWhenSync.Count; i++)
        {
            colToSend.Remove(gameSettings.sQliteDBSettings.columnsToSkipWhenSync[i]);
        }

        for (int u = 0; u < unSyncUsers.Count; u++)
        {
            WWWForm form = new WWWForm();

            for (int i = 0; i < colToSend.Count; i++)
            {
                Debug.Log(colToSend[i] + " : " + unSyncUsers[u].GetType()
                    .GetField(colToSend[i])
                    .GetValue(unSyncUsers[u])
                    .ToString()); 

                form.AddField(colToSend[i],
                    unSyncUsers[u].GetType()
                    .GetField(colToSend[i])
                    .GetValue(unSyncUsers[u])
                    .ToString());
            }           

            using (UnityWebRequest www = UnityWebRequest.Post(gameSettings.serverAddress, form))
            {

                loadingHandler.SetActive(true);
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    errorHandler.SetActive(true);
                    errorCodeText.text = www.error;

                    blockDataHandler.SetActive(false);

                    StopAllCoroutines();
                    yield break;
                }
                else
                {
                    yield return new WaitForEndOfFrame();
                    var jsonData = JsonUtility.FromJson<JSONResponse>(www.downloadHandler.text);

                    Debug.Log(www.downloadHandler.text);

                    if (jsonData.result != "success")
                    {

                        HideAllHandler();
                        errorHandler.SetActive(true);
                        errorCodeText.text = "Send but fail " + www.error;

                        blockDataHandler.SetActive(false);

                        StopAllCoroutines();
                        yield break;
                    }

                    successSendDataHandler.GetComponentInChildren<TextMeshProUGUI>().text = jsonData.result;

                    totalSent++;
                    sentText.text = totalSent.ToString();

                    udb.UpdateSyncUser(unSyncUsers[u]);

                    successSendDataHandler.SetActive(true);
                }
            }

            yield return new WaitForSeconds(0.2f);
        }

        udb.Close();

        blockDataHandler.SetActive(false);
    }

    #endregion

    [ContextMenu("ShowAll")]
    public void ShowAll()
    {
        SetUpDb();
        IDataReader reader = udb.GetAllData();
        while (reader.Read())
        {
            string text = "";
            for (int i = 0; i < reader.FieldCount; i++)
            {
                text += reader[i] + " ";
            }
            Debug.Log(text);
        }
    }

    #region Get Server Data
    [ContextMenu("GetServerData")]
    public void DoGetDataFromServer()
    {
        StartCoroutine(GetDataFromServer());
    }

    
    // to be configure
    IEnumerator GetDataFromServer()
    {
        LoadGameSettingFromMaster();

        osm = FindObjectOfType<OnlineServerModel>();

        yield return StartCoroutine(osm.FeedEmail(emailList));

        emailList = new List<string>();
        emailList.AddRange(osm.emailList);

        if (emailList.Count < 1) { Debug.Log("no server user"); yield break; }

        for (int i = 0; i < emailList.Count; i++)
        {
            // add user never exist in local
            AddUniqueUser(emailList[i], localEmailList);
        }

    }
    

    [ContextMenu("CompareLocalAndServerData")]
    public void CompareLocalAndServerData()
    {
        UniversalUserEntity u = new UniversalUserEntity();
        FieldInfo[] fields = u.GetType().GetFields();

        foreach (string m in emailList)
        {
            Debug.Log("Comparing Email");

            UniversalUserEntity foundUser = FindDuplicatedStringItem(m, unSyncUsers);

            if (foundUser != null)
            {
                SetUpDb();
                if(udb.GetDataByString("email", foundUser.email) != null) udb.UpdateSyncUser(foundUser);
                unSyncUsers.Remove(foundUser);
                Debug.Log(foundUser.email + " is removed from sync");
            }
        }
        udb.Close();
    }

    [ContextMenu("Get UnSync email")]
    public void GetUnSyncEmail()
    {
        SetUpDb();
        IDataReader reader = udb.GetDataByString("is_submitted", "false");
        while (reader.Read())
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                Debug.Log(reader[i]);
            }
            
        }
        udb.Close();
    }

    #endregion

}