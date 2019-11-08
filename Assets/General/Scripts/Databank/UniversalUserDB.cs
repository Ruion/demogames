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
        private string KEY_GAME_SCORE = "game_score"; // 7
        private string KEY_GAME_RESULT = "game_result"; // 8
        private string KEY_VOUCHER_ID = "voucher_id"; // 9
        private string KEY_DATE = "created_at"; // 10
        private string KEY_SYNC = "is_submitted"; // 11

        public int TestIndex = 5;
        #endregion

        private void Awake()
        {
            CreateTable();
        }

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

            for (int n = 0; n < columns_.Count; n++)
            {
                if (n != columns_.Count - 1)
                { dbcmd2.CommandText += columns_[n] + ", "; }

                else
                {
                    dbcmd2.CommandText += columns_[n];
                }
            }

            /*
            foreach (var item in columns_)
            {
                if (columns_.IndexOf(item) != (columns_.Count - 1))
                { dbcmd2.CommandText += item + ", "; }

                else
                {
                    dbcmd2.CommandText += item;
                }
            }
            */

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

            /*
            foreach (var v in values_)
            {
                if (values_.IndexOf(v) != (values_.Count - 1))
                { dbcmd2.CommandText += v + "', '"; }

                else
                {
                    dbcmd2.CommandText += v;
                }
            }
            */

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
                    val[1] = "test00" + TestIndex.ToString() + n.ToString() + "@gmail.com";
                }
                
                val[2] = "+60145445" + (n+TestIndex)+n.ToString();
                val[3] = n.ToString();
                val[4] = "0000:00:00";
                val[5] = "male";
                val[6] = "lose";
                val[7] = "200";
                val[val.Count - 2] = System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                val[val.Count - 1] = "false";

                AddData(col, val);
            }

            TestIndex++;

            Close();
        }

        public void UpdateData(List<string> columns_, List<string> values_, string conditions)
        {
            /*
            UPDATE table_name
            SET column1 = value1, column2 = value2...., columnN = valueN
            WHERE[condition];
            */
            ConnectDbCustom();

            IDbCommand dbcmd2 = GetDbCommand();
            dbcmd2.CommandText =
                "UPDATE " + gameSettings.sQliteDBSettings.tableName
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
                if (result == 0) Debug.LogWarning("query not successful");
            }
            catch (DbException ex)
            {
                string msg = string.Format("ErrorCode: {0}", ex.Message);
                Debug.LogWarning(dbcmd2.CommandText);
                Debug.LogWarning(msg);
            }

            Close();
        }

        #region Get Data
        public override IDataReader GetDataById(int id)
        {
            return base.GetDataById(id);
        }

        public override IDataReader GetDataByString(string conditionlowercase, string str)
        {
            ConnectDbCustom();
            Debug.Log(CodistanTag + "Getting data: " + conditionlowercase + " " + str);

            IDbCommand dbcmd = GetDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + gameSettings.sQliteDBSettings.tableName + " WHERE " + conditionlowercase + " = '" + str + "'";
            return dbcmd.ExecuteReader();
        }

        public IDataReader GetDataByStringLimit(string conditionlowercase, string str, int limitNumber)
        {
            ConnectDbCustom();
            Debug.Log(CodistanTag + "Getting data: " + conditionlowercase + " " + str);

            IDbCommand dbcmd = GetDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + gameSettings.sQliteDBSettings.tableName + " WHERE " + conditionlowercase + " = '" + str + "' LIMIT " + limitNumber.ToString();
            return dbcmd.ExecuteReader();
        }

        #endregion



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

                string columnData = "";

                for (int i = 1; i < reader.FieldCount; i++)
                {
                   if(i<reader.FieldCount) fieldInfo[i].SetValue(userInstance, reader[i].ToString());

                    columnData += fieldInfo[i].Name + " : " + userInstance.GetType().GetField(fieldInfo[i].Name).GetValue(userInstance) + " | " ;
                 //   Debug.Log(fieldInfo[i].Name + " : " + userInstance.GetType().GetField(fieldInfo[i].Name).GetValue(userInstance));
                }

                Debug.Log(columnData);
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

        public List<UniversalUserEntity> GetAllUnSyncUserSlow()
        {
            List<UniversalUserEntity> entities = new List<UniversalUserEntity>();

            ConnectDbCustom();

            FieldInfo[] fieldInfo = Type.GetType(gameSettings.sQliteDBSettings.UniversalUserClassName).GetFields();

            IDataReader reader = GetDataByString(KEY_SYNC, "false");
         //   Debug.Log("Reader fieldCount : " + reader.FieldCount);

            while (reader.Read())
            {
                UniversalUserEntity entity = new UniversalUserEntity();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                   // Debug.Log("Index i : " + i);
                    fieldInfo[i].SetValue(entity, reader[i].ToString());
                }

                entities.Add(entity);
            }

            Close();

            return entities;
        }

        public List<UniversalUserEntity> GetAllUnSyncUser()
        {
            List<UniversalUserEntity> entities = new List<UniversalUserEntity>();

            ConnectDbCustom();

            IDataReader reader = GetDataByString(KEY_SYNC, "false");
            //   Debug.Log("Reader fieldCount : " + reader.FieldCount);

            while (reader.Read())
            {
                UniversalUserEntity entity = new UniversalUserEntity();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    // Debug.Log("Index i : " + i);
                    entity.name = reader[1].ToString();
                    entity.email = reader[2].ToString();
                    entity.contact = reader[3].ToString();
                //    entity.age = reader[4].ToString();
                //    entity.dob = reader[5].ToString();
                //   entity.gender = reader[6].ToString();
                    entity.game_result = reader[7].ToString();
                    entity.game_score = reader[8].ToString();
                    entity.created_at = reader[9].ToString();
                    entity.is_submitted = reader[10].ToString();
                }

                entities.Add(entity);
            }

            Close();

            return entities;
        }

        [ContextMenu("Get10Unsync")]
        public List<UniversalUserEntity> GetAllUnSyncUserLimit()
        {
            List<UniversalUserEntity> entities = new List<UniversalUserEntity>();

            ConnectDbCustom();

            IDataReader reader = GetDataByStringLimit(KEY_SYNC, "false", 10);
             Debug.Log("Reader fieldCount : " + reader.FieldCount);

            while (reader.Read())
            {
                UniversalUserEntity entity = new UniversalUserEntity();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    // Debug.Log("Index i : " + i);
                    entity.name = reader[1].ToString();
                    entity.email = reader[2].ToString();
                    entity.contact = reader[3].ToString();
                    //    entity.age = reader[4].ToString();
                    //    entity.dob = reader[5].ToString();
                    //   entity.gender = reader[6].ToString();
                    entity.game_result = reader[7].ToString();
                    entity.game_score = reader[8].ToString();
                    entity.created_at = reader[9].ToString();
                    entity.is_submitted = reader[10].ToString();
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

        #region Delete Data

        public override void DeleteAllData(string table_name)
        {
            ConnectDbCustom();
            base.DeleteAllData(table_name);
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
        #endregion

    }
}
