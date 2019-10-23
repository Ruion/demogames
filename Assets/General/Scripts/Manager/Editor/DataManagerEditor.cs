using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DataManager))]
public class DataManagerEditor : Editor
{
    DataManager dm;

    private void OnEnable()
    {
        dm = (DataManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Show Data"))
        {
            dm.GetAllPlayer();
        }

        if (GUILayout.Button("Clear Data"))
        {
            dm.ClearData();
        }

        if (GUILayout.Button("Load Master Setting"))
        {
            dm.LoadGameSettingFromMaster();
        }
    }
}
