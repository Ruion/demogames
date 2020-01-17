using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockChecker : MonoBehaviour
{
    public VendingMachineDBModelEntity sDb;
    public GameObject outOfStockMessage;
    private int stockRemainAmount;

    private void OnEnable()
    {
        InvokeRepeating("CheckStock", 1f, 2f);
    }

    void CheckStock()
    {
        stockRemainAmount = System.Int32.Parse(sDb.ExecuteCustomSelectObject("SELECT SUM(quantity) FROM " + sDb.dbSettings.tableName + " WHERE item_limit > 0").ToString());
        Debug.Log(name + " - CheckStock() : Vending Machine item remain " + stockRemainAmount);

        // if out of stock
        if (stockRemainAmount < 1)
            outOfStockMessage.SetActive(true);

        else outOfStockMessage.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}
