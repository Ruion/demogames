using Mono.Data.Sqlite;
using UnityEngine;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace DataBank
{
    public class UniversalDB : GameSettingEntity
    {
        private const string CodistanTag = "Codistan: UniversalDB:\t";

        public string db_connection_string;
        public IDbConnection db_connection;

        public int numberToPopulate = 10;

        public virtual void SetUpSetting(){}

        public virtual void ConnectDb(string dbName)
        {
            db_connection_string = "URI = file:" + Application.streamingAssetsPath + "/" + dbName;
            db_connection = new SqliteConnection(db_connection_string);
            db_connection.Open();
        }

        public virtual void ConnectDbCustom() { }

        public virtual void CreateTable(){ }

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

        public virtual IDataReader GetAllData()
        {
            Debug.Log(CodistanTag + "This function is not implemented");
            throw null;
        }

        public virtual List<string> GetDataByStringToList(string singleColumnName, string conditionlowercase = "", string str = "")
        {
            ConnectDbCustom();

            IDbCommand dbcmd = GetDbCommand();
            dbcmd.CommandText =
                "SELECT " + singleColumnName + " FROM " + gameSettings.sQliteDBSettings.tableName + " WHERE " + conditionlowercase + " = '" + str + "' ";


            if (conditionlowercase != "" && str != "") dbcmd.CommandText += " WHERE " + conditionlowercase + " = '" + str + "' ";

            dbcmd.CommandText += ";";

            Debug.Log(dbcmd.CommandText);

            IDataReader reader = dbcmd.ExecuteReader();

            List<string> stringList = new List<string>();

            while (reader.Read())
            {
                stringList.Add(reader[0].ToString());
            }

            Close();

            return stringList;
        }

        public virtual List<string> GetDataByStringToList(string singleColumnName)
        {
            ConnectDbCustom();

            IDbCommand dbcmd = GetDbCommand();
            dbcmd.CommandText =
                "SELECT " + singleColumnName + " FROM " + gameSettings.sQliteDBSettings.tableName + " ;";

            Debug.Log(dbcmd.CommandText);

            IDataReader reader = dbcmd.ExecuteReader();

            List<string> stringList = new List<string>();

            while (reader.Read())
            {
                stringList.Add(reader[0].ToString());
            }

            Close();

            return stringList;
        }


        public virtual void Populate(){ CreateTable(); }

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


        public virtual void DeleteAllData(string table_name)
        {
            IDbCommand dbcmd = db_connection.CreateCommand();
            dbcmd.CommandText = "DROP TABLE IF EXISTS " + table_name;
            dbcmd.ExecuteNonQuery();
            Close();
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