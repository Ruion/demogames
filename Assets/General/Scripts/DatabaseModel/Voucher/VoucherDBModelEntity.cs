using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Data;
using System;

public class VoucherDBModelEntity : DBModelMaster
{
    #region Fields
    DataRowCollection rows;
    [ReadOnly]string voucher_id;
    [ReadOnly]string voucher_name;
    [ReadOnly]int voucher_quantity;
    [SerializeField]private int populate_quantity;

    public UnityEvent OnOutOfStock;
    public UnityEvent OnVoucherPrint;

    public string[] vouchersName;
    public int[] vouchersQuantity;

    public int ChosenVoucher;
    public VoucherEntity ChosenVoucherEntity;

    #endregion

    protected override void OnEnable()
    {
        CheckDay();
    }

    public void SetVoucherStock()
    {
        LoadSetting();

    }

    public virtual void GetVoucher()
    {
        LoadSetting();

        string query = "SELECT id, name, quantity FROM " + dbSettings.localDbSetting.tableName + " WHERE " + selectCustomCondition;
        DataRowCollection drc = ExecuteCustomSelectQuery(query);

        if(drc.Count < 1)
        {
            // out of stock
            Debug.LogError("out of stock");
            if(OnOutOfStock.GetPersistentEventCount() > 0)OnOutOfStock.Invoke();

            return;
        }

        voucher_id = drc[0][0].ToString();
        voucher_name = drc[0][1].ToString();
        voucher_quantity = System.Int32.Parse(drc[0][2].ToString());
        
        if (OnVoucherPrint.GetPersistentEventCount() > 0) OnVoucherPrint.Invoke();

    }

    [ContextMenu("Print")]
    public void PrintVoucher()
    {
        GetVoucher();

        voucher_quantity--;

        try
        {
            List<string> col = new List<string>();
            List<string> val = new List<string>();

            col.Add("quantity");
            val.Add(voucher_quantity.ToString());

            UpdateData(col, val, "id = '" + voucher_id + "'");
            Debug.Log(voucher_id + " : " + voucher_name + " has " + voucher_quantity + " left");

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

        List<string> col = new List<string>();
        col.AddRange(dbSettings.localDbSetting.columns);

        List<string> val = new List<string>();
        val.AddRange(dbSettings.localDbSetting.columns);

        col.RemoveAt(0);
        val.RemoveAt(0);


        for (int n = 0; n < vouchersName.Length; n++)
        {
            string name = vouchersName[n];
            string quantity = vouchersQuantity[n].ToString();
            string dateCreated = System.DateTime.Now.ToString();

            val[1] = name;
            val[2] = quantity;
            val[3] = dateCreated;

            AddData(col, val);
        }

        TestIndex++;

        Close();
    }

    public void CheckDay()
    {
        string temp = PlayerPrefs.GetString("TheDate", "");
        if (temp == "")
        {
            string todaydate = DateTime.Now.Date.ToString();
            PlayerPrefs.SetString("TheDate", todaydate);
        }
        else
        {
            DateTime Date1 = DateTime.Parse(temp);
            DateTime Date2 = DateTime.Now.Date;

            int a = DateTime.Compare(Date1, Date2);
            if (a < 0)
            {
                // reset all voucher
                DeleteAllData();
                Populate();
            }

                string todaydate = DateTime.Now.Date.ToString();
                PlayerPrefs.SetString("TheDate", todaydate);
         }

        }
 }



public class VoucherEntity
{

    public int _id;
    public string _type;
    public int _stock;
    public string _dateCreated; // Auto generated timestamp

    public VoucherEntity(string type, int stock)
    {
        _type = type;
        _stock = stock;
        _dateCreated = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public VoucherEntity(int id, string type, int stock)
    {
        _id = id;
        _type = type;
        _stock = stock;
        _dateCreated = "";
    }

    public VoucherEntity(int id, string type, int stock, string dateCreated)
    {
        _id = id;
        _type = type;
        _stock = stock;
        _dateCreated = dateCreated;
    }
}
