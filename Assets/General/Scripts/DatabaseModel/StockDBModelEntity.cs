using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Data;
using UnityEngine.UI;
using TMPro;

public class StockDBModelEntity : DBModelEntity
{
    #region fields

    [Header("Handler")]
    public GameObject emptyHandler;
    public GameObject errorHandler;

    [Header("Stock setting")]

    public VendingMachine vm;

    #endregion

    DataRowCollection rows;
    [SerializeField] int item_id;
    [SerializeField] string item_name;
    [SerializeField] int item_quantity;
    [SerializeField] int item_lane;
    [SerializeField] private int laneOccupyPerItem = 1;
    [SerializeField] private int quantityPerLane;

    public UnityEvent OnOutOfStock;
    public UnityEvent OnStockGiven;
    public TextMeshProUGUI text_;

    protected override void OnEnable()
    {
        base.OnEnable();
        HideAllHandler();
        ReloadUI();
    }

    [ContextMenu("HideHandler")]
    public void HideAllHandler()
    {
        emptyHandler.SetActive(false);
        errorHandler.SetActive(false);
    }

    #region UI
    [ContextMenu("ReloadUI")]
    public void ReloadUI()
    {
        HideAllHandler();
        LoadSetting();
    }

    #endregion

    public virtual void GetItem()
    {
        LoadSetting();

        string query = "SELECT * FROM " + dbSettings.tableName + " WHERE " + selectCustomCondition;
        DataRowCollection drc = ExecuteCustomSelectQuery(query);

        if (drc.Count < 1)
        {
            // out of stock
            Debug.LogError("out of stock");
            if (OnOutOfStock.GetPersistentEventCount() > 0) OnOutOfStock.Invoke();
            return;
        }
        else
        {

            item_id = (int)drc[0][0];
            item_name = drc[0][1].ToString();
            item_quantity = int.Parse(drc[0][2].ToString());
            item_lane = int.Parse(drc[0][3].ToString()); ;
            
        }
    }

    [ContextMenu("GiveReward")]
    public void GiveReward()
    {
        GetItem();

        if (item_quantity < 1) return;

        ConnectDb();

       try
        {
            vm.SendToPort(item_lane);
            if (OnStockGiven.GetPersistentEventCount() > 0) OnStockGiven.Invoke();

            item_quantity--;

            List<string> col = new List<string>();
            List<string> val = new List<string>();

            col.Add("quantity");
            val.Add(item_quantity.ToString());

            UpdateData(col, val, "id = " + item_id );

            if (item_quantity < 1)
            {
                col = new List<string>();
                val = new List<string>();

                col.Add("is_disabled");
                val.Add("true");

                UpdateData(col, val, "lane = '" + item_lane + "'");
            }

            Debug.Log(item_id + " : " + item_name + " has " + item_quantity + " left | Lane : " + item_lane);
            if (text_ != null) text_.text = item_id + " : " + item_name + " has " + item_quantity + " left";

            if (transform.GetChild(0).gameObject.activeInHierarchy) ReloadUI();

            Close();
        }
       catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        
    }

    #region Save
    public void SaveStockMultiple()
    {
        List<string> col = new List<string>();
        List<string> val = new List<string>();

        for (int v = 0; v < dbSettings.columns.Count; v++)
        {
            col.Add(dbSettings.columns[v].name);
        }

        try
        {
            //for (int i = 0; i < numberToPopulate * laneOccupyPerItem; i++)
            for (int i = 0; i < numberToPopulate; i++)
            {
                string name = "Motor_" + (i.ToString());
                string quantity = quantityPerLane.ToString();
                string lane = i.ToString();
                string is_disabled = "true";

               // if (i % (laneOccupyPerItem) == 0) { is_disabled = "false"; Debug.Log(i + "% " + laneOccupyPerItem + " is " + i % laneOccupyPerItem); }
               // if (i == 0) is_disabled = "false";

                val = new List<string>();
                val.Add(i.ToString());
                val.Add(name);
                val.Add(quantity);
                val.Add(lane);
                val.Add(is_disabled); 

                AddData(col, val);
            }
        }
        catch(System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    public override void Populate()
    {
        CreateTable();
        ConnectDb();
        SaveStockMultiple();
        TestIndex++;
    }
    #endregion


}
