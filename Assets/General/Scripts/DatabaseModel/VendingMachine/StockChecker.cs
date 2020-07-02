using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StockChecker : MonoBehaviour
{
    public VendingMachineDBModelEntity sDb;
    public GameObject outOfStockMessage;
    private int stockRemainAmount;

    public UnityEvent onOutOfStock;
    public UnityEvent onStockPass;

    private void OnEnable()
    {
        InvokeRepeating("CheckStock", 1f, 2f);
    }

    private void CheckStock()
    {
        stockRemainAmount = System.Int32.Parse(sDb.ExecuteCustomSelectObject("SELECT SUM(quantity) FROM " + sDb.dbSettings.tableName + " WHERE item_limit > 0").ToString());
        Debug.Log(name + " - CheckStock() : Vending Machine item remain " + stockRemainAmount);

        // if out of stock
        if (stockRemainAmount < 1)
        {
            outOfStockMessage.SetActive(true);
            if (onOutOfStock.GetPersistentEventCount() > 0) onOutOfStock.Invoke();
        }
        else
        {
            outOfStockMessage.SetActive(false);
            if (onStockPass.GetPersistentEventCount() > 0) onStockPass.Invoke();
        }
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}