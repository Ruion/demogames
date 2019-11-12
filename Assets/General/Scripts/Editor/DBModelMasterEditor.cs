using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DBModelMaster), true)]
public class DBModelMasterEditor : DBSettingEntityEditor { 
    DBModelMaster db;


    public override void OnInspectorGUI()
    {
        db = (DBModelMaster)target;

        base.OnInspectorGUI();

        if (GUILayout.Button("Populate"))
        {
            db.Populate();
        }

        if (GUILayout.Button("GetAll"))
        {
            db.GetAllDataInToDataTable();
        }

        if (GUILayout.Button("GetAllUnSync"))
        {
            db.GetAllDataInToDataTable();
        }

        if (GUILayout.Button("Delete"))
        {
            db.DeleteAllData();
        }

        if (GUILayout.Button("Sync"))
        {
            db.Sync();
        }

    }
}
