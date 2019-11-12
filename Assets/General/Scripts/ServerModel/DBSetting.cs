using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DBSetting
{
    // save the game setting file
    public static void SaveSetting(DBEntitySetting setting)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.streamingAssetsPath + "/" + setting.fileName + ".setting";
        FileStream stream = new FileStream(path, FileMode.Create);

        DBEntitySetting data = new DBEntitySetting(setting);


        try
        {
            formatter.Serialize(stream, data);
            stream.Close();
            Debug.Log("Save setting success");

        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    // load the game setting file
    public static DBEntitySetting LoadSetting(string fileName)
    {
        string path = Application.streamingAssetsPath + "/" + fileName + ".dbsetting";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            DBEntitySetting setting = (DBEntitySetting)formatter.Deserialize(stream);
            stream.Close();

            return setting;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}

[System.Serializable]
public class DBEntitySetting
{
    public string fileName = "db";
    public string sendURL;
    public LocalDBSetting localDbSetting;

    public DBEntitySetting(DBEntitySetting setting)
    {
        fileName = setting.fileName;
        sendURL = setting.sendURL;
        localDbSetting = setting.localDbSetting;
    }
}

[System.Serializable]
public class LocalDBSetting
{
    public string dbName;
    public string tableName;
    public List<string> columns;
    public List<string> attributes;
}