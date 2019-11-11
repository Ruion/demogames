using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DBSettingEntity), true)]
public class DBSettingEntityEditor : Editor
{
    DBSettingEntity db;

    public override void OnInspectorGUI()
    {
        db = (DBSettingEntity)target;

        base.OnInspectorGUI();

        if (GUILayout.Button("Save Setting"))
        {
            db.SaveSetting();
        }

        if (GUILayout.Button("Load Setting"))
        {
            db.LoadSetting();
        }

        if (GUILayout.Button("Load Master Setting"))
        {
            db.LoadDBSettingFromMaster();
        }

    }
}