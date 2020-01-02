using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Data;
using System;
using System.IO;
using Sirenix.OdinInspector;

public class VoucherDBModelEntity : DBModelMaster
{
    #region Fields
    DataRowCollection rows;
    [ReadOnly] int voucher_id;
    [ReadOnly] string voucher_name;
    [ReadOnly] int voucher_quantity;
    [SerializeField] private int populate_quantity;

    public UnityEvent OnOutOfStock;
    public UnityEvent OnVoucherPrint;

    public string[] vouchersName;
    public int[] vouchersQuantity;

    [DisableContextMenu]
    public VoucherEntity ChosenVoucherEntity;

    public RandomFeature rf;
    public Print_Program printer;
    public DBModelEntity voucherDistributionDBModelEntity;

    #endregion

    #region SetUp
    protected override void OnEnable()
    {
        CheckDay();
        Directory.CreateDirectory(Path.GetDirectoryName(dbSettings.folderPath + "\\Vouchers\\"));
        DirectoryInfo di = new DirectoryInfo(dbSettings.folderPath + "\\Vouchers\\");
        if (di.GetFiles("*.exe*").Length < 0)
            Debug.LogError(String.Format("No voucher pdf files in {0}. Please put Printer.exe & voucher design pdf", dbSettings.folderPath + "\\Vouchers\\"));
    }

    protected override void Populate()
    {
        SaveSetting();
        CreateTable();

        List<string> col = new List<string>();
        List<string> val = new List<string>();

        for (int v = 1; v < dbSettings.columns.Count; v++)
        {
            col.Add(dbSettings.columns[v].name);
        }

        val.AddRange(col);

        for (int n = 0; n < vouchersName.Length; n++)
        {
            string name = vouchersName[n];
            string quantity = vouchersQuantity[n].ToString();
            string dateCreated = System.DateTime.Now.ToString("yyyy - MM - dd HH:mm:ss");

            val[0] = name;
            val[1] = quantity;
            val[2] = quantity;
            val[val.Count - 1] = dateCreated;

            AddData(col, val);
        }

        TestIndex++;

        Close();
    }

    #endregion

    [ContextMenu("Print")]
    public void PrintVoucher()
    {
        #region Get Voucher
        LoadSetting();

        ProbabilityCheck pc = rf.CalculateProbability();

        if (pc == null) { if (OnOutOfStock.GetPersistentEventCount() > 0) OnOutOfStock.Invoke(); Debug.LogError("no more voucher"); return; }

        voucher_id = pc.id;
        voucher_name = pc.name;
        voucher_quantity = pc.quantity;

        voucher_quantity--;
        #endregion

        #region Print Voucher
        if (OnVoucherPrint.GetPersistentEventCount() > 0) OnVoucherPrint.Invoke();

        printer.printerPath = dbSettings.folderPath + "\\Vouchers\\";
        printer.Print(voucher_name);
        // UPDATE voucher quantity
        ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET quantity = " + voucher_quantity + " WHERE voucher_code = '" + voucher_name + "'");

        // Insert printed voucher data into VoucherDistributionDatabaseModel to be sync to server
        List<string> col = new List<string>();
        List<string> val = new List<string>();

        col.Add("user_id");
        col.Add("voucher_id");
        col.Add("online_status");

        val.Add(PlayerPrefs.GetString(FindObjectOfType<GameSettingEntity>().gameSettings.userPrimaryKeyName));
        val.Add(voucher_id.ToString());
        val.Add("new");
        voucherDistributionDBModelEntity.AddData(col, val);

        Debug.Log(voucher_id + " : " + voucher_name + " has " + voucher_quantity + " left");

        #endregion
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
            if (a < 0) ResetDailyStock();

            string todaydate = DateTime.Now.Date.ToString();
            PlayerPrefs.SetString("TheDate", todaydate);
        }

    }

    [ContextMenu("Reset Daily Stock")]
    private void ResetDailyStock()
    {
        // update all voucher quantity to daily_quantity
        DataRowCollection drc = ExecuteCustomSelectQuery(string.Format("SELECT * FROM {0}", dbSettings.tableName));
        foreach (DataRow r in drc)
        {
            ExecuteCustomNonQuery(string.Format(
                "UPDATE {0} SET quantity = {1} WHERE id = {2}",
                new System.Object[] { dbSettings.tableName, r["daily_quantity"].ToString(), r["id"].ToString() }));
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