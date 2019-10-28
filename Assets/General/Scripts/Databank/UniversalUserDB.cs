using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace DataBank
{
    public class UniversalUserDB : UniversalDB
    {
        #region variables
        private const string CodistanTag = "UniversalUserDB:\t";

        private string KEY_NAME = "name"; // 1
        private string KEY_EMAIL = "email"; // 2
        private string KEY_CONTACT = "contact"; // 3
        private string KEY_AGE = "age"; // 4
        private string KEY_DOB = "dob"; // 5
        private string KEY_GENDER = "gender"; // 6
        private string KEY_GAME_SCORE = "game_score"; // 7
        private string KEY_GAME_RESULT = "game_result"; // 8
        private string KEY_VOUCHER_ID = "voucher_id"; // 9
        private string KEY_DATE = "register_datetime"; // 10
        private string KEY_SYNC = "is_submitted"; // 11

        #endregion

        [ContextMenu("Create table")]
        public override void CreateTable()
        {
            LoadGameSettingFromMaster();

            ConnectDb(gameSettings.sQliteDBSettings.dbName);

            IDbCommand dbcmd = GetDbCommand();

            // columns for table
            dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + gameSettings.sQliteDBSettings.tableName + " ( " ;

            for (int i = 0; i < gameSettings.sQliteDBSettings.columns.Count; i++)
            {

                dbcmd.CommandText += "'"+ gameSettings.sQliteDBSettings.columns[i] + "' " + gameSettings.sQliteDBSettings.attributes[i];

                if (i != gameSettings.sQliteDBSettings.columns.Count-1) dbcmd.CommandText += " , ";
                else dbcmd.CommandText += " ) ; ";
            }

            try { dbcmd.ExecuteNonQuery(); Debug.Log("table success created"); }
            catch (Exception ex) { Debug.LogError(ex.Message); return; }

            db_connection.Close();
        }

        public override void ConnectDbCustom()
        {
            LoadGameSettingFromMaster(); 
            ConnectDb(gameSettings.sQliteDBSettings.dbName);
        }

        public string AddData(List<string> columns_, List<string> values_)
        {
            ConnectDbCustom();
            #region example
            /*
            IDbCommand dbcmd = GetDbCommand();
            dbcmd.CommandText =
                "INSERT INTO " + gameSettings.sQliteDBSettings.tableName
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
                "INSERT INTO " + gameSettings.sQliteDBSettings.tableName
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
                "INSERT INTO " + gameSettings.sQliteDBSettings.tableName
                + " ( ";

            foreach (var item in columns_)
            {
                if (columns_.IndexOf(item) != (columns_.Count - 1))
                { dbcmd2.CommandText += item + ", "; }

                else
                {
                    dbcmd2.CommandText += item;
                }
            }

            dbcmd2.CommandText += ")";
            dbcmd2.CommandText += " VALUES ( '";

            foreach (var v in values_)
            {
                if (values_.IndexOf(v) != (values_.Count - 1))
                { dbcmd2.CommandText += v + "', '"; }

                else
                {
                    dbcmd2.CommandText += v;
                }
            }

            dbcmd2.CommandText += "')";

            using (db_connection)
            {
                try
                {
                    dbcmd2.ExecuteNonQuery();
                    return "true";
                }
                catch (DbException ex)
                {
                    string msg = string.Format("ErrorCode: {0}", ex.Message);
                    Debug.LogError(msg);
                    return msg;
                }
            }
        }

        public override void Populate()
        {
            ConnectDbCustom();

            base.Populate();

            List<string> col = new List<string>();
            col.AddRange(gameSettings.sQliteDBSettings.columns);

            List<string> val = new List<string>();
            val.AddRange(gameSettings.sQliteDBSettings.columns);

            col.RemoveAt(0);
            val.RemoveAt(0);


            for (int n = 0; n < numberToPopulate; n++)
            {
                for (int i = 0; i < col.Count; i++)
                {
                    val[i] = col[i] + ((n+1).ToString());
                }

                AddData(col, val);
            }

            Close();
        }

        public void UpdateData(List<string> columns_, List<string> values_, string conditions)
        {
            /*
            UPDATE table_name
            SET column1 = value1, column2 = value2...., columnN = valueN
            WHERE[condition];
            */

            IDbCommand dbcmd2 = GetDbCommand();
            dbcmd2.CommandText =
                "UPDATE " + gameSettings.sQliteDBSettings.tableName
                + " SET ";

            foreach (string c in columns_)
            {
                dbcmd2.CommandText += c + " = '";
            }

            foreach (string v in values_)
            {
                if (values_.IndexOf(v) != values_.Count - 1) dbcmd2.CommandText += v + "' ,";

                else dbcmd2.CommandText += v + "'";
            }

            dbcmd2.CommandText += " WHERE ";
            dbcmd2.CommandText += conditions + " ;";

            try
            {
                int result = dbcmd2.ExecuteNonQuery();
                if (result == 0) Debug.LogWarning("query not successful");
            }
            catch (DbException ex)
            {
                string msg = string.Format("ErrorCode: {0}", ex.Message);
                Debug.Log(msg);
            }
        }

        public override IDataReader GetDataById(int id)
        {
            return base.GetDataById(id);
        }

        public override IDataReader GetDataByString(string conditionlowercase, string str)
        {
            Debug.Log(CodistanTag + "Getting data: " + str);

            IDbCommand dbcmd = GetDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + gameSettings.sQliteDBSettings.tableName + " WHERE " + conditionlowercase + " = '" + str + "'";
            return dbcmd.ExecuteReader();
        }
       
        public override void DeleteDataByString(string id)
        {
            Debug.Log(CodistanTag + "Deleting Location: " + id);

            IDbCommand dbcmd = GetDbCommand();
            dbcmd.CommandText =
                "DELETE FROM " + gameSettings.sQliteDBSettings.tableName + " WHERE " + KEY_CONTACT + " = '" + id + "'";
            dbcmd.ExecuteNonQuery();
        }

        public override void DeleteDataById(int id)
        {
            base.DeleteDataById(id);
        }

        public override void DeleteAllData()
        {
            Debug.Log(CodistanTag + "Deleting Table");

            ConnectDbCustom();

            base.DeleteAllData(gameSettings.sQliteDBSettings.tableName);

            Close();
        }

        [ContextMenu("GetAllData")]
        public override IDataReader GetAllData()
        {
            ConnectDbCustom();

            List<UniversalUserEntity> entities = new List<UniversalUserEntity>();

            IDbCommand dbcommand = db_connection.CreateCommand();
            dbcommand.CommandText = "SELECT * FROM " + gameSettings.sQliteDBSettings.tableName + " ;";

            IDataReader reader = dbcommand.ExecuteReader();

            if (reader.FieldCount < 1) { Debug.LogError("Empty record"); return null; }
            else
            {
              //  Debug.Log(reader.FieldCount + " record in table " + gameSettings.sqliteDBSettings.tableName);
            }

            return reader;
        }

        public List<object> GetAllData(string userClassName)
        {
            ConnectDbCustom();

            Type t = Type.GetType(userClassName);

            List<object> entityObjects = new List<object>();
            
            FieldInfo[] fieldInfo = t.GetFields();

            IDbCommand dbcommand = db_connection.CreateCommand();
            dbcommand.CommandText = "SELECT * FROM " + gameSettings.sQliteDBSettings.tableName + " ;";

            IDataReader reader = dbcommand.ExecuteReader();

            if (reader.FieldCount < 1) { Debug.LogError("Empty record"); return null; }
            else
            {
                  Debug.Log(reader.FieldCount + " columns in table " + gameSettings.sQliteDBSettings.tableName);
            }

            while (reader.Read())
            {
               var userInstance = Activator.CreateInstance(t);
                  
                for (int i = 1; i < reader.FieldCount; i++)
                {
                    fieldInfo[i].SetValue(userInstance, reader[i].ToString());
                    Debug.Log(fieldInfo[i].Name + " : " + userInstance.GetType().GetField(fieldInfo[i].Name).GetValue(userInstance));
                }

                entityObjects.Add(userInstance); 
            }

            Close();

            return entityObjects;
        }

        public List<UniversalUserEntity> GetAllUser()
        {
            List<UniversalUserEntity> entities = new List<UniversalUserEntity>();
            FieldInfo[] fieldInfo = Type.GetType(gameSettings.sQliteDBSettings.UniversalUserClassName).GetFields();

            IDataReader reader = GetAllData();

            while (reader.Read())
            {
                UniversalUserEntity entity = new UniversalUserEntity();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    fieldInfo[i].SetValue(entity, reader[i].ToString());
                }

                entities.Add(entity);
            }

            Close();

            return entities;
        }

        public List<string> GetAllUserEmail()
        {
            List<string> entities = new List<string>();

            IDbCommand dbcmd = GetDbCommand();

            string command = "SELECT email FROM " + gameSettings.sQliteDBSettings.tableName;

            Debug.Log(command);
            dbcmd.CommandText = command;

            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                entities.Add(reader[0].ToString());
            }

            return entities;
        }

        public List<UniversalUserEntity> GetAllUnSyncUser()
        {
            List<UniversalUserEntity> entities = new List<UniversalUserEntity>();

            ConnectDbCustom();

            FieldInfo[] fieldInfo = Type.GetType(gameSettings.sQliteDBSettings.UniversalUserClassName).GetFields();

            /*    IDbCommand dbcmd = GetDbCommand();
                dbcmd.CommandText =
                    "SELECT * FROM " + gameSettings.sQliteDBSettings.tableName + " WHERE " + KEY_SYNC + " = 'false'";

                IDataReader reader = dbcmd.ExecuteReader();
            */

            IDataReader reader = GetDataByString(KEY_SYNC, "false");

            while (reader.Read())
            {
                UniversalUserEntity entity = new UniversalUserEntity();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    fieldInfo[i].SetValue(entity, reader[i].ToString());
                }

                entities.Add(entity);
            }

            Close();

            return entities;
        }

        public void UpdateSyncUser(UniversalUserEntity userEntity)
        {
            List<string> col = new List<string>();
            List<string> con = new List<string>();

            col.Add("is_submitted");
            con.Add("true");

            string condition = KEY_EMAIL + " = '" + userEntity.email + "'";

            try { UpdateData(col, con, condition); Debug.Log("Update user " + userEntity.email + " to submitted"); }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }

        public override void DeleteAllData(string table_name)
        {
            ConnectDbCustom();
            base.DeleteAllData(table_name);
        }
    }
}
