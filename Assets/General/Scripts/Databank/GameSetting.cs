using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
    public string dbName = "user_db";
    public string scoreName = "score";
    public string tableName = "user";
    public string serverAddress = "http://192.168.0.28/honda/submit-data.php";
    public string serverGetDataAddress = "http://192.168.0.28/honda/submit-data.php";

    public RewardType rewardType = RewardType.PrintVoucher;

    [Header("Vending Machine setting, Ignore if not involve vending machine")]
    public string stockDbName = "http://192.168.0.28/honda/submit-data.php";
    public string stockTableName = "http://192.168.0.28/honda/submit-data.php";
    public WriteType VMSerialPortWriteType = WriteType.Byte;
    public string vmserialPortText;
    public PortName portname = PortName.COM1;
    public PortBaudrate portbaudrate = PortBaudrate.pb115200;
    public int numberToPopulate = 35;
    public int numberPerLane = 3;
    public int laneOccupyPerMotor = 1;

    public Settings(Settings setting)
    {
        fileName = setting.fileName;
        dbName = setting.dbName;
        scoreName = setting.scoreName;
        tableName = setting.tableName;
        serverAddress = setting.serverAddress;
        serverGetDataAddress = setting.serverGetDataAddress;
        stockDbName = setting.stockDbName;
        stockTableName = setting.stockTableName;
        portname = setting.portname;
        portbaudrate = setting.portbaudrate;
        VMSerialPortWriteType = setting.VMSerialPortWriteType;
        vmserialPortText = setting.vmserialPortText;
        rewardType = setting.rewardType;

        // stock settings
        numberToPopulate = setting.numberToPopulate;
        numberPerLane = setting.numberPerLane;
        laneOccupyPerMotor = setting.laneOccupyPerMotor;
}

    public Settings(){  }
}

[System.Serializable]
public enum WriteType
{
    Byte,
    String
}