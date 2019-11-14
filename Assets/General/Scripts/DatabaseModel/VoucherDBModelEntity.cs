using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Data;

public class VoucherDBModelEntity : DBModelMaster
{
    DataRowCollection rows;
    [ReadOnly]string voucher_id;
    [ReadOnly]string voucher_name;
    [ReadOnly]int voucher_quantity;
    [SerializeField]private int populate_quantity;

    public UnityEvent OnOutOfStock;
    public UnityEvent OnVoucherPrint;

    public override void SaveToLocal() {
        base.SaveToLocal();

        List<string> col = new List<string>();
        col.AddRange(dbSettings.localDbSetting.columns);
        col.RemoveAt(0);

        List<string> val = new List<string>();

        for (int i = 0; i < col.Count; i++)
        {
            val.Add(PlayerPrefs.GetString(col[i]));
        }

       AddData(col, val);
        Debug.Log(gameObject.name + " Data save to local");
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


        for (int n = 0; n < numberToPopulate; n++)
        {
            val[1] = colPrefix[0] + n.ToString();
            val[val.Count - 1] = populate_quantity.ToString();

            AddData(col, val);
        }

        TestIndex++;

        Close();
    }
}
