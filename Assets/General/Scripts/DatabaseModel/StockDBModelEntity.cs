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

    #region stock
    public TextMeshProUGUI currentlaneAmount;
    public TextMeshProUGUI currentlaneAmountOccupyByItem;
    public TextMeshProUGUI currentitemQuantityPerLane;
    public TextMeshProUGUI totalItemLeftText;

    public TMP_InputField laneAmount;
    public TMP_InputField laneAmountOccupyByItem;
    public TMP_InputField itemQuantityPerLane;
    public Button setButton;
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

        laneAmount.text = numberToPopulate.ToString();
        laneAmountOccupyByItem.text = laneOccupyPerItem.ToString();
        itemQuantityPerLane.text = quantityPerLane.ToString();

        currentlaneAmount.text = numberToPopulate.ToString();
        currentlaneAmountOccupyByItem.text = laneOccupyPerItem.ToString();
        currentitemQuantityPerLane.text = quantityPerLane.ToString();

        DataRowCollection drc = ExecuteCustomSelectQuery("SELECT quantity FROM " + dbSettings.tableName + " WHERE is_disabled = 'false'");
        int total = 0;
        Debug.Log("total lane: " +drc.Count);
        for (int t = 0; t < drc.Count; t++)
        {
            total += System.Int32.Parse(drc[t][0].ToString());
        }

        totalItemLeftText.text = total.ToString();
    }

    public void ReSetStock()
    {
        LoadSetting();

        DeleteAllData();
        
        numberToPopulate = System.Int32.Parse(laneAmount.text);
        laneOccupyPerItem = System.Int32.Parse(laneAmountOccupyByItem.text);
        quantityPerLane = System.Int32.Parse(itemQuantityPerLane.text);

        SaveSetting();
        DeleteAllData();
        Populate();
        ReloadUI();
    }

    #region validation
    private bool ValidateInput(TMP_InputField field)
    {
        if (field.text != "" && System.Int32.Parse(field.text) > 0) { return false; }
        else return true;
    }

    public void ValidateInputSettings()
    {
        if (!ValidateInput(laneAmount) || !ValidateInput(laneAmountOccupyByItem) || !ValidateInput(laneAmountOccupyByItem)) setButton.interactable = false; 
        else setButton.interactable = true; 
    }

    #endregion

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
            return;
        }
        else
        {

            item_id = (int)drc[0][0];
            item_name = drc[0][1].ToString();
            item_quantity = (int)drc[0][2];
            item_lane = (int)drc[0][3];
            
        }
    }

    [ContextMenu("GiveReward")]
    public void GiveReward()
    {
        GetItem();

        if (item_quantity < 1) { if (OnOutOfStock.GetPersistentEventCount() > 0) OnOutOfStock.Invoke(); Debug.LogError("out of stock"); return; }

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

            UpdateData(col, val, "lane = '" + item_lane + "'");

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
        try
        {
            //for (int i = 0; i < numberToPopulate * laneOccupyPerItem; i++)
            for (int i = 0; i < numberToPopulate; i++)
            {
                string name = "Motor_" + (i.ToString());
                string quantity = quantityPerLane.ToString();
                string lane = i.ToString();
                string is_disabled = "false";

               // if (i % (laneOccupyPerItem) == 0) { is_disabled = "false"; Debug.Log(i + "% " + laneOccupyPerItem + " is " + i % laneOccupyPerItem); }
               // if (i == 0) is_disabled = "false";

                List<string> col = new List<string>();
                List<string> val = new List<string>();

                for (int v = 1; v < dbSettings.columns.Count; v++)
                {
                    col.Add(dbSettings.columns[v].name);
                }

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

    }
    #endregion


}
