using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Data;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manipulate database for Vending Machine, use with VendingMachine.cs to drop gift
/// Tips: call GiveReward() to drop gift from vending machine
/// </summary>
public class StockDBModelEntity : DBModelEntity
{
    #region fields

    [Header("Handler")]
    public GameObject stockEmptyHandler;
    public GameObject stockErrorHandler;

    [Header("Stock setting")]

    public VendingMachine vm;

    

    DataRowCollection rows;
    [SerializeField] int item_id;
    [SerializeField] string item_name;
    [SerializeField] int item_quantity;
    [SerializeField] string item_lane;
    //[SerializeField] private int laneOccupyPerItem = 1;
    [SerializeField] private int quantityPerLane = 2;

    public UnityEvent OnOutOfStock;
    public UnityEvent OnStockGiven;
#endregion

    protected override void OnEnable()
    {
        base.OnEnable();
        HideAllHandler();
    }

    [ContextMenu("HideHandler")]
    public new void HideAllHandler()
    {
        stockEmptyHandler.SetActive(false);
        stockErrorHandler.SetActive(false);
    }

    public virtual void GetItem()
    {
        HideAllHandler();
        LoadSetting();

        string query = "SELECT * FROM " + dbSettings.tableName + " WHERE " + selectCustomCondition;
        DataRowCollection drc = ExecuteCustomSelectQuery(query);

        if (drc.Count < 1)
        {
            // out of stock
            Debug.LogError("out of stock");
            if (OnOutOfStock.GetPersistentEventCount() > 0) {OnOutOfStock.Invoke(); emptyHandler.SetActive(true);}
            return;
        }
        else
        {

            item_id = (int)drc[0][0];
            item_name = drc[0][1].ToString();
            item_quantity = int.Parse(drc[0][2].ToString());
            item_lane = drc[0][3].ToString() ;
            
        }
    }

    [ContextMenu("GiveReward")]
    public void GiveReward()
    {
        GetItem();

        if (item_quantity < 1) return;

        ConnectDb();

     //  try
     //   {
            // vm.SendToPort(item_lane); int

/*
            #region testing

            byte[] SendingBytes = new byte[]{ 0x01, 0x02, 0x31, 0x01, 0x00, 0x00, 0x35 };
            System.Text.ASCIIEncoding encodingString = new System.Text.ASCIIEncoding();
            Debug.Log("string : " + encodingString.GetString(SendingBytes));

            #endregion
            */

            #region SendToPort by bytes
            // convert text 0x01, 0x02, 0x31, 0x01, 0x00, 0x00, 0x35 to byte[]
            // split text by ","
            // convert to byte[]
            string[] byteTextArray = item_lane.ToString().Split(new string[] {","}, System.StringSplitOptions.RemoveEmptyEntries);
            string byteText = "";
            
            byte[] bytes = new byte[byteTextArray.Length];

            for (int b = 0; b < byteTextArray.Length; b++)
                {
                    byteText = byteTextArray[b].Trim().Replace(" ", string.Empty);
                   // Debug.Log(byteText);
                    bytes[b] = System.Convert.ToByte(byteText, 16);
                }

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            Debug.Log("string from bytes : " + encoding.GetString(bytes));

            vm.SendToPort(bytes); // bytes

            #endregion

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

                UpdateData(col, val, "id = '" + item_id + "'");
            }

            Debug.Log(item_id + " : " + item_name + " has " + item_quantity + " left | Lane : " + item_lane);

            Close();
      //  }
      // catch (System.Exception ex)
      //  {
        //    Debug.LogError(ex.Message);
      //  }
        
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
