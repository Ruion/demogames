using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using DataBank;
using TMPro;
using System.Linq;

public class DataManager : GameSettingEntity {

    #region fields
    [Header("Fields")]
    public InputField nameInput;
    public InputField emailInput;
    public InputField phoneInput;
    
    public int numberToPopulate = 100;

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

    public List<UserEntity> oldUser;

    public List<UserEntity> serverUsers = new List<UserEntity>();

    private List<UserEntity> unSyncUsers = new List<UserEntity>();
    private OnlineServerModel osm;

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
    }

    public void SaveScoreToLocal()
    {
        SetUpDb();

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
            localScoreNotSaveHandler.SetActive(true);
            localNotSaveCodeText.text = msg;
        }

    }

    void SavePlayerTemporary(string key, string data)
    {
        PlayerPrefs.SetString(key, data);
    }

    public void Populate()
    {
        SetUpDb();

        for (int i = 0; i < numberToPopulate; i++)
        {
            UserEntity user = new UserEntity();
            user.name = "p" + i.ToString();
            user.email = "p" + i.ToString() + "@gmail.com";
            user.phone = "01" + i.ToString() + "2244213";
            user.score = i.ToString();
            user.is_submitted = "false";

            userDb.AddData(user);
        }

        userDb.Close();
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

        blockDataHandler.SetActive(true);

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
            HideAllHandler();
            errorHandler.SetActive(true);
            yield break;
        }

        if (unSyncUsers == null || unSyncUsers.Count < 1)
        {
            HideAllHandler();
            emptyHandler.SetActive(true);
            yield break;
        }

        totalSent = 0;

      //  StartCoroutine(CompareLocalAndServerData());

        

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

       yield return StartCoroutine(osm.FeedUsers(serverUsers));

        if (osm.serverUsers.Count < 1) Debug.Log("no server user");

     //   serverUsers.Clear(); // clear list
     //  serverUsers.AddRange(osm.serverUsers); // add server users into list

       for (int i = 0; i < osm.serverUsers.Count; i++)
       {
            // add user never exist in local
            AddUniqueUser(osm.serverUsers[i], oldUser);
       }

        Debug.Log("Current old users");

        foreach (UserEntity u in oldUser)
        {
            Debug.Log(u.email);
        }

    }

    public void AddUniqueUser(UserEntity user, List<UserEntity> users)
    {

        UserEntity foundUser = users.FirstOrDefault(i => i.email == user.email);
        if (foundUser == null)
        {
            users.Add(user);
        }
        else
            Debug.Log("email : " + user.email + "already existed");
    }

}

[System.Serializable]
public class JSONResponse
{
    public string result;
    public UserEntity[] users;
}
