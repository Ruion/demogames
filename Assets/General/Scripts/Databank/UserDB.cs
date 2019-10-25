using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data.Common;

namespace DataBank{

	public class UserDB : SqliteHelper {
        #region variables
        private const string CodistanTag = "Codistan: UserDB:\t";
        
        private string TABLE_NAME = "user";
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

        public List<string> columns = new List<string>();
        #endregion

        //  private string[] COLUMNS = new string[] {KEY_ID, KEY_TYPE, KEY_LAT, KEY_LNG, KEY_DATE};

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
        * 9. voucher_id
        * 10. register_datetime
        */

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
                KEY_CONTACT + " TEXT UNIQUE, " +
                KEY_AGE + " TEXT , " +
                KEY_DOB + " TEXT , " +
                KEY_GENDER + " TEXT , " +
                KEY_GAME_RESULT + " TEXT , " +
                KEY_GAME_SCORE + " TEXT, " +
                KEY_VOUCHER_ID + " TEXT, " +
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
                "INSERT INTO " + TABLE_NAME
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
                + KEY_CONTACT + ", "
                + KEY_AGE + ", "
                + KEY_DOB + ", "
                + KEY_GENDER + ", "
                + KEY_GAME_RESULT + ", "
                + KEY_GAME_SCORE + ", "
                + KEY_VOUCHER_ID + ", "
                + KEY_DATE + ", "
                + KEY_SYNC  + " ) "

                + "VALUES ( '"
                + user.name + "', '"
                + user.email + "', '"
                + user.contact + "', '"
                + user.age + "', '"
                + user.dob + "', '"
                + user.gender + "', '"
                + user.game_result + "', '"
                + user.game_score + "', '"
                + user.voucher_id + "', '"
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

        public string UpdateLastDataScore(string score)
        {
           IDataReader lastReader =  GetLatestTimeStamp();
            UserEntity lastEntity = new UserEntity();

            while (lastReader.Read())
            {
                lastEntity.contact = lastReader[3].ToString();
                lastEntity.game_score = lastReader[4].ToString();
            }

            IDbCommand dbcmd2 = GetDbCommand();
            dbcmd2.CommandText = "UPDATE " + TABLE_NAME + " SET " + KEY_GAME_SCORE + " = '" + score + "' WHERE " + KEY_CONTACT + " = '" + lastEntity.contact + "' ;";

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

        public override IDataReader GetDataByString(string conditionlowercase, string str)
        {
            Debug.Log(CodistanTag + "Getting data: " + str);

            IDbCommand dbcmd = GetDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " WHERE " + conditionlowercase + " = '" + str + "'";
            return dbcmd.ExecuteReader();
        }

        public override void DeleteDataByString(string id)
        {
            Debug.Log(CodistanTag + "Deleting Location: " + id);

            IDbCommand dbcmd = GetDbCommand();
            dbcmd.CommandText =
                "DELETE FROM " + TABLE_NAME + " WHERE " + KEY_CONTACT + " = '" + id + "'";
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
                entity.contact = reader[3].ToString();
                entity.age = reader[4].ToString();
                entity.dob = reader[5].ToString();
                entity.gender = reader[6].ToString();
                entity.game_result = reader[7].ToString();
                entity.game_score = reader[8].ToString();
                entity.voucher_id = reader[9].ToString();
                entity.register_datetime = reader[10].ToString();
                entity.is_submitted = reader[11].ToString();

                entities.Add(entity);
            }

            return entities;
        }

        public List<UserEntity> GetAllUserEmailAndContact()
        {
            List<UserEntity> entities = new List<UserEntity>();

            IDbCommand dbcmd = GetDbCommand();

            string command = "SELECT email, contact FROM " + TABLE_NAME;

            Debug.Log(command);
            dbcmd.CommandText = command;
            
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                UserEntity entity = new UserEntity();
                entity.email = reader[0].ToString();
                entity.contact = reader[1].ToString();
                entities.Add(entity);
            }

            return entities;
        }


        public List<UserEntity> GetAllUnSyncUser()
        {
            List<UserEntity> entities = new List<UserEntity>();

            IDbCommand dbcmd = GetDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_SYNC + " = 'false'";
           
            IDataReader reader = dbcmd.ExecuteReader(); 
            while (reader.Read())
            {
                UserEntity entity = new UserEntity();
                entity.name = reader[1].ToString();
                entity.email = reader[2].ToString();
                entity.contact = reader[3].ToString();
                entity.age = reader[4].ToString();
                entity.dob = reader[5].ToString();
                entity.gender = reader[6].ToString();
                entity.game_result = reader[7].ToString();
                entity.game_score = reader[8].ToString();
                entity.voucher_id = reader[9].ToString();
                entity.register_datetime = reader[10].ToString();
                entity.is_submitted = reader[11].ToString();

                entities.Add(entity);
            }

            return entities;
        }

        public void UpdateSyncUser(UserEntity userEntity)
        {
            List<string> col = new List<string>();
            List<string> con = new List<string>();

            col.Add("is_submitted");
            con.Add("true");

            string condition = KEY_EMAIL + " = '"+ userEntity.email + "'";

            try { UpdateData(col, con, condition); Debug.Log("Update user " + userEntity.email + " to submitted"); }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);
            }
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
