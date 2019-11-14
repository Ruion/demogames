using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using DataBank;
using TMPro;
using System.Data;

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
    public GameObject successBar;
    public TextMeshProUGUI successText;
    public GameObject failBar;

    private UniversalUserDB udb;
    #endregion

    List<DataBank.UniversalUserEntity> unSyncUsers = new List<DataBank.UniversalUserEntity>();

    public List<string> localEmailList = new List<string>();
    public List<string> emailList = new List<string>();

    private OnlineServerModel osm;

    private void Start(){}

    public void StartUp()
    {
        HideAllHandler();

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
        successBar.SetActive(false);
        failBar.SetActive(false);
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

    #endregion

    #region Sync Data
    [ContextMenu("Sync")]
    public void SendDataToDatabase()
    {
        LoadGameSettingFromMaster();

        HideAllHandler();
        GetUnSyncData(false);
        StartCoroutine(DataToSend());
    }

    [ContextMenu("AutoSync")]
    public void AutoSendDataToDatabase()
    {
        LoadGameSettingFromMaster();

        HideAllHandler();
        GetUnSyncData(true);
        StartCoroutine(DataToSend());
    }

    private void GetUnSyncData(bool limit = false)
    {
        // Get unSync user
        unSyncUsers = new List<UniversalUserEntity>();

        SetUpDb();
        
        if (limit) unSyncUsers = udb.GetAllUnSyncUserLimit();
        else unSyncUsers = udb.GetAllUnSyncUser();

        if (unSyncUsers == null || unSyncUsers.Count < 1)
        {
            Debug.Log("unsync users : " + unSyncUsers.Count);
            HideAllHandler();
            emptyHandler.SetActive(true);
            return;
        }
    }

    IEnumerator DataToSend()
    { 
        blockDataHandler.SetActive(true);

        #region Check Internet
        ///////////// CHECK Internet Connection /////////////
        string HtmlText = GetHtmlFromUri("http://google.com");
        if (HtmlText == "")
        {
            //No connection
            internetErrorHandler.SetActive(true);
            yield break;
        }
        #endregion

        #region Compare with online server

        // CompareLocalAndServerData // Temporary Disable 
        /*   yield return StartCoroutine(CompareLocalAndServerData());

           if (unSyncUsers == null || unSyncUsers.Count < 1)
           {
               Debug.Log("unsync users : " + unSyncUsers.Count);
               HideAllHandler();
               emptyHandler.SetActive(true);
               yield break;
           }
           */
        #endregion

        Debug.Log("unsync users : " + unSyncUsers.Count);

        totalSent = 0;

        #region using Reflection to send data but DISABLED FOR VERY SLOW 
        /*
        List<string> colToSend = new List<string>();
        colToSend.AddRange(gameSettings.sQliteDBSettings.columns);

        for (int i = 0; i < gameSettings.sQliteDBSettings.columnsToSkipWhenSync.Count; i++)
        {
            colToSend.Remove(gameSettings.sQliteDBSettings.columnsToSkipWhenSync[i]);
        }
        */
        #endregion

        #region WWWForm & UnityWebRequest send data
        Debug.Log("Start sync");

        for (int u = 0; u < unSyncUsers.Count; u++)
        {
            WWWForm form = new WWWForm();


            form.AddField("name", unSyncUsers[u].name);
            form.AddField("email", unSyncUsers[u].email);
            form.AddField("contact", unSyncUsers[u].contact);

            form.AddField("age", "0");
            form.AddField("dob", "0000-00-00");
            form.AddField("gender", "0");

            form.AddField("game_result", unSyncUsers[u].game_result);
            form.AddField("game_score", unSyncUsers[u].game_score);
            form.AddField("created_at", unSyncUsers[u].created_at);

            Debug.Log(unSyncUsers[u].created_at);

            #region Reflection to send object data but SLOW
            /*  for (int i = 0; i < colToSend.Count; i++)
              {

                  form.AddField(colToSend[i],
                      unSyncUsers[u].GetType()
                      .GetField(colToSend[i])
                      .GetValue(unSyncUsers[u])
                      .ToString());
              }
          */
            #endregion

            using (UnityWebRequest www = UnityWebRequest.Post(gameSettings.serverAddress, form))
            {

                loadingHandler.SetActive(true);
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    errorHandler.SetActive(true);
                    errorCodeText.text = www.error;

                    blockDataHandler.SetActive(false);
                    Debug.LogError("try sync but server fail");
                    Debug.LogError(www.error);
                    StopAllCoroutines();

                    // show red bar on fail
                    failBar.SetActive(true); failBar.GetComponent<StatusBar>().Finish();

                    yield break;
                }
                else
                {
                  //  yield return new WaitForEndOfFrame();
                    var jsonData = JsonUtility.FromJson<JSONResponse>(www.downloadHandler.text);

                    Debug.Log(www.downloadHandler.text);

                    if (jsonData.result != "Success")
                    {

                        HideAllHandler();
                        errorHandler.SetActive(true);
                        errorCodeText.text = "Send but fail " + www.error;

                        blockDataHandler.SetActive(false);

                        StopAllCoroutines();
                        Debug.LogError("try sync but fail");

                        // show red bar on fail
                        failBar.SetActive(true); failBar.GetComponent<StatusBar>().Finish();

                        yield break;
                    }

                    successSendDataHandler.GetComponentInChildren<TextMeshProUGUI>().text = jsonData.result;

                    totalSent++;
                    sentText.text = totalSent.ToString();
                    successText.text = sentText.text;
                    successBar.SetActive(true);

                    udb.UpdateSyncUser(unSyncUsers[u]);
                    successSendDataHandler.SetActive(true);
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
        #endregion

        udb.Close();
        successBar.GetComponent<StatusBar>().Finish();
        failBar.GetComponent<StatusBar>().Finish();

        blockDataHandler.SetActive(false);
    }

    #endregion

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
    
    private IEnumerator CompareLocalAndServerData()
    {
        SetUpDb();

        DataBank.UniversalUserEntity u = new DataBank.UniversalUserEntity();

        for (int m = 0; m < emailList.Count; m++)
        {
            UniversalUserEntity foundUser = FindDuplicatedStringItem(emailList[m], unSyncUsers);
            if (foundUser != null)
            {
               // SetUpDb();
              //  if (udb.GetDataByString("email", foundUser.email) != null) udb.UpdateSyncUser(foundUser);

                yield return new WaitForEndOfFrame();
                unSyncUsers.Remove(foundUser);
                
                Debug.Log(foundUser.email + " is removed from sync");
            }
        }

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
