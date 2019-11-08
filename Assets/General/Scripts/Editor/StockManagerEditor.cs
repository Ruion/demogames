using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DataBank;

[CustomEditor(typeof(StockManager))]
public class StockManagerEditor : GameSettingEntityEditor
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

        if (GUILayout.Button("Get available stock"))
        {
           List<Stock> stocks = sm.GetAvailableStocks();
            foreach (var s in stocks)
            {
                Debug.Log(s.ID + "th stock has "+ s.number + " is available");
            }
        }

        if (GUILayout.Button("Drop Gift"))
        {
            sm.DropGift();
        }
    }
}
