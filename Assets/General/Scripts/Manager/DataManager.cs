using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Networking;
using DataBank;
using TMPro;

public class DataManager : GameSettingEntity {

    #region fields
    [Header("Fields")]
    public InputField nameInput;
    public InputField emailInput;
    public InputField phoneInput;

    //public string[] columns;

    public List<UserEntity> oldUser;

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
    public GameObject localNotSaveHandler;
    public GameObject localScoreNotSaveHandler;
    public TextMeshProUGUI localNotSaveCodeText;

    private UserDB userDb;
    #endregion

    public UnityEvent OnErrorMsgShown;
    public UnityEvent OnSyncOperationEnded;

    private List<UserEntity> serverUsers = new List<UserEntity>();
    List<UserEntity> unSyncUsers = new List<UserEntity>();

    private void Start()
    {
        HideAllHandler();

        oldUser = new List<UserEntity>();
        UserDB userDb = new UserDB(gameSettings.dbName, gameSettings.tableName);
        oldUser = userDb.GetAllUser();
        userDb.Close();
    }

    private void SetUpDb()
    {
        userDb = new UserDB(gameSettings.dbName, gameSettings.tableName);
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

    public void GetAllPlayer()
    {
        LoadSetting();
        SetUpDb();

        List<UserEntity> entities = userDb.GetAllUser();
        userDb.Close();

        if (entities.Count < 1)
        {
            Debug.LogError("no data in record");
            return;
        }

        int n = 0;
        foreach (UserEntity e in entities)
        {
            n++;
            Debug.Log(n + " name is " + e.name);
            Debug.Log(n + " email is " + e.email);
            Debug.Log(n + " phone is " + e.phone);
            Debug.Log(n + " score is " + e.score);
            Debug.Log(n + " register_datetime is " + e.register_datetime);
        }
    }

    public void ClearData()
    {
        SetUpDb();
        userDb.DeleteAllData();
        userDb.Close();
    }

    #region Save Data
    public void SaveToLocal()
    {
        SavePlayerTemporary("name", nameInput.text.ToString());
        SavePlayerTemporary("email", emailInput.text.ToString());
        SavePlayerTemporary("phone", phoneInput.text.ToString());
        SavePlayerTemporary("register_datetime", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        #region sqlite
        /*
        UserDB userDb = new UserDB();
        UserEntity user = new UserEntity();
        user.name = nameInput.text.ToString();
        user.email = emailInput.text.ToString();
        user.phone = phoneInput.text.ToString();
        user.register_datetime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        user.is_sync = "N";
        

        bool added = userDb.AddData(user);

        userDb.Close();

        if (!added)
        {
            localNotSaveHandler.SetActive(true);
        }
        */
        #endregion
    }

    public void SaveScoreToLocal()
    {
        #region old
        /*
        UserDB userDb = new UserDB();

        bool updated = userDb.UpdateLastDataScore(PlayerPrefs.GetString("score"));

        if (!updated)
        {
            localScoreNotSaveHandler.SetActive(true);
        }

        string msg = userDb.AddData(user);

        userDb.Close();

        if (msg != "true")
        {
            OnErrorMsgShown.Invoke();
            localScoreNotSaveHandler.SetActive(true);
            localNotSaveCodeText.text = msg;
        }
        */
        #endregion


        SetUpDb();

        /*
        UserEntity user = new UserEntity();
        user.name = PlayerPrefs.GetString("name");
        user.email = PlayerPrefs.GetString("email");
        user.phone = PlayerPrefs.GetString("phone");
        user.score = PlayerPrefs.GetString(scoreName);
        user.register_datetime = PlayerPrefs.GetString("register_datetime");
        user.is_submitted = "N";
        */

        List<string> col = new List<string>();
        List<string> val = new List<string>();

        col.Add("name");
        col.Add("email");
        col.Add("phone");
        col.Add("score");
        col.Add("register_datetime");
        col.Add("is_submitted");

        val.Add(PlayerPrefs.GetString("name"));
        val.Add(PlayerPrefs.GetString("email"));
        val.Add(PlayerPrefs.GetString("phone"));
        val.Add(PlayerPrefs.GetString(gameSettings.scoreName));
        val.Add(PlayerPrefs.GetString("register_datetime"));
        val.Add("N");

        // string msg = userDb.AddData(user);
        string msg = userDb.AddData(col, val);

        userDb.Close();

        if (msg != "true")
        {
            OnErrorMsgShown.Invoke();
            localScoreNotSaveHandler.SetActive(true);
            localNotSaveCodeText.text = msg;
        }

    }

    void SavePlayerTemporary(string key, string data)
    {
        PlayerPrefs.SetString(key, data);
    }
    #endregion

    #region validation
    public string CheckSameUserInput()
    {
        string type = "";

        for (int i = 0; i < oldUser.Count; i++)
        {
            // if the email & phone is same
            if (emailInput.text == oldUser[i].email)
            {
                type = "email";
            }
            if (phoneInput.text == oldUser[i].phone)
            {
                type = "phone";
            }
        }

        return type;
    }

    public bool CheckSameUserEmail()
    {
        bool emailIsSame = false;

        for (int i = 0; i < oldUser.Count; i++)
        {
            // if the email & phone is same
            if (emailInput.text == oldUser[i].email)
            {
                emailIsSame = true;
            }
        }

        return emailIsSame;
    }

    public bool CheckSameUserPhone()
    {
        bool phoneIsSame = false;

        for (int i = 0; i < oldUser.Count; i++)
        {
            // if the email & phone is same
            if (phoneInput.text == oldUser[i].phone)
            {
                phoneIsSame = true;
            }
        }

        return phoneIsSame;
    }
    #endregion

    public void SendDataToDatabase()
    {
        StartCoroutine(DataToSend());
    }

    IEnumerator DataToSend()
    {
        HideAllHandler();

        string HtmlText = GetHtmlFromUri("http://google.com");
        if (HtmlText == "")
        {
            //No connection
            internetErrorHandler.SetActive(true);
            yield break;
        }

        // Get unSync user
        SetUpDb();

        List<UserEntity> unSyncUsers = new List<UserEntity>();
        try {
            unSyncUsers = userDb.GetAllUnSyncUser();
          //  StartCoroutine(CompareLocalAndServerData());
          //  Debug.Log("unsync users : " + unSyncUsers.Count);
        }
        catch {
            errorHandler.SetActive(true);
            yield break;
        }

        if (unSyncUsers == null || unSyncUsers.Count < 1)
        {
            emptyHandler.SetActive(true);
            yield break;
        }

        totalSent = 0;

      //  StartCoroutine(CompareLocalAndServerData());

        blockDataHandler.SetActive(true);

        for (int i = 0; i < unSyncUsers.Count; i++)
        {
            WWWForm form = new WWWForm();
            form.AddField("name", unSyncUsers[i].name);
            form.AddField("email", unSyncUsers[i].email);
            form.AddField("phone", unSyncUsers[i].phone);
            form.AddField("score", unSyncUsers[i].score);
            form.AddField("register_datetime", unSyncUsers[i].register_datetime);

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

                    userDb.UpdateSyncUser(unSyncUsers[i]);

                    successSendDataHandler.SetActive(true);
                }
            }

            yield return new WaitForSeconds(0.2f);
        }

        userDb.Close();

        blockDataHandler.SetActive(false);
        OnSyncOperationEnded.Invoke();
    }

    // function for check internet connection
    public string GetHtmlFromUri(string resource)
    {
        string html = string.Empty;
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(resource);
        try
        {
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
                if (isSuccess)
                {
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        //We are limiting the array to 80 so we don't have
                        //to parse the entire html document feel free to 
                        //adjust (probably stay under 300)
                        char[] cs = new char[80];
                        reader.Read(cs, 0, cs.Length);
                        foreach (char ch in cs)
                        {
                            html += ch;
                        }
                    }
                }
            }
        }
        catch
        {
            return "";
        }
        return html;
    }

    public void GetDataFromServerFunction()
    {
        serverUsers = new List<UserEntity>();

        string HtmlText = GetHtmlFromUri("http://google.com");
        if (HtmlText == "")
        {
            //No connection
            internetErrorHandler.SetActive(true);
        }
        else
        {
            // fetch server data to serverUser
            WWWForm form = new WWWForm();
            //form.AddField("licensekey", value);

            using (UnityWebRequest www = UnityWebRequest.Post(gameSettings.serverGetDataAddress, form))
            {

                loadingHandler.SetActive(true);
                www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    errorHandler.SetActive(true);
                    errorCodeText.text = www.error;

                    blockDataHandler.SetActive(false);

                }
                else
                {
                    var jsonData = JsonUtility.FromJson<JSONResponse>(www.downloadHandler.text);

                    if (jsonData.result != "success")
                    {
                        HideAllHandler();
                        errorHandler.SetActive(true);
                        errorCodeText.text = "request but fail " + www.error;

                        blockDataHandler.SetActive(false);
                    }

                    foreach (UserEntity u in jsonData.users)
                    {
                        serverUsers.Add(u);
                        Debug.Log("user " + u.name + "is fetched from server");
                    }

                }
            }
        }
    }
    // to be configure
    IEnumerator GetDataFromServer()
    {
        serverUsers = new List<UserEntity>();

        string HtmlText = GetHtmlFromUri("http://google.com");
        if (HtmlText == "")
        {
            //No connection
            internetErrorHandler.SetActive(true);
            yield break;
        }
        else
        {
            // fetch server data to serverUser
            WWWForm form = new WWWForm();
            //form.AddField("licensekey", value);

            using (UnityWebRequest www = UnityWebRequest.Post(gameSettings.serverGetDataAddress, form))
            {

                loadingHandler.SetActive(true);
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    errorHandler.SetActive(true);
                    errorCodeText.text = www.error;

                    blockDataHandler.SetActive(false);

                    yield return null;
                }
                else
                {
                    var jsonData = JsonUtility.FromJson<JSONResponse>(www.downloadHandler.text);

                    if (jsonData.result != "success")
                    {
                        HideAllHandler();
                        errorHandler.SetActive(true);
                        errorCodeText.text = "request but fail " + www.error;

                        blockDataHandler.SetActive(false);

                        yield return null;
                    }

                    foreach (UserEntity u in jsonData.users)
                    {
                        serverUsers.Add(u);
                        Debug.Log("user " + u.name + "is fetched from server");
                    }

                }
            }
            
        }
    }

    IEnumerator CompareLocalAndServerData()
    {
        unSyncUsers = new List<UserEntity>();
        

        yield return StartCoroutine(GetDataFromServer());

        SetUpDb();
        oldUser = userDb.GetAllUser();

        if (serverUsers.Count > 1)
        {
            foreach(UserEntity u in oldUser)
            {
                foreach(UserEntity s in serverUsers)
                {
                    if (s.email == u.email)
                    {
                        // update the old user isSubmmited
                        userDb.UpdateSyncUser(u);
                        Debug.Log("user " + u.email + "existed in server");
                    }
                }
            }

            
            
        }
        // get new unsync list from sqlite
        unSyncUsers = userDb.GetAllUnSyncUser();
        userDb.Close();
    }

}

[System.Serializable]
public class JSONResponse
{
    public string result;
    public DataBank.UserEntity[] users;
}
