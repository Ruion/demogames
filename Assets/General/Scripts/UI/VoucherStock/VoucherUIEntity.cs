using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Data;

public class VoucherUIEntity : MonoBehaviour
{
    public bool is_enabled_ { get { return is_enabled; } set { is_enabled = value; ChangeState(); } }
    public bool is_enabled = false;

    public Color32[] colors;

    public Image img_ { set { gameObject.SetActive(true); img = value; SelectPort(value.transform.GetSiblingIndex()); } }
    private Image img;

    public TextMeshProUGUI motorName;
    public TMP_InputField laneField;
    public TMP_InputField quantityField;
    public GameObject enabledBtn;
    public GameObject disabledBtn;
    public Button SaveButton;

    public bool tempPortIsEnabled;

    public StockDBModelEntity stockDb;

    public void ChangeState()
    {
        img.color = colors[int.Parse(is_enabled.ToString())];
    }

    private void SelectPort(int id)
    {
        try
        {
            DataRowCollection drc = stockDb.ExecuteCustomSelectQuery("SELECT name, lane, quantity, is_disabled FROM " + stockDb.dbSettings.localDbSetting.tableName + " WHERE id = " + id);

            motorName.text = drc[0][0].ToString();
            laneField.text = drc[0][1].ToString();
            quantityField.text = drc[0][2].ToString();
            tempPortIsEnabled = is_enabled_ = System.Convert.ToBoolean(drc[0][3].ToString());

            if (drc[0][3].ToString() == "true") disabledBtn.SetActive(true);
            else disabledBtn.SetActive(false);
        }
        catch(System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }

    }

    public void RevertPortInfo()
    {
         is_enabled_ = tempPortIsEnabled;
    }

    public void SavePort()
    {
        try
        {
            stockDb.ExecuteCustomNonQuery(
                "UPDATE " + stockDb.dbSettings.localDbSetting.tableName +
                " SET quantity = '" + quantityField.text + "' ," +
                " lane = '" + laneField.text + "' ," +
                " is_disabled = '" + is_enabled.ToString() + "'" +
                " WHERE name = '" + motorName.text + "'"
                );
        }catch(System.Exception ex) { Debug.Log(ex.Message); }
    }

    public void ValidateInput()
    {
        if (laneField.text == "" || quantityField.text == "" || int.Parse(quantityField.text) < 1) SaveButton.interactable = false;
    }
}
