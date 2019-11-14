using Mono.Data.Sqlite;
using UnityEngine;
using System.Data;
using System.Data.Common;

namespace DataBank
{
    public class SqliteHelper
    {
        private const string CodistanTag = "Codistan: SqliteHelper:\t";

        public string database_name = "user_db";

        public string db_connection_string;
        public IDbConnection db_connection;

        public SqliteHelper()
        {
            
        }

        ~SqliteHelper()
        {
            db_connection.Close();
        }

        public virtual void ConnectDb(string db_name)
        {
            db_connection_string = "URI = file:" + Application.streamingAssetsPath + "/" + db_name;
            db_connection = new SqliteConnection(db_connection_string);
            db_connection.Open();
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
                return reader;
            }
            catch(DbException ex)
            {
                Debug.Log("Error : " + ex.Message);
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

		public virtual void Close (){
			db_connection.Close ();
		}
    }
}