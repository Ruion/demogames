/*
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StockDBModelEntity), true)]
public class StockDBModelEditor : DBModelMasterEditor
{
    public override void OnInspectorGUI()
    {
        StockDBModelEntity s;

        base.OnInspectorGUI();

        s= (StockDBModelEntity)target;

        if (GUILayout.Button("Drop"))
        {
            s.GiveReward();
        }
    }
}
*/