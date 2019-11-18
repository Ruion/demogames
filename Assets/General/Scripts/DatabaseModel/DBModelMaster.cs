using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.Data.Common;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using Sirenix.OdinInspector;

public class DBModelMaster : DBSettingEntity
{
    private const string CodistanTag = "Codistan: SqliteHelper:\t";

    [HideInInspector] public string db_connection_string;
    [HideInInspector] public IDbConnection db_connection;
    [HideInInspector] public SqliteConnection sqlitedb_connection;

    [FoldoutGroup("Populate Setting")] public int numberToPopulate = 10;
    [FoldoutGroup("Populate Setting")] public int TestIndex = 0;
    [FoldoutGroup("Populate Setting")] public string selectCustomCondition = "is_submitted = 'false'";

    protected virtual void OnEnable()
    {
        CreateTable();
    }

    #region setUp
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
            // Debug.Log("table success created"); }
        }
        catch (Exception ex) { Close(); Debug.LogError(ex.Message + "\n" + dbcmd.CommandText); return; }

       
    }

    public virtual void ConnectDb()
    {
        db_connection_string = "URI = file:" + Application.streamingAssetsPath + "/" + dbSettings.dbName + ".sqlite";
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

            Debug.Log("Columns : " + dt.Columns.Count + "| Rows : " + dt.Rows.Count + 
                "\n" +
                "View in Tools > Local DB (selecting the db gameObject) "
                );

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

            
            Debug.Log("Columns : " + dt.Columns.Count + "| Rows : " + dt.Rows.Count);

            foreach (DataRow r in dt.Rows)
            {
                string record = "";

                foreach (TableColumn col in dbSettings.columns)
                {
                    record += r[col.name].ToString() + " | ";
                }
                Debug.Log(record);
            }
            

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

    public void UpdateSyncData(string id)
    {
        List<string> col = new List<string>();
        List<string> con = new List<string>();

        col.Add("is_submitted");
        con.Add("true");

        string condition = "id = '" + id + "'";

        try { UpdateData(col, con, condition); Debug.Log("Update record " + id + " is_submitted to true"); }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }

    }

    #endregion

    #region Custom Query
    public virtual void ExecuteCustomNonQuery(string query)
    {
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

            val[val.Count - 1] = "false";

            AddData(col, val);
        }

        TestIndex++;
    }

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

    #region Save & Sync 
    public virtual void SaveToLocal()
    {
        LoadSetting();
    }

    [DisableIf("@String.IsNullOrEmpty(dbSettings.sendURL)")][Button(ButtonSizes.Medium)]
    public virtual void Sync()
    {
        LoadSetting();
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
}




