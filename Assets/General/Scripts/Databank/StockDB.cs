using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data.Common;

namespace DataBank
{

    public class StockDB : SqliteHelper
    {
        #region variables
        private const string CodistanTag = "Codistan: UserDB:\t";

        private string TABLE_NAME = "stock";
        private string KEY_ID = "ID";
        private string KEY_LANE = "lane";
        private string KEY_NUMBER = "number";
        private string KEY_ISDISABLED = "isDisabled";


        public List<string> columns = new List<string>();
        #endregion

        //  private string[] COLUMNS = new string[] {KEY_ID, KEY_TYPE, KEY_LAT, KEY_LNG, KEY_DATE};

        public StockDB(string db_name, string table_name) : base()
        {
            db_connection_string = "URI = file:" + Application.streamingAssetsPath + "/" + db_name;
            db_connection = new SqliteConnection(db_connection_string);
            db_connection.Open();

            TABLE_NAME = table_name;

            IDbCommand dbcmd = GetDbCommand();

            // columns for table

            dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " ( " +
                KEY_ID + " INTEGER PRIMARY KEY, " +
                KEY_LANE + " TEXT NOT NULL, " +
                KEY_NUMBER + " INTEGER NOT NULL, " +
                KEY_ISDISABLED + " TEXT NOT NULL, " +
            dbcmd.ExecuteNonQuery();

            return;
        }

        public string AddData(List<string> columns_, List<string> values_)
        {
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

        public string AddData(Stock stock)
        {
            IDbCommand dbcmd = GetDbCommand();
            dbcmd.CommandText =
                "INSERT INTO " + TABLE_NAME
                + " ( "
                + KEY_ID + ", "
                + KEY_LANE + ", "
                + KEY_NUMBER + ", "
                + KEY_ISDISABLED + " ) "

                + "VALUES ( '"
                + stock.ID + "', '"
                + stock.lane + "', '"
                + stock.number + "', '"
                + stock.isDisabled + "' )";

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
                if (values_.IndexOf(v) != values_.Count - 1) dbcmd2.CommandText += v + "' ,";

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

        public void UpdateStock(Stock stock)
        {
            /*
            UPDATE table_name
            SET column1 = value1, column2 = value2...., columnN = valueN
            WHERE[condition];
            */

            IDbCommand dbcmd2 = GetDbCommand();
            dbcmd2.CommandText =
                "UPDATE " + TABLE_NAME
                + " SET "
                + KEY_NUMBER + " = " + stock.number + " , "
                + KEY_LANE + " = '" + stock.lane + "' , "
                + KEY_ISDISABLED + " = '" + stock.isDisabled + "' WHERE "
                + KEY_ID + " = " + stock.ID +" ;";

            try { dbcmd2.ExecuteNonQuery(); Debug.Log("Updated stock with ID " + stock.ID); }
            catch (DbException ex)
            {
                string msg = string.Format("ErrorCode: {0}", ex.Message);
                Debug.Log(msg);
            }
        }

        #region Delete
        public override void DeleteDataByString(string id)
        {
            Debug.Log(CodistanTag + "Deleting Location: " + id);

            IDbCommand dbcmd = GetDbCommand();
            dbcmd.CommandText =
                "DELETE FROM " + TABLE_NAME + " WHERE " + KEY_ID + " = '" + id + "'";
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

        #endregion

        #region Get
        public override IDataReader GetAllData()
        {
            return base.GetAllData(TABLE_NAME);
        }

        public List<Stock> GetAllStock()
        {
            List<Stock> entities = new List<Stock>();

            IDataReader reader = GetAllData();
            int fieldCount = reader.FieldCount;

            while (reader.Read())
            {
                Stock entity = new Stock();
                entity.ID = (int)reader[0];
                entity.lane = reader[1].ToString();
                entity.number = (int)reader[2];
                entity.isDisabled = reader[3].ToString();

                entities.Add(entity);
            }

            return entities;
        }

        public List<Stock> GetAllAvailable()
        {
            List<Stock> entities = new List<Stock>();

            /*
            IDbCommand dbcmd = GetDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_ISDISABLED + " = 'No'";

            IDataReader reader = dbcmd.ExecuteReader();
            */

            IDataReader reader = GetDataByString(KEY_ISDISABLED, "false");

            while (reader.Read())
            {
                Stock entity = new Stock();
                entity.ID = (int)reader[0];
                entity.lane = reader[1].ToString();
                entity.number = (int)reader[2];
                entity.isDisabled = reader[3].ToString();

                entities.Add(entity);
            }

            return entities;
        }

        public override IDataReader GetDataById(int id)
        {
            return base.GetDataById(id);
        }

        public override IDataReader GetDataByString(string conditionlowercase, string str)
        {
            Debug.Log(CodistanTag + "Getting Location: " + str);

            IDbCommand dbcmd = GetDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " WHERE " + conditionlowercase + " = '" + str + "'";
            return dbcmd.ExecuteReader();
        }

        #endregion


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
