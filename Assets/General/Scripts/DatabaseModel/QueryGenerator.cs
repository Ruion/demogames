using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Mono.Data.Sqlite;
using System.Data;

public class QueryGenerator : MonoBehaviour
{

    public DBModelEntity databaseModel;
    public string queryCondition;

    public enum QueryType { select = 1, update = 2, delete = 3 }

    public QueryType queryType = QueryType.select;

    [Button(ButtonSizes.Medium)]
    private void LogQuery()
    {
        
        DataRowCollection drc = databaseModel.ExecuteCustomSelectQuery("SELECT name FROM " + databaseModel.dbSettings.tableName);

        string log = "";

        foreach (DataRow r in drc)
        {
            log += r["name"] + "\n";
        }

        Debug.Log(log);

    }
}
