using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DataBank;

[CustomEditor(typeof(StockManager))]
public class StockManagerEditor : Editor
{
    StockManager sm;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        sm = (StockManager)target;

        if (GUILayout.Button("Populate"))
        {
            sm.Populate();
        }

        if (GUILayout.Button("Clear stock"))
        {
            sm.ClearData();
        }
    }
}
