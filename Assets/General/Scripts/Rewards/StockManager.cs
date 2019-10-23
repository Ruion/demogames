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

    [ReadOnly]
    public List<Stock> stockList;
    [ReadOnly]
    public List<Stock> availableStockList;

    [Header("Handler")]
    public GameObject emptyHandler;
    public GameObject successSendDataHandler;
    public GameObject blockDataHandler;
    public GameObject stockNotSaveHandler;

    [Header("Stock setting")]
    public int numberToPopulate = 35;
    public int numberPerLane = 3;
    public int availableLanePerJump = 1;
    public bool resetOnPopulate = true;
    private StockDB stockDb;

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
        stockDb = new StockDB(stockDbName, stockTableName);
        if (stockDb.GetAllStock().Count < 1) Populate(); // create stock list if no list exist
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
        stockDb = new StockDB(stockDbName, stockTableName);
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
        List<Stock> entities = stockDb.GetAllStock();
        stockDb.Close();

        if (entities.Count < 1)
        {
            Debug.LogError("empty in stock");
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
        stockDb = new StockDB(stockDbName, stockTableName);
        stockDb.DeleteAllData();
        stockDb.Close();
    }

    #region Save Data

    public void SaveStock(Stock stock)
    {
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

    public void SaveStockMultiple(int amount, int numberPerLane, int availableLanePerJump_ = 1  )
    {
        stockDb.DeleteAllData();

        for (int i = 1; i < amount+1; i++)
        {
            Stock stock = new Stock();
            stock.lane = "Motor_" + i.ToString();
            stock.number = numberPerLane;

           if(availableLanePerJump_ % i == 0) stock.isDisabled = "true";
           if(i == 1) stock.isDisabled = "false";

            string msg = stockDb.AddData(stock);
            if (msg != "true")
            {
                stockNotSaveHandler.SetActive(true);
                break;
            }
        }

        stockDb.Close();
    }

    private void UpdateStock(Stock stock)
    {
        stockDb.UpdateStock(stock);
    }

    #endregion

    #region Get stock

    public List<Stock> GetAvailableStocks()
    {
        List<Stock> stocks = stockDb.GetAllAvailable();

        if(stocks.Count < 1)
        {
            Debug.Log("empty stock");
            emptyHandler.SetActive(true);
            return stocks;
        }

        foreach (var s in stocks)
        {
            Debug.Log(s.ID + " stock is " + s.isDisabled);
        }

        return stocks;
    }

    public Stock GetAvailableStock()
    {
        availableStockList = GetAvailableStocks();

        return availableStockList[0];
    }

    #endregion

    public void DropGift() 
    {

        Stock stock = GetAvailableStock();

        if (stock == null) return;

        Debug.Log(stock.lane + " stock have " + stock.number + " left");

        stock.number--;

        if(stock.number == 0) { stock.isDisabled = "true";  Debug.Log("change motor on next drop"); } // if no more gift at that lane, disabled it in database

        UpdateStock(stock);
    }
   

    public void Populate()
    {
        Debug.Log("Creating stock list");
        SaveStockMultiple(numberToPopulate, numberPerLane, availableLanePerJump);
        GetAllStock();
    }
}
