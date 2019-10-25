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
    public InputField contactInput;
    
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

    public List<UserEntity> unSyncUsers = new List<UserEntity>();
    private OnlineServerModel osm;

    private void Start()
    {
        HideAllHandler();

        oldUser = new List<UserEntity>();
        SetUpDb();

      //  oldUser = userDb.GetAllUser();

        // only get email and contact for faster loading and occupy less memory
        oldUser = userDb.GetAllUserEmailAndContact();
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

       // List<UserEntity> entities = userDb.GetAllUser();
        List<UserEntity> entities = userDb.GetAllUserEmailAndContact();
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
            Debug.Log(
                n + " name : " + e.name 
                + " email : " + e.email 
                + " contact : " + e.contact 
                + " age : " + e.age 
                + " dob : " + e.dob 
                + " gender : " + e.gender 
                + " game_result : " + e.game_result 
                + " game_score : " + e.game_score 
                + " voucher_id : " + e.voucher_id 
                + " register_datetime : " + e.register_datetime);
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
        /**
        * Player data submission API
        * 
        * 2 possible results response from this player data submission api:
        * 1. Success
        * 2. Fail
        * 
        * Requires 10 parameters
        * 1. name
        * 2. email
        * 3. contact
        * 4. age
        * 5. dob
        * 6. gender
        * 7. game_result
        * 8. game_score
        * 9. voucher_name
        * 10. register_datetime
        */

         SavePlayerTemporary("name", nameInput.text.ToString());
         SavePlayerTemporary("email", emailInput.text.ToString());
         SavePlayerTemporary("contact", contactInput.text.ToString());
         SavePlayerTemporary("age", contactInput.text.ToString());
         SavePlayerTemporary("dob", contactInput.text.ToString());
         SavePlayerTemporary("gender", contactInput.text.ToString());
      //   SavePlayerTemporary("game_result", contactInput.text.ToString());
      //   SavePlayerTemporary("game_score", contactInput.text.ToString());
      //   SavePlayerTemporary("voucher_id", contactInput.text.ToString());
         SavePlayerTemporary("register_datetime", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
    }

    public void SaveScoreToLocal()
    {
     SetUpDb();

     List<string> col = new List<string>();
     List<string> val = new List<string>();

     col.Add("name");
     col.Add("email");
     col.Add("contact");
     col.Add("age");
     col.Add("dob");
     col.Add("gender");
     col.Add("game_result");
     col.Add("game_score");
     col.Add("voucher_id");
     col.Add("register_datetime");
     col.Add("is_submitted");

     val.Add(PlayerPrefs.GetString("name"));
     val.Add(PlayerPrefs.GetString("email"));
     val.Add(PlayerPrefs.GetString("contact"));
     val.Add(PlayerPrefs.GetString("age"));
     val.Add(PlayerPrefs.GetString("dob"));
     val.Add(PlayerPrefs.GetString("gender"));

        string game_result = "lose";
        if (System.Int32.Parse(PlayerPrefs.GetString(gameSettings.scoreName)) >= gameSettings.scoreToWin) game_result = "win";

     val.Add(PlayerPrefs.GetString(gameSettings.scoreName));
     val.Add(game_result); // win or lose
     val.Add(PlayerPrefs.GetString("voucher_id"));
     val.Add(PlayerPrefs.GetString("register_datetime"));
     val.Add("false");

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
        for (int i = 0; i < numberToPopulate; i++)
        {
            SetUpDb();
            UserEntity user = new UserEntity();
             user.name = "p" + i.ToString();
             user.email = "p" + i.ToString() + "@gmail.com";
             user.contact = "01" + i.ToString() + "2244213";
             user.age = i.ToString();
             user.dob = i.ToString();
             user.gender = "male";
             user.game_result = "win";
             user.game_score = i.ToString();
             user.voucher_id = i.ToString();
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
         // if the email & contact is same
         if (emailInput.text == oldUser[i].email)
         {
             type = "email";
         }
         if (contactInput.text == oldUser[i].contact)
         {
             type = "contact";
         }
     }

     return type;
    }

    public bool CheckSameUserEmail()
    {
     bool emailIsSame = false;

     for (int i = 0; i < oldUser.Count; i++)
     {
         // if the email & contact is same
         if (emailInput.text == oldUser[i].email)
         {
             emailIsSame = true;
         }
     }

     return emailIsSame;
    }

    public bool CheckSameUserPhone()
    {
     bool contactIsSame = false;

     for (int i = 0; i < oldUser.Count; i++)
     {
         // if the email & contact is same
         if (contactInput.text == oldUser[i].contact)
         {
             contactIsSame = true;
         }
     }

     return contactIsSame;
    }
    #endregion


    public void SendDataToDatabase()
    {
     /**
    * Player data submission API
    * 
    * 2 possible results response from this player data submission api:
    * 1. Success
    * 2. Fail
    * 
    * Requires 8 parameters
    * 1. name
    * 2. email
    * 3. contact
    * 4. age
    * 5. dob
    * 6. gender
    * 7. game_result
    * 8. game_score
    * 9. voucher_name
    */
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


        SetUpDb();

        // Get unSync user
        List<UserEntity> unSyncUsers = new List<UserEntity>();
        unSyncUsers.Clear();

        try {
            unSyncUsers = userDb.GetAllUnSyncUser();

            CompareLocalAndServerData();
             Debug.Log("unsync users : " + unSyncUsers.Count);
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
            form.AddField("name", unSyncUsers[i].name); // 1
            form.AddField("email", unSyncUsers[i].email); // 2
            form.AddField("contact", unSyncUsers[i].contact); // 3
            form.AddField("age", unSyncUsers[i].age); // 4
            form.AddField("dob", unSyncUsers[i].dob); // 5
            form.AddField("gender", unSyncUsers[i].gender); // 6
            form.AddField("game_result", unSyncUsers[i].game_result); // 7
            form.AddField("game_score", unSyncUsers[i].game_score); // 8
            form.AddField("voucher_id", unSyncUsers[i].voucher_id); // 9
            form.AddField("register_datetime", unSyncUsers[i].register_datetime); // 10

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

        yield return StartCoroutine(osm.FeedUsers(serverUsers));

        if (serverUsers.Count < 1) { Debug.Log("no server user"); yield break; }

        for (int i = 0; i < serverUsers.Count; i++)
        {
            // add user never exist in local
            AddUniqueUser(serverUsers[i], oldUser);
        }

    }

    [ContextMenu("CompareLocalAndServerData")]
    public void CompareLocalAndServerData()
    {
        SetUpDb();

        foreach (UserEntity u in serverUsers)
        {
            RemoveDuplicateUser(u, unSyncUsers);
            userDb.UpdateSyncUser(u);
        }

        userDb.Close();
    }


    public void AddUniqueUser(UserEntity user, List<UserEntity> users)
    {
        UserEntity foundUser = users.FirstOrDefault(i => i.email == user.email);
        if (foundUser == null)
        {
            users.Add(user);
        }
      //  else
      //      Debug.Log("email : " + user.email + "already existed");
    }

    public void RemoveDuplicateUser(UserEntity user, List<UserEntity> users)
    {
        UserEntity foundUser = users.FirstOrDefault(i => i.email == user.email);
        if (foundUser != null)
        {
            users.Remove(user);
            Debug.LogWarning("Remove duplicate user with email " + user.email );
        }
        //  else
        //      Debug.Log("email : " + user.email + "already existed");
    }

    #endregion

}

    [System.Serializable]
    public class JSONResponse
    {
        public string result;
        public UserEntity[] users;
    }
