using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.Data.Common;
using System;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// Sqlite database master class that hold database settings, methods for manipulating user data \n
/// Method Utilities : \n
/// Basic : Get, Insert, Update, Delete \n 
/// Server : Fetch string list from server \n
/// Public : Execute custom query on this database with ExecuteCustomNonQuery() and ExecuteCustomQuery() \n
/// Notes: see usage in DBModelEntity which inherit and extend this class
/// </summary>
public class DBModelMaster : DBSettingEntity
{
    #region fields
    private const string CodistanTag = "Codistan: SqliteHelper:\t";

    [HideInInspector] public string db_connection_string;
    [HideInInspector] public IDbConnection db_connection;
    [HideInInspector] public SqliteConnection sqlitedb_connection;

    [FoldoutGroup("Populate Setting")] public int numberToPopulate = 10;
    [FoldoutGroup("Populate Setting")] public int TestIndex = 0;
    [FoldoutGroup("Populate Setting")] public string selectCustomCondition = "online_status = 'new'";

    [ToggleGroup("hasSync")]
    public bool hasSync = false;
    [ToggleGroup("hasSync")] public GameObject emptyHandler;
    [ToggleGroup("hasSync")] public GameObject internetErrorHandler;
    [ToggleGroup("hasSync")] public GameObject errorHandler;
    [ToggleGroup("hasSync")] public GameObject blockDataHandler;
    [ToggleGroup("hasSync")] public GameObject successBar;
    [ToggleGroup("hasSync")] public GameObject failBar;
    [ToggleGroup("hasSync")] [ReadOnly] public int entityId;

    [HideInInspector] public List<string> serverEmailList;
    [ToggleGroup("hasSync")]
    [ReadOnly] public bool isFetchingData = false;

    #endregion

    protected virtual void OnEnable()
    {
        CreateTable();
    }

    #region setUp

    [Button(ButtonSizes.Large), GUIColor(.3f, .78f, .78f)][ButtonGroup("Setting")]
    protected override void SaveSetting()
    {
        base.SaveSetting();
        CreateTable();
    }

    [Button(ButtonSizes.Large), GUIColor(.3f, .78f, .78f)][ButtonGroup("Setting")]
    protected override void LoadSetting(){ base.LoadSetting(); }

    [ContextMenu("CreateTable")]
    public virtual void CreateTable()
    {
        LoadSetting();

        ConnectDb();

        IDbCommand dbcmd = GetDbCommand();

        // columns for table
        dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + dbSettings.tableName + " ( ";

        for (int i = 0; i < dbSettings.columns.Count; i++)
        {

            dbcmd.CommandText += "'" + dbSettings.columns[i].name + "' " + dbSettings.columns[i].attribute;

            if (i != dbSettings.columns.Count - 1) dbcmd.CommandText += " , ";
            else dbcmd.CommandText += " ) ; ";
        }

        try
        {
            dbcmd.ExecuteNonQuery(); Close();
        }
        catch (Exception ex) { Close(); Debug.LogError(ex.Message + "\n" + dbcmd.CommandText); return; }

       
    }

    public virtual void ConnectDb()
    {
        /// we use StreamingAssets folder in pass, but now use C:\UID-APP\APPS folder now
       // db_connection_string = "URI = file:" + Application.streamingAssetsPath + "/" + dbSettings.dbName + ".sqlite";
        db_connection_string = "URI = file:" + dbSettings.folderPath + "\\" + dbSettings.dbName + ".sqlite";
        db_connection = new SqliteConnection(db_connection_string);
        db_connection.Open();
    }

    //helper functions
    public IDbCommand GetDbCommand()
    {
        return db_connection.CreateCommand();
    }
    #endregion

    #region Insert
    public virtual string AddData(List<string> columns_, List<string> values_)
    {
        ConnectDb();
        #region example
        /*
        IDbCommand dbcmd = GetDbCommand();
        dbcmd.CommandText =
            "INSERT INTO " + dbSettings.tableName
            + " ( "
            + KEY_NAME + ", "
            + KEY_EMAIL + ", "
            + KEY_CONTACT + ", "
            + KEY_GAME_SCORE + " ) "

            + "VALUES ( '"
            + user.name + "', '"
            + user.email + "', '"
            + user.contact + "', '"
            + user.score + "' )";
        dbcmd.ExecuteNonQuery();

        foreach (var item in columns_)
        {

        }

        IDbCommand dbcmd2 = GetDbCommand();
        dbcmd2.CommandText =
            "INSERT INTO " + dbSettings.tableName
            + " ( "
            + KEY_NAME + ", "
            + KEY_EMAIL + ", "
            + KEY_CONTACT + ", "
            + KEY_GAME_SCORE + " ) "

            + "VALUES ( '"
            + user.name + "', '"
            + user.email + "', '"
            + user.contact + "', '"
            + user.score + "' )";
        dbcmd.ExecuteNonQuery();
        */
        #endregion

        IDbCommand dbcmd2 = GetDbCommand();
        dbcmd2.CommandText =
            "INSERT INTO " + dbSettings.tableName
            + " ( ";

        for (int n = 0; n < columns_.Count; n++)
        {
            if (n != columns_.Count - 1)
            { dbcmd2.CommandText += columns_[n] + ", "; }

            else
            {
                dbcmd2.CommandText += columns_[n];
            }
        }

        dbcmd2.CommandText += ")";
        dbcmd2.CommandText += " VALUES ( '";

        for (int v = 0; v < values_.Count; v++)
        {
            if (v != columns_.Count - 1)
            { dbcmd2.CommandText += values_[v] + "', '"; }

            else
            {
                dbcmd2.CommandText += values_[v];
            }
        }

        dbcmd2.CommandText += "')";

        using (db_connection)
        {
            try
            {
                dbcmd2.ExecuteNonQuery(); Close();
                return "true";

            }
            catch (DbException ex)
            {
                string msg = string.Format("ErrorCode: {0}", ex.Message + "\n" + dbcmd2.CommandText);
                Debug.LogError(msg); Close();
                return msg;
            }
        }
    }
    #endregion

    #region Get
    [ButtonGroup("DBGet")]
    [Button("Show All", ButtonSizes.Medium)]
    public virtual DataTable GetAllDataInToDataTable()
    {
        ConnectDb();
        try
        {
            string query = "SELECT * FROM " + dbSettings.tableName;
            sqlitedb_connection = new SqliteConnection(db_connection_string);

            SqliteCommand cmd = new SqliteCommand(query, sqlitedb_connection);

            SqliteDataAdapter da = new SqliteDataAdapter(cmd);

            DataTable dt = new DataTable();
            da.Fill(dt);

/*            Debug.Log("Columns : " + dt.Columns.Count + "| Rows : " + dt.Rows.Count + 
                "\n" +
                "View in Tools > Local DB (selecting the db gameObject) "
                );
                */

            /*
            foreach (DataRow r in dt.Rows)
            {
                string record = "";

                foreach (TableColumn col in dbSettings.columns)
                {
                    record += col.name + " : " + r[col.name].ToString() + " | ";
                    // Debug.Log(record);
                }
                Debug.Log(record);
            }
            */

            Close();
            return dt;

        }
        catch (DbException ex)
        {
            Debug.Log("Error : " + ex.Message);
            Close();
            return null;
        }
    }

    [ButtonGroup("DBGet")]
    [Button("Show Custom", ButtonSizes.Medium)]
    public DataRowCollection GetAllCustomCondition()
    {
        ConnectDb();
        try
        {
            string query = "SELECT * FROM " + dbSettings.tableName + " WHERE " + selectCustomCondition;
            sqlitedb_connection = new SqliteConnection(db_connection_string);

            SqliteCommand cmd = new SqliteCommand(query, sqlitedb_connection);

            SqliteDataAdapter da = new SqliteDataAdapter(cmd);

            DataTable dt = new DataTable();
            da.Fill(dt);

            
//            Debug.Log("Columns : " + dt.Columns.Count + "| Rows : " + dt.Rows.Count);

/*
            foreach (DataRow r in dt.Rows)
            {
                string record = "";

                foreach (TableColumn col in dbSettings.columns)
                {
                    record += r[col.name].ToString() + " | ";
                }
                Debug.Log(record);
            }
 */           

            Close();
            return dt.Rows;

        }
        catch (DbException ex)
        {
            Debug.LogError("Error : " + ex.Message);
            Close();
            return null;
        }
    }

    public List<string> GetDataByStringToList(string item)
    {
        List<string> list = new List<string>();

        try
        {
            ConnectDb();
            DataRowCollection drc = ExecuteCustomSelectQuery("SELECT " + item + " FROM " + dbSettings.tableName);

            for (int d = 0; d < drc.Count; d++)
            {
                list.Add(drc[0][0].ToString());
            }

            return list;
        }catch(Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }

    }
    #endregion

    #region Delete
    [ContextMenu("DropTable")]
    public virtual void DeleteAllData()
    {
        ConnectDb();
        IDbCommand dbcmd = db_connection.CreateCommand();
        dbcmd.CommandText = "DROP TABLE IF EXISTS " + dbSettings.tableName;
        dbcmd.ExecuteNonQuery();
        Close();
        TestIndex++;
    }

    [Button(ButtonSizes.Medium)][FoldoutGroup("Populate Setting")][HorizontalGroup("Populate Setting/Btn")]
    public virtual void ClearAllData()
    {
        ConnectDb();
        IDbCommand dbcmd = db_connection.CreateCommand();
        dbcmd.CommandText = "DELETE FROM " + dbSettings.tableName ;
        dbcmd.ExecuteNonQuery();
        Close();
        TestIndex++;
    }
    #endregion

    #region Update
    public void UpdateData(List<string> columns_, List<string> values_, string conditions)
    {
        /*
        UPDATE table_name
        SET column1 = value1, column2 = value2...., columnN = valueN
        WHERE[condition];
        */
        ConnectDb();

        IDbCommand dbcmd2 = GetDbCommand();
        dbcmd2.CommandText =
            "UPDATE " + dbSettings.tableName
            + " SET ";


        for (int c = 0; c < columns_.Count; c++)
        {
            dbcmd2.CommandText += columns_[c] + " = '";
        }

        for (int v = 0; v < values_.Count; v++)
        {
            if (v != values_.Count - 1) dbcmd2.CommandText += values_[v] + "' ,";

            else dbcmd2.CommandText += values_[v] + "'";
        }

        dbcmd2.CommandText += " WHERE ";
        dbcmd2.CommandText += conditions + " ;";

        try
        {
            int result = dbcmd2.ExecuteNonQuery();
            if (result == 0) Debug.LogError("query not successful");
        }
        catch (DbException ex)
        {
            string msg = string.Format("ErrorCode: {0}", ex.Message);
            Debug.LogError(dbcmd2.CommandText);
            Debug.LogError(msg);
        }

        Close();
    }


    #endregion

    #region Custom Query
    public virtual void ExecuteCustomNonQuery(string query)
    {
        #region Usage
        // Usage
        // ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET quantity = " + voucher_quantity + " WHERE name = '" + voucher_name + "'");
        #endregion

        sqlitedb_connection = new SqliteConnection(db_connection_string);
        sqlitedb_connection.Open();

        SqliteCommand cmd = new SqliteCommand(query, sqlitedb_connection);

        try
        {
            cmd.ExecuteNonQuery();

            Debug.Log("Custom Query success" + "\n" + cmd.CommandText);
            sqlitedb_connection.Close();
        }
        catch (DbException ex)
        {
            Debug.Log("Error : " + ex.Message + "\n" + cmd.CommandText);
            sqlitedb_connection.Close();
        }
    }

    public virtual DataRowCollection ExecuteCustomSelectQuery(string query)
    {
        #region Usage
        // Usage
        // DataRowCollection drc = ExecuteCustomSelectQuery("SELECT " + item + " FROM " + dbSettings.tableName);
        //    for (int d = 0; d < drc.Count; d++)
        //    {
        //        list.Add(drc[0][0].ToString()); drc[0][0] means drc[row0][column0]
        //    }
        #endregion

        ConnectDb();
        try
        {
            sqlitedb_connection = new SqliteConnection(db_connection_string);

            SqliteCommand cmd = new SqliteCommand(query, sqlitedb_connection);

            SqliteDataAdapter da = new SqliteDataAdapter(cmd);

            DataTable dt = new DataTable();

            da.Fill(dt);

            Close();

            return dt.Rows;
        }
        catch (DbException ex)
        {
            Debug.Log("Error : " + ex.Message);
            Close();
            return null;
        }
    }
    #endregion

    public virtual void Close()
    {
        db_connection.Close();
    }

    [Button(ButtonSizes.Medium)][HorizontalGroup("Populate Setting/Btn")]
    public virtual void Populate()
    {
        CreateTable();

        List<string> col = new List<string>();
        List<string> val = new List<string>();

        for (int v = 1; v < dbSettings.columns.Count; v++)
        {
            col.Add(dbSettings.columns[v].name);
        }

        val.AddRange(col);

        for (int n = 0; n < numberToPopulate; n++)
        {
            for (int i = 0; i < col.Count; i++)
            {
                val[i] = dbSettings.columns[i+1].dummyPrefix + ((n + 1).ToString());
            }

            val[val.Count - 1] = "new";

            AddData(col, val);
        }

        TestIndex++;
    }
    
    #region handler
    public virtual void HideAllHandler()
    {
        emptyHandler.SetActive(false);
        internetErrorHandler.SetActive(false);
        errorHandler.SetActive(false);
        blockDataHandler.SetActive(false); ;
        successBar.SetActive(false);
        failBar.SetActive(false);
    }

    protected virtual void ToogleHandler(GameObject handler, bool state = false)
    {
        if (handler == null) return;

        if (handler.transform.parent.gameObject.activeInHierarchy) handler.SetActive(state);
    }

    protected virtual void ToogleStatusBar(GameObject bar, int total)
    {
        if (bar == null) return;
        ToogleHandler(bar, true);
        bar.GetComponentInChildren<TextMeshProUGUI>().text = total.ToString();
    }

    #endregion

    #region Save & Sync 
    public virtual void SaveToLocal()
    {
        LoadSetting();
    }

    [DisableIf("@String.IsNullOrEmpty(dbSettings.sendURL)")][Button(ButtonSizes.Medium)]
    public virtual void Sync()
    {
        //LoadSetting();
        #region Check Internet
        ///////////// CHECK Internet Connection /////////////
        string HtmlText = GetHtmlFromUri("http://google.com");
        if (HtmlText == "")
        {
            //No connection
            Debug.LogError("No internet connection");
            return;
        }
        #endregion
    }

    #endregion

    #region Online Server Fetching

    protected virtual string GetHtmlFromUri(string resource = "http://google.com")
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

    public void AddUniqueDataToStringList(string text, List<string> dataList)
    {
        if (!dataList.Exists(i => i == text)) dataList.Add(text);
    }

    public bool RemoveDuplicateStringItem(string text, List<string> list)
    {
        string foundData = list.FirstOrDefault(i => i == text);
        if (foundData != null)
        {
            list.Remove(foundData);
            Debug.LogWarning("Remove duplicate item with " + foundData);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SetUpTextPath()
    {
        dbSettings.serverEmailFilePath = Application.streamingAssetsPath + "/" + dbSettings.keyFileName + ".txt";
        if (!File.Exists(dbSettings.serverEmailFilePath))
        {
            File.WriteAllText(dbSettings.serverEmailFilePath, "");
        }
    }

    public void DoGetDataFromServer()
    {
        try{
        SetUpTextPath();
        if (isFetchingData) return;

        isFetchingData = true;
        StartCoroutine(GetDataFromServer());
        }catch{
            Debug.LogError(name);
        }
    }

    public IEnumerator GetDataFromServer()
    {
        if (!dbSettings.hasMultipleLocalDB) yield break;

        SetUpTextPath();

        serverEmailList = new List<string>();

        string HtmlText = GetHtmlFromUri();
        if (HtmlText == "")
        {
            //No connection
            Debug.LogError("no internet connection");
            isFetchingData = false;
            yield break;
        }
        else
        {
            using (UnityWebRequest www = UnityWebRequest.Get(dbSettings.keyDownloadURL))
            {
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.LogError(www.error);
                    isFetchingData = false;
                    yield break;
                }
                else
                {
                    while (!www.downloadHandler.isDone) yield return null;

                    string texts = www.downloadHandler.text;

                    // clear text file
                    File.WriteAllText(dbSettings.serverEmailFilePath, "");

                    // write email list to file
                    StreamWriter writer = new StreamWriter(dbSettings.serverEmailFilePath, true); //open txt file (doesnt actually open it inside the game)
                    writer.Write(texts); //write into txt file the string declared above
                    writer.Close();

                    List<string> lines = new List<string>(
                     texts
                     .Split(new string[] { "\r", "\n" },
                     System.StringSplitOptions.RemoveEmptyEntries));

                    lines = lines
                        .Where(line => !(line.StartsWith("//")
                                        || line.StartsWith("#")))
                        .ToList();

                    foreach (string line in lines)
                    {
                        serverEmailList.Add(line.ToString());
                    }
                }
            }
        }

        isFetchingData = false;
    }

/// <summary>
/// call GetDataFromServer() coroutine to get the list of email from server, call UpdateEmailExistedOnline() to
/// update "online_status = 'duplicate'" of email in local database if the email already exist in server
/// </summary>
/// <returns></returns>
    public IEnumerator CompareServerData()
    {
        yield return StartCoroutine(GetDataFromServer());
        UpdateEmailExistedOnline();
    }

/// <summary>
/// update "online_status = 'duplicate'" of email in local database if the email already exist in server
/// </summary>
    public void UpdateEmailExistedOnline()
    {
        if (serverEmailList.Count < 0) return;

        for (int o = 0; o < serverEmailList.Count; o++)
        {
            ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET online_status = 'duplicate' WHERE email = '" + serverEmailList[o] + "'");
        }

        serverEmailList = new List<string>();
    }

    #endregion
}




