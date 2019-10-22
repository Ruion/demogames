using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data.Common;

namespace DataBank{

    [RequireComponent(typeof(DataManager))]
	public class UserDB : SqliteHelper {
        #region variables
        private const string CodistanTag = "Codistan: UserDB:\t";
        
        private string TABLE_NAME = "user";
        private string KEY_NAME = "name";
        private string KEY_EMAIL = "email";
        private string KEY_PHONE = "phone";
        private string KEY_SCORE = "score";
		private string KEY_DATE = "register_datetime";
		private string KEY_SYNC = "is_submitted";

        public List<string> columns = new List<string>();
        #endregion

        //  private string[] COLUMNS = new string[] {KEY_ID, KEY_TYPE, KEY_LAT, KEY_LNG, KEY_DATE};

        public UserDB(string db_name, string table_name) : base()
        {
            db_connection_string = "URI = file:" + Application.streamingAssetsPath + "/" + db_name;
            db_connection = new SqliteConnection(db_connection_string);
            db_connection.Open();

            TABLE_NAME = table_name;

            IDbCommand dbcmd = GetDbCommand();

            // columns for table

            dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " ( " +
                "id" + " INTEGER PRIMARY KEY, " +
                KEY_NAME + " TEXT, " +
                KEY_EMAIL + " TEXT UNIQUE, " +
                KEY_PHONE + " TEXT UNIQUE, " +
                KEY_SCORE + " TEXT, " +
                KEY_DATE + " TEXT, " +
                KEY_SYNC + " TEXT )";
            dbcmd.ExecuteNonQuery();

            return;
        }

        public string AddData(List<string> columns_, List<string> values_)
        {
            #region example
            /*
            IDbCommand dbcmd = GetDbCommand();
            dbcmd.CommandText =
                "INSERT INTO " + TABLE_NAME
                + " ( "
                + KEY_NAME + ", "
                + KEY_EMAIL + ", "
                + KEY_PHONE + ", "
                + KEY_SCORE + " ) "

                + "VALUES ( '"
                + user.name + "', '"
                + user.email + "', '"
                + user.phone + "', '"
                + user.score + "' )";
            dbcmd.ExecuteNonQuery();

            foreach (var item in columns_)
            {
                
            }

            IDbCommand dbcmd2 = GetDbCommand();
            dbcmd2.CommandText =
                "INSERT INTO " + TABLE_NAME
                + " ( "
                + KEY_NAME + ", "
                + KEY_EMAIL + ", "
                + KEY_PHONE + ", "
                + KEY_SCORE + " ) "

                + "VALUES ( '"
                + user.name + "', '"
                + user.email + "', '"
                + user.phone + "', '"
                + user.score + "' )";
            dbcmd.ExecuteNonQuery();
            */
            #endregion

            IDbCommand dbcmd2 = GetDbCommand();
            dbcmd2.CommandText =
                "INSERT INTO " + TABLE_NAME
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
                    Debug.Log(msg);
                    return msg;
                }
            }
        }

        public string AddData(UserEntity user)
        {
            IDbCommand dbcmd = GetDbCommand();
            dbcmd.CommandText =
                "INSERT INTO " + TABLE_NAME
                + " ( "
                + KEY_NAME + ", "
                + KEY_EMAIL + ", "
                + KEY_PHONE + ", "
                + KEY_DATE + ", "
                + KEY_SYNC  + " ) "

                + "VALUES ( '"
                + user.name + "', '"
                + user.email + "', '"
                + user.phone + "', '"
                + user.register_datetime + "', '"
                + user.is_submitted + "' )";

            using (db_connection)
            {
                try
                {
                    dbcmd.ExecuteNonQuery();
                    return "true";
                }
                catch (DbException ex)
                {
                    string msg = string.Format("ErrorCode: {0}", ex.Message);
                    Debug.Log(msg);
                    return msg;
                }
            }
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
                "UPDATE " + TABLE_NAME
                + " SET ";

            foreach (string c in columns_)
            {
                dbcmd2.CommandText += c + " = '";
            }

            foreach (string v in values_)
            {
                if(values_.IndexOf(v) != values_.Count - 1) dbcmd2.CommandText += v + "' ,";

                else dbcmd2.CommandText += v + "'"; 
            }

            dbcmd2.CommandText += " WHERE ";
            dbcmd2.CommandText += conditions + " ;";

            try { dbcmd2.ExecuteNonQuery(); }
            catch (DbException ex)
            {
                string msg = string.Format("ErrorCode: {0}", ex.Message);
                Debug.Log(msg);
            }
        }

        public string UpdateLastDataScore(string score)
        {
           IDataReader lastReader =  GetLatestTimeStamp();
            UserEntity lastEntity = new UserEntity();

            while (lastReader.Read())
            {
                lastEntity.phone = lastReader[3].ToString();
                lastEntity.score = lastReader[4].ToString();
            }

            IDbCommand dbcmd2 = GetDbCommand();
            dbcmd2.CommandText = "UPDATE " + TABLE_NAME + " SET " + KEY_SCORE + " = '" + score + "' WHERE " + KEY_PHONE + " = '" + lastEntity.phone + "' ;";

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
                    Debug.Log(msg);
                    return msg;
                }
            }

        }

        public override IDataReader GetDataById(int id)
        {
            return base.GetDataById(id);
        }

        public override IDataReader GetDataByString(string condiotionlowercase, string str)
        {
            Debug.Log(CodistanTag + "Getting Location: " + str);

            IDbCommand dbcmd = GetDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_PHONE + " = '" + str + "'";
            return dbcmd.ExecuteReader();
        }

        public override void DeleteDataByString(string id)
        {
            Debug.Log(CodistanTag + "Deleting Location: " + id);

            IDbCommand dbcmd = GetDbCommand();
            dbcmd.CommandText =
                "DELETE FROM " + TABLE_NAME + " WHERE " + KEY_PHONE + " = '" + id + "'";
            dbcmd.ExecuteNonQuery();
        }

		public override void DeleteDataById(int id)
        {
            base.DeleteDataById(id);
        }

        public override void DeleteAllData()
        {
            Debug.Log(CodistanTag + "Deleting Table");

            base.DeleteAllData(TABLE_NAME);
        }

        public override IDataReader GetAllData()
        {
           return base.GetAllData(TABLE_NAME);
        }

        public List<UserEntity> GetAllUser()
        {
            List<UserEntity> entities = new List<UserEntity>();

            IDataReader reader = GetAllData();
            int fieldCount = reader.FieldCount;

            while (reader.Read())
            {
                UserEntity entity = new UserEntity();
                entity.name = reader[1].ToString();
                entity.email = reader[2].ToString();
                entity.phone = reader[3].ToString();
                entity.score = reader[4].ToString();
                entity.register_datetime = reader[5].ToString();
                entity.is_submitted = reader[6].ToString();

                entities.Add(entity);
            }

            return entities;
        }

        public List<UserEntity> GetAllUnSyncUser()
        {
            List<UserEntity> entities = new List<UserEntity>();

            IDbCommand dbcmd = GetDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_SYNC + " = 'N'";
           
            IDataReader reader = dbcmd.ExecuteReader(); 
            while (reader.Read())
            {
                UserEntity entity = new UserEntity();
                entity.name = reader[1].ToString();
                entity.email = reader[2].ToString();
                entity.phone = reader[3].ToString();
                entity.score = reader[4].ToString();
                entity.register_datetime = reader[5].ToString();
                entity.is_submitted = reader[6].ToString();

                entities.Add(entity);
            }

            return entities;
        }

        public void UpdateSyncUser(UserEntity userEntity)
        {
            List<string> col = new List<string>();
            List<string> con = new List<string>();

            col.Add("is_submitted");
            con.Add("Y");

            string condition = "email = '"+ userEntity.email + "'";

            UpdateData(col, con, condition);

        }

        public IDataReader GetLatestTimeStamp()
        {
            IDbCommand dbcmd = GetDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " ORDER BY id DESC LIMIT 1";

          /*  IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                Debug.Log(reader[3].ToString());
            }
            */
            return dbcmd.ExecuteReader();
        }
    }
}
