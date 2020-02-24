using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Sirenix.OdinInspector;

public class AppCommunicator : SerializedMonoBehaviour
{
    [Header("file.txt | db column name")]
    public DBModelEntity dbmodel;

    // file.txt , db column name
    public Dictionary<string, string> writeInfo = new Dictionary<string, string>();

    [Header("sourcePath | file.txt")]
    public List<string> copyInfo = new List<string>();

    [Button]
    public void WriteExistingDataToFile()
    {
        string appLaunchFilePath = Path.Combine(dbmodel.dbSettings.folderPath, "AppLaunchNumberFilePath.txt");

        // Get data file path from AppLaunchNumberFilePath.txt
        string[] dataFilePath = File.ReadAllLines(appLaunchFilePath);



        foreach (KeyValuePair<string,string> kvp in writeInfo)
        {
            string writePath = Path.Combine(Path.GetDirectoryName(dataFilePath[0]), kvp.Key);
            List<string> dataList = dbmodel.GetDataByStringToList(kvp.Value);

            File.WriteAllLines(writePath, dataList.ToArray());

        }
    }

    [Button]
    public void CopyFileToFile()
    {
        string appLaunchFilePath = Path.Combine(dbmodel.dbSettings.folderPath, "AppLaunchNumberFilePath.txt");

        // Get data file path from AppLaunchNumberFilePath.txt
        string[] dataFilePath = File.ReadAllLines(appLaunchFilePath);


        foreach (string s in copyInfo)
        {
            string writePath = Path.Combine(Path.GetDirectoryName(dataFilePath[0]), Path.GetFileName(s));

            File.Copy( s, writePath, true);

        }
    }
}
