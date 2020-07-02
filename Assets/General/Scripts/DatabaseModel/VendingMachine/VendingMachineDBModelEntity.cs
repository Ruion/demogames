using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Data;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.IO;

/// <summary>
/// Manipulate database for Vending Machine, use with VendingMachine.cs to drop gift
/// Tips: call GiveReward() to drop gift from vending machine
/// </summary>
public class VendingMachineDBModelEntity : DBModelEntity
{
    #region fields

    [Header("Handler")]
    public GameObject stockErrorHandler;

    [Header("Stock setting")]
    public VendingMachine vm;

    private DataRowCollection rows;
    [DisableInEditorMode] [SerializeField] private int item_id;
    [DisableInEditorMode] [SerializeField] private string item_name;
    [DisableInEditorMode] [SerializeField] private int item_quantity;
    [DisableInEditorMode] [SerializeField] private int item_limit;
    [DisableInEditorMode] [SerializeField] private string item_lane;

    //[SerializeField] private int laneOccupyPerItem = 1;
    [SerializeField] private int quantityPerLane = 2;

    public UnityEvent OnOutOfStock;
    public UnityEvent OnStockGiven;
    public UnityEvent OnTestEnded;

    public float autoDropInterval = 10f;
    public int autoDropCycle = 1;
    public bool isTest = false;
    public TMP_InputField dropIntervalField;
    public TMP_InputField dropQuantityField;
    public TMP_InputField dropCycleField;

    [FilePath(AbsolutePath = true, Extensions = "txt")]
    public string queryCondition { get { return System.IO.File.ReadAllText(@"C:\UID-APP\App-VendingMachineDropper\QueryCondition.txt"); } }

    #endregion fields

    protected override void OnEnable()
    {
        base.OnEnable();
        HideAllHandler();
    }

    [ContextMenu("HideHandler")]
    public new void HideAllHandler()
    {
        if (stockErrorHandler != null)
            stockErrorHandler.SetActive(false);
    }

    public virtual void GetItem()
    {
        HideAllHandler();
        LoadSetting();

        // string query = "SELECT * FROM " + dbSettings.tableName + " WHERE " + selectCustomCondition;
        string query =
        string.Format("SELECT * FROM {0} WHERE is_disabled = 'false' AND quantity > 0{1} LIMIT 1"
        // string.Format("SELECT * FROM {0} WHERE is_disabled = 'false' ORDER BY RANDOM() LIMIT 1"
                        , new System.Object[] { dbSettings.tableName, queryCondition });

        /*string query =
        string.Format("SELECT * FROM {0} WHERE is_disabled = 'false' AND quantity > 0 LIMIT 1"
        // string.Format("SELECT * FROM {0} WHERE is_disabled = 'false' ORDER BY RANDOM() LIMIT 1"
                        , new System.Object[] { dbSettings.tableName });*/
        DataRowCollection drc = ExecuteCustomSelectQuery(query);

        if (drc.Count < 1)
        {
            // out of stock
            Debug.LogError("out of stock");
            if (OnOutOfStock.GetPersistentEventCount() > 0) { OnOutOfStock.Invoke(); }
            return;
        }
        else
        {
            item_id = int.Parse(drc[0][0].ToString());
            item_name = drc[0][1].ToString();
            item_quantity = int.Parse(drc[0][2].ToString());
            item_lane = drc[0][3].ToString();
            item_limit = System.Int32.Parse(drc[0]["item_limit"].ToString());
        }
    }

    [ContextMenu("GiveReward")]
    public void GiveReward()
    {
        GetItem();

        if (item_quantity < 1) return;

        ConnectDb();

        // turn the motor
        vm.TurnMotor(item_id.ToString());

        PlayerPrefs.SetString("motor_id", item_id.ToString());
        PlayerPrefs.SetString("lane", item_lane);
        PlayerPrefs.SetString("item_index", (item_limit + 1 - item_quantity).ToString());
        PlayerPrefs.SetString("operate_at", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        if (OnStockGiven.GetPersistentEventCount() > 0) OnStockGiven.Invoke();

        item_quantity--;

        List<string> col = new List<string>();
        List<string> val = new List<string>();

        col.Add("quantity");
        val.Add(item_quantity.ToString());

        UpdateData(col, val, "id = " + item_id);

        if (item_quantity < 1)
        {
            col = new List<string>();
            val = new List<string>();

            col.Add("is_disabled");
            val.Add("true");

            UpdateData(col, val, "id = '" + item_id + "'");
        }

        Debug.Log(name + " - " + item_id + " : " + item_name + " has " + item_quantity + " left | Lane : " + item_lane);

        Close();
    }

    #region Testing

    public void DropTest()
    {
        GetItem();

        if (item_quantity < 1) return;

        ConnectDb();

        // turn the motor
        vm.TurnMotor(item_id.ToString());

        PlayerPrefs.SetString("motor_id", item_id.ToString());
        PlayerPrefs.SetString("lane", item_lane);
        PlayerPrefs.SetString("item_index", (item_limit + 1 - item_quantity).ToString());
        PlayerPrefs.SetString("operate_at", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        if (OnStockGiven.GetPersistentEventCount() > 0) OnStockGiven.Invoke();

        item_quantity--;

        List<string> col = new List<string>();
        List<string> val = new List<string>();

        col.Add("quantity");
        val.Add(item_quantity.ToString());

        UpdateData(col, val, "id = " + item_id);

        if (item_quantity < 1)
        {
            col = new List<string>();
            val = new List<string>();

            col.Add("is_disabled");
            val.Add("true");

            UpdateData(col, val, "id = '" + item_id + "'");
        }

        Debug.Log(name + " - " + item_id + " : " + item_name + " has " + item_quantity + " left | Lane : " + item_lane);

        Close();

        if (OnTestEnded.GetPersistentEventCount() > 0) OnTestEnded.Invoke();
    }

    [Button(ButtonSizes.Large)]
    public void AutoTest()
    {
        autoDropInterval = float.Parse(dropIntervalField.text);
        // RefreshAutoTest();
        StartCoroutine(DropTestRoutine());
    }

    public void DropTestSpecific(int motorId)
    {
        LoadSetting();

        // string query = "SELECT * FROM " + dbSettings.tableName + " WHERE " + selectCustomCondition;
        string query =
        string.Format("SELECT * FROM {0} WHERE is_disabled = 'false' AND quantity > 0 AND id = {1} LIMIT 1"
                        , new System.Object[] { dbSettings.tableName, motorId });
        DataRowCollection drc = ExecuteCustomSelectQuery(query);

        if (drc.Count < 1)
        {
            // out of stock
            Debug.LogError("out of stock");
            if (OnOutOfStock.GetPersistentEventCount() > 0) { OnOutOfStock.Invoke(); }
            return;
        }
        else
        {
            item_id = int.Parse(drc[0][0].ToString());
            item_name = drc[0][1].ToString();
            item_quantity = int.Parse(drc[0][2].ToString());
            item_lane = drc[0][3].ToString();
            item_limit = System.Int32.Parse(drc[0]["item_limit"].ToString());
        }

        if (item_quantity < 1) return;

        ConnectDb();

        // turn the motor
        vm.TurnMotor(item_id.ToString());

        PlayerPrefs.SetString("motor_id", item_id.ToString());
        PlayerPrefs.SetString("lane", item_lane);
        PlayerPrefs.SetString("item_index", (item_limit + 1 - item_quantity).ToString());
        PlayerPrefs.SetString("operate_at", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        if (OnStockGiven.GetPersistentEventCount() > 0) OnStockGiven.Invoke();

        item_quantity--;

        List<string> col = new List<string>();
        List<string> val = new List<string>();

        col.Add("quantity");
        val.Add(item_quantity.ToString());

        UpdateData(col, val, "id = " + item_id);

        if (item_quantity < 1)
        {
            col = new List<string>();
            val = new List<string>();

            col.Add("is_disabled");
            val.Add("true");

            UpdateData(col, val, "id = '" + item_id + "'");
        }

        Debug.Log(name + " - " + item_id + " : " + item_name + " has " + item_quantity + " left | Lane : " + item_lane);

        Close();
    }

    [Button(ButtonSizes.Large)]
    public void AutoTestRandom()
    {
        autoDropInterval = float.Parse(dropIntervalField.text);
        StartCoroutine(DropTestRandomRoutine());
    }

    [Button(ButtonSizes.Large)]
    public void RefreshAutoTest()
    {
        DataRowCollection drc = ExecuteCustomSelectQuery("SELECT id FROM " + dbSettings.tableName + " WHERE item_limit > 0");

        // drop lane from 1 - 35 one by one with interval
        for (int i = 0; i < drc.Count; i++)
        {
            // Update all lane to 1 quantity and disable
            ExecuteCustomNonQuery(string.Format(
               "UPDATE {0} SET quantity = {2} , is_disabled = 'true' WHERE id = {1}",
               new System.Object[] { dbSettings.tableName, drc[i]["id"].ToString(), dropQuantityField.text }));
        }

        ExecuteCustomNonQuery(string.Format(
               "UPDATE {0} SET is_disabled = 'false' WHERE id = 1",
               new System.Object[] { dbSettings.tableName }));
    }

    private IEnumerator RefreshAutoTestRoutine()
    {
        DataRowCollection drc = ExecuteCustomSelectQuery("SELECT id FROM " + dbSettings.tableName + " WHERE item_limit > 0");

        // drop lane from 1 - 35 one by one with interval
        for (int i = 0; i < drc.Count; i++)
        {
            // Update all lane to 1 quantity and disable
            ExecuteCustomNonQuery(string.Format(
               "UPDATE {0} SET quantity = {2} , is_disabled = 'true' WHERE id = {1}",
               new System.Object[] { dbSettings.tableName, drc[i]["id"].ToString(), dropQuantityField.text }));
        }

        ExecuteCustomNonQuery(string.Format(
                "UPDATE {0} SET is_disabled = 'false' WHERE id = 1",
                new System.Object[] { dbSettings.tableName }));

        yield return null;
    }

    private IEnumerator DropTestRoutine()
    {
        for (int d = 0; d < System.Int32.Parse(dropCycleField.text); d++)
        {
            yield return StartCoroutine(RefreshAutoTestRoutine());

            yield return new WaitForSeconds(1f);

            DataRowCollection drc = ExecuteCustomSelectQuery("SELECT id,quantity FROM " + dbSettings.tableName + " WHERE item_limit > 0");

            // drop lane from 1 - 35 one by one with interval
            for (int i = 0; i < drc.Count; i++)
            {
                for (int y = 0; y < System.Convert.ToInt32(drc[i]["quantity"]); y++)
                {
                    GiveReward();
                    yield return new WaitForSeconds(autoDropInterval);
                }

                if (i + 1 == drc.Count) continue;
                ExecuteCustomNonQuery(string.Format(
                    "UPDATE {0} SET is_disabled = 'false' , quantity = {2} WHERE id = {1}",
                    new System.Object[] { dbSettings.tableName, drc[i + 1]["id"].ToString(), dropQuantityField.text }));
            }

            if (OnTestEnded.GetPersistentEventCount() > 0) OnTestEnded.Invoke();
            yield return null;
        }
    }

    private IEnumerator DropTestRandomRoutine()
    {
        for (int d = 0; d < System.Int32.Parse(dropCycleField.text); d++)
        {
            // Have limit list (item_limit more than 0)
            DataRowCollection drc = ExecuteCustomSelectQuery("SELECT id FROM " + dbSettings.tableName + " WHERE item_limit > 0");

            // drop lane from 1 - 35 one by one with interval
            for (int i = 0; i < drc.Count; i++)
            {
                // activate next lane
                ExecuteCustomNonQuery(string.Format(
                    "UPDATE {0} SET quantity = {2} , is_disabled = 'false' WHERE id = {1}",
                    new System.Object[] { dbSettings.tableName, drc[i]["id"].ToString(), dropQuantityField.text }));
            }

            int totalItem = System.Convert.ToInt32(ExecuteCustomSelectObject("SELECT SUM(quantity) FROM " + dbSettings.tableName + " WHERE is_disabled = 'false'"));

            yield return new WaitForSeconds(3);

            for (int i = 0; i < totalItem; i++)
            {
                GiveReward();
                yield return new WaitForSeconds(autoDropInterval);
            }

            if (OnTestEnded.GetPersistentEventCount() > 0) OnTestEnded.Invoke();
        }
    }

    #endregion Testing

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
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    protected override void Populate()
    {
        CreateTable();
        ConnectDb();
        SaveStockMultiple();
        TestIndex++;
    }

    #endregion Save

    #region External App Text file Communication

    public void SaveStockRemainToFile()
    {
        string appLaunchFilePath = Path.Combine(dbSettings.folderPath, "AppLaunchNumberFilePath.txt");

        // Get data file path from AppLaunchNumberFilePath.txt
        string[] dataFilePath = File.ReadAllLines(appLaunchFilePath);

        // Write stock left to text file
        DataRowCollection drc = ExecuteCustomSelectQuery("SELECT SUM(quantity) FROM " + dbSettings.tableName + " WHERE is_disabled = 'false'");
        File.WriteAllText(dataFilePath[1], drc[0][0].ToString());
    }

    #endregion External App Text file Communication
}