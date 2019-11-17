using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Data;
using Sirenix.OdinInspector;

public class VoucherUIEntity : MonoBehaviour
{
    public bool is_enabled_ { get { return is_enabled; } set { is_enabled = value; ChangeState(); } }
    public bool is_enabled = false;

    public Color32[] colors;
    public GameObject model;
    public Image img_ { set { model.SetActive(true); img = value; SelectPort(value.transform.GetSiblingIndex()); } }
    private Image img;
    public Transform portParent;
    

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
        img.color = colors[System.Convert.ToInt32(is_enabled)];
    }

    private void SelectPort(int lane)
    {
        try
        {
            DataRowCollection drc = stockDb.ExecuteCustomSelectQuery("SELECT name, lane, quantity, is_disabled FROM " + stockDb.dbSettings.tableName + " WHERE lane = " + lane);

            motorName.text = drc[0][0].ToString();
            laneField.text = drc[0][1].ToString();
            quantityField.text = drc[0][2].ToString();
            tempPortIsEnabled = is_enabled_ = !bool.Parse(drc[0][3].ToString());

            if (drc[0][3].ToString() == "false") disabledBtn.SetActive(false);
            else disabledBtn.SetActive(true);
        }
        catch(System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }

    }

    [Button(ButtonSizes.Medium)]
    public void SetPorts()
    {
        Image[] imgs = portParent.GetComponentsInChildren<Image>();
        for (int i = 0; i < imgs.Length; i++)
        {
            DataRowCollection drc = stockDb.ExecuteCustomSelectQuery("SELECT name, lane, quantity, is_disabled FROM " + stockDb.dbSettings.tableName + " WHERE lane = " + i);
            img_ = imgs[i];
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
                "UPDATE " + stockDb.dbSettings.tableName +
                " SET quantity = '" + quantityField.text + "' ," +
                " lane = '" + laneField.text + "' ," +
                " is_disabled = '" + is_enabled.ToString() + "'" +
                " WHERE name = '" + motorName.text + "'"
                );
        }catch(System.Exception ex) { Debug.Log(ex.Message); }
    }

    public void ValidateInput()
    {
        if (laneField.text == "" || quantityField.text == "" || int.Parse(quantityField.text) < 1 || tempPortIsEnabled == is_enabled) SaveButton.interactable = false;
        else SaveButton.interactable = true;
    }
}
