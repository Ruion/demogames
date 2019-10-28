using UnityEditor;
using UnityEngine;
using DataBank;

[CustomEditor(typeof(UniversalUserDB))]
public class UniversalUserDBEditor : GameSettingEntityEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UniversalUserDB u = (UniversalUserDB)target;

        if (GUILayout.Button("GetAllData")){
            u.GetAllData(u.gameSettings.sQliteDBSettings.UniversalUserClassName);
        }

        if (GUILayout.Button("Populate"))
        {
            u.Populate();
        }

        if (GUILayout.Button("Clear Table"))
        {
            u.DeleteAllData(u.gameSettings.sQliteDBSettings.tableName);
        }
    }
}
