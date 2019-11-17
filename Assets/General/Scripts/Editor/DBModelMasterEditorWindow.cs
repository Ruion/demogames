#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Data;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;

public class DBModelMasterEditorWindow : OdinEditorWindow
{
    private DBModelMaster db;

    [ReadOnly]
    public string[,] line;
    private int testIndex;

    [MenuItem("Tools/Local DB")]
    public static void OpenWindow()
    {
        var window = GetWindow<DBModelMasterEditorWindow>();

        // Nifty little trick to quickly position the window in the middle of the editor.
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(500, 600);

    }

    [Button(ButtonSizes.Medium)]
    public void Refresh()
    {
        DataTable dt = db.GetAllDataInToDataTable();
        
        line = new string[dt.Columns.Count,dt.Rows.Count];

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            for (int c = 0; c < dt.Columns.Count; c++)
            {
                line[c,i] += dt.Rows[i][c];
            }
        }
    }

    protected override IEnumerable<object> GetTargets()
    {

        if (Selection.activeGameObject == null) Close();

        db = Selection.activeGameObject.GetComponent<DBModelMaster>();

        if(testIndex != db.TestIndex)
        {
            Refresh();
            testIndex = db.TestIndex;
        }

        // Draws this instance using Odin
        yield return this;

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        line = new string[0, 0];
    }
}
#endif
