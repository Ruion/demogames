using Mono.Data.Sqlite;
using UnityEngine;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace DataBank
{
    public class UniversalDB : GameSettingEntity
    {
        private const string CodistanTag = "Codistan: UniversalDB:\t";

        public string db_connection_string;
        public IDbConnection db_connection;

        public UniversalDB()
        {

        }

        ~UniversalDB()
        {
            db_connection.Close();
        }

        public virtual void ConnectDb(string dbName)
        {
            db_connection_string = "URI = file:" + Application.streamingAssetsPath + "/" + dbName;
            db_connection = new SqliteConnection(db_connection_string);
            db_connection.Open();
        }

        [ContextMenu("Create table")]
        public void CreateTable()
        {
            new UniversalDB();
        }

        //vitual functions
        public virtual IDataReader GetDataById(int id)
        {
            Debug.Log(CodistanTag + "This function is not implemnted");
            throw null;
        }

        public virtual IDataReader GetDataByString(string conditionlowercase, string str)
        {
            Debug.Log(CodistanTag + "This function is not implemnted");
            throw null;
        }

        public virtual void DeleteDataById(int id)
        {
            Debug.Log(CodistanTag + "This function is not implemented");
            throw null;
        }

        public virtual void DeleteDataByString(string id)
        {
            Debug.Log(CodistanTag + "This function is not implemented");
            throw null;
        }

        public virtual IDataReader GetAllData()
        {
            Debug.Log(CodistanTag + "This function is not implemented");
            throw null;
        }

        public virtual void DeleteAllData()
        {
            Debug.Log(CodistanTag + "This function is not implemnted");
            throw null;
        }

        public virtual IDataReader GetNumOfRows()
        {
            Debug.Log(CodistanTag + "This function is not implemnted");
            throw null;
        }

        //helper functions
        public IDbCommand GetDbCommand()
        {
            return db_connection.CreateCommand();
        }

        public IDataReader GetAllData(string table_name)
        {
            try
            {
                IDbCommand dbcmd = db_connection.CreateCommand();
                dbcmd.CommandText =
                    "SELECT * FROM " + table_name;
                IDataReader reader = dbcmd.ExecuteReader();

                db_connection.Close();

                return reader;
            }
            catch (DbException ex)
            {
                Debug.Log("Error : " + ex.Message);
                db_connection.Close();
                return null;
            }
        }

        public void DeleteAllData(string table_name)
        {
            IDbCommand dbcmd = db_connection.CreateCommand();
            dbcmd.CommandText = "DROP TABLE IF EXISTS " + table_name;
            dbcmd.ExecuteNonQuery();
        }

        public IDataReader GetNumOfRows(string table_name)
        {
            IDbCommand dbcmd = db_connection.CreateCommand();
            dbcmd.CommandText =
                "SELECT COALESCE(MAX(id)+1, 0) FROM " + table_name;
            IDataReader reader = dbcmd.ExecuteReader();
            return reader;
        }

        public virtual void Close()
        {
            db_connection.Close();
        }
    }
}

[System.Serializable]
public class DBTable
{
    public List<string> columns;
    public List<string> properties;
}