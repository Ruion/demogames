using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public static class GameSetting
{
    // save the game setting file
    public static void SaveSetting(Settings setting)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.streamingAssetsPath + "/" + setting.fileName + ".setting";
        FileStream stream = new FileStream(path, FileMode.Create);

        Settings data = new Settings(setting);

        try
        {
            formatter.Serialize(stream, data);
            stream.Close();
            Debug.Log("Save setting success");

        }catch(System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    // load the game setting file
    public static Settings LoadSetting(string fileName)
    {
        string path = Application.streamingAssetsPath + "/" + fileName + ".setting";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Settings setting = (Settings)formatter.Deserialize(stream);
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
public class Settings
{
    public string fileName = "game_setting";
    public string DbName { get { return sQliteDBSettings.dbName; } }
    public string TableName { get { return sQliteDBSettings.tableName; } }
    public string scoreName = "game_score";
    public string serverAddress = "http://192.168.0.28/honda/submit-data.php";
    public string serverGetDataAddress = "http://192.168.0.28/honda/submit-data.php";
    public int scoreToWin = 3;
    public string userTypeName;

    public RewardType rewardType = RewardType.PrintVoucher;

    public SQliteDBSettings sQliteDBSettings;

    [Header("Vending Machine setting, Ignore if not involve vending machine")]
    public VendingStockSettings vendingStockSettings;

    

    public Settings(Settings setting)
    {
        fileName = setting.fileName;
        scoreName = setting.scoreName;
        serverAddress = setting.serverAddress;
        serverGetDataAddress = setting.serverGetDataAddress;
        scoreToWin = setting.scoreToWin;
        userTypeName = setting.userTypeName;

        // stock settings
        vendingStockSettings = setting.vendingStockSettings;

        sQliteDBSettings = setting.sQliteDBSettings;
}

    public Settings(){  }
}

[System.Serializable]
public enum WriteType
{
    Byte,
    String
}

[System.Serializable]
public class SQliteDBSettings
{
    public string dbName = "db_";
    public string tableName = "registration";
    public string UniversalUserClassName = "DataBank.";
    public List<string> columns;
    public List<string> attributes;
    public List<string> columnsToSkipWhenSync;
}

[System.Serializable]
public class VendingStockSettings
{
    public string stockDbName = "db_stock";
    public string stockTableName = "table_stock";
    public WriteType VMSerialPortWriteType = WriteType.Byte;
    public string vmserialPortText;
    public PortName portname = PortName.COM1;
    public PortBaudrate portbaudrate = PortBaudrate.pb115200;
    public int numberToPopulate = 35;
    public int numberPerLane = 3;
    public int laneOccupyPerMotor = 1;
}