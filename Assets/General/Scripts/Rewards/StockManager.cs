using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Networking;
using DataBank;
using TMPro;

public class StockManager : MonoBehaviour {

    #region fields

    public List<Stock> stockList;

    [Header("Handler")]
    public GameObject emptyHandler;
    public GameObject successSendDataHandler;
    public GameObject blockDataHandler;
    public GameObject stockNotSaveHandler;

    public int numberToPopulate;
    public int numberPerLane;

    [Header("GameSetting - SAVE setting every new project")]
    public string settingFileName = "game";
    public string dbName = "user_db";
    public string scoreName = "score";
    public string tableName = "user";
    public string serverAddress = "http://192.168.0.28/honda/submit-data.php";
    public string serverGetDataAddress = "http://192.168.0.28/honda/submit-data.php";
    public string stockDbName = "u665541817_eys10";
    public string stockTableName = "stock";

    #endregion

    private void Awake()
    {
        LoadSetting();
    }

    #region setting
    public void LoadSetting()
    {
        string filepath = settingFileName;
        Settings setting = GameSetting.LoadSetting(filepath);

        dbName = setting.dbName;
        scoreName = setting.scoreName;
        tableName = setting.tableName;
        serverAddress = setting.serverAddress;
        serverGetDataAddress = setting.serverGetDataAddress;
        stockDbName = setting.stockDbName;
        stockTableName = setting.stockTableName;
    }

    
    [ContextMenu("SaveSetting")]
    public void SaveSetting()
    {
        Settings setting = new Settings();
        setting.fileName = settingFileName;
        setting.dbName = dbName;
        setting.scoreName = scoreName;
        setting.tableName = tableName;
        setting.serverAddress = serverAddress;
        setting.serverGetDataAddress = serverGetDataAddress;
        setting.stockDbName = stockDbName;
        setting.stockTableName = stockTableName;

        GameSetting.SaveSetting(setting);
    }
    #endregion

    private void Start()
    {
        HideAllHandler();

        stockList = new List<Stock>();
        StockDB stockDb = new StockDB(stockDbName, stockTableName);
        stockList = stockDb.GetAllStock();
        stockDb.Close();
    }

    [ContextMenu("HideHandler")]
    public void HideAllHandler()
    {
        emptyHandler.SetActive(false);
        successSendDataHandler.SetActive(false);
    }

    [ContextMenu("Get All stock")]
    public void GetAllStock()
    {
        LoadSetting();
        StockDB stockDb = new StockDB(stockDbName, stockTableName);
        List<Stock> entities = stockDb.GetAllStock();
        stockDb.Close();

        if (entities.Count < 1)
        {
            Debug.LogError("no data in record");
            return;
        }

        int n = 0;
        foreach (Stock e in entities)
        {
            n++;
            Debug.Log(n + " ID is " + e.ID);
            Debug.Log(n + " lane is " + e.lane);
            Debug.Log(n + " number is " + e.number);
            Debug.Log(n + " isSubmitted is " + e.isDisabled);
        }
    }

    [ContextMenu("Get All stock")]
    public void GetAllStock()
    {
        LoadSetting();
        StockDB stockDb = new StockDB(stockDbName, stockTableName);
        List<Stock> entities = stockDb.GetAllStock();
        stockDb.Close();

        if (entities.Count < 1)
        {
            Debug.LogError("no data in record");
            return;
        }

        int n = 0;
        foreach (Stock e in entities)
        {
            n++;
            Debug.Log(n + " ID is " + e.ID);
            Debug.Log(n + " lane is " + e.lane);
            Debug.Log(n + " number is " + e.number);
            Debug.Log(n + " isSubmitted is " + e.isDisabled);
        }
    }

    public void ClearData()
    {
        StockDB stockDb = new StockDB(stockDbName, stockTableName);
        stockDb.DeleteAllData();
        stockDb.Close();
    }

    #region Save Data

    public void SaveStock(Stock stock)
    {
        StockDB stockDb = new StockDB(stockDbName, stockTableName);

        string msg = stockDb.AddData(stock);

        stockDb.Close();

        if (msg != "true")
        {
            stockNotSaveHandler.SetActive(true);
        }
        else
        {
            Debug.Log("insert succeed");
        }

    }

    public void SaveStockMultiple(int amount, int numberPerLane)
    {
        StockDB stockDb = new StockDB(stockDbName, stockTableName);
        stockDb.DeleteAllData();

        for (int i = 0; i < amount; i++)
        {
            Stock stock = new Stock();
            stock.lane = "Motor_" + i.ToString();
            stock.number = numberPerLane;
            stock.isDisabled = "false";

            string msg = stockDb.AddData(stock);
            if (msg != "true")
            {
                stockNotSaveHandler.SetActive(true);
                break;
            }
        }

        stockDb.Close();
    }

    #endregion



    public void Populate()
    {
        SaveStockMultiple(numberToPopulate, numberPerLane);
    }
}
