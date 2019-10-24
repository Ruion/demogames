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

public class StockManager : RewardFeature {

    #region fields

    [ReadOnly]
    public List<Stock> stockList;
    [ReadOnly]
    public List<Stock> availableStockList;

    [Header("Handler")]
    public GameObject emptyHandler;
    public GameObject errorHandler;

    [Header("Stock setting")]
    public int numberOfMotor = 35;
    public int numberOfGiftPerMotor = 3;
    public int laneOccupyPerMotor = 1;
    public bool resetOnPopulate = true;
    public bool showAllOnPopulate = true;
    public bool showAvailableOnPopulate = true;
    private StockDB stockDb;
    public VendingMachine vm;

    #endregion

    public override void GiveReward()
    {
        DropGift();
    }

    private void Start()
    {
        stockDb = new StockDB(gameSettings.stockDbName, gameSettings.stockTableName);
        if (stockDb.GetAllStock().Count < 1) Populate(); // create stock list if no list exist
        if (vm == null) { vm = FindObjectOfType<VendingMachine>(); }

        HideAllHandler();

        stockList = new List<Stock>();
        stockDb = new StockDB(gameSettings.stockDbName, gameSettings.stockTableName);
        stockList = stockDb.GetAllStock();
        stockDb.Close();
    }

    private void SetUpDb()
    {
        stockDb = new StockDB(gameSettings.stockDbName, gameSettings.stockTableName);
    }

    [ContextMenu("HideHandler")]
    public void HideAllHandler()
    {
        emptyHandler.SetActive(false);
        errorHandler.SetActive(false);
    }

    [ContextMenu("Get All stock")]
    public void GetAllStock()
    {
        LoadSetting();
        SetUpDb();

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
        SetUpDb();
        stockDb.DeleteAllData();
        stockDb.Close();
    }

    #region Save Data

    public void SaveStock(Stock stock)
    {
        SetUpDb();
        string msg = stockDb.AddData(stock);

        stockDb.Close();

        if (msg != "true")
        {
            errorHandler.SetActive(true);
            errorHandler.GetComponentInChildren<TextMeshProUGUI>().text = "error" + msg;
        }
        else
        {
            Debug.Log("insert succeed");
        }

    }

    public void SaveStockMultiple(int amount, int numberPerLane, int laneOccupyPerMotor = 1  )
    {
        List<Stock> stocks = new List<Stock>();

        for (int i = 0; i < amount; i++)
        {
            Stock stock = new Stock();
            stock.ID = i+1;
            stock.lane = "Motor_" + (i.ToString());
            stock.number = numberPerLane;
            stock.isDisabled = "true";

            if (i % (laneOccupyPerMotor) == 0) { stock.isDisabled = "false"; Debug.Log(i + "% " + laneOccupyPerMotor + " is " + i % laneOccupyPerMotor); }
             if(i == 0) stock.isDisabled = "false";

            stocks.Add(stock);

            
            SetUpDb();
            string msg = stockDb.AddData(stock);

            if (msg != "true")
            {
                Debug.LogError("Failed at " + i + "th saving");
                break;
            }

            stockDb.Close();
            
        }
    }

    private void UpdateStock(Stock stock)
    {
        SetUpDb();
        stockDb.UpdateStock(stock);
        stockDb.Close();
    }

    #endregion

    #region Get stock

    public List<Stock> GetAvailableStocks()
    {
        SetUpDb();
        List<Stock> stocks = stockDb.GetAllAvailable();

        if(stocks.Count < 1)
        {
            Debug.Log("empty stock");
            emptyHandler.SetActive(true);
            return stocks;
        }

        stockDb.Close();

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

        if (stock == null) { Debug.LogError("out of stock"); return; }

        try
        {
            vm.SendToPort(stock.ID - 1);
        }
        catch(System.Exception ex)
        {
            Debug.LogError("Vending Machine error " +ex.Message);
            return;
        }

        stock.number--;

        Debug.Log(stock.lane + " drop 1 gift, " + stock.lane + " stock have " + stock.number + " left");  

        if(stock.number == 0) { stock.isDisabled = "true";  Debug.Log("change motor on next drop"); } // if no more gift at that lane, disabled it in database

        UpdateStock(stock);

    }
   
    public void Populate()
    {
        Debug.ClearDeveloperConsole();

        LoadSetting();

        if (resetOnPopulate) ClearData();

        SaveStockMultiple(numberOfMotor, numberOfGiftPerMotor, laneOccupyPerMotor);

        if(showAllOnPopulate) GetAllStock();
        if(showAvailableOnPopulate) GetAvailableStocks();
        
    }
}
