﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Data;
using Sirenix.OdinInspector;

/// <summary>
/// Use on port UI model popup to change lane, enable state of a port.
/// </summary>
public class PortUIEntity : MonoBehaviour
{
    #region Fields
    public bool is_enabled_ { get { return is_enabled; } set { is_enabled = value; ChangeState(); } }
    public bool is_enabled = false;

    public Color32[] colors;
    public GameObject model;
    public Image img_ { set { model.SetActive(true); img = value; SelectPort(value.transform.GetSiblingIndex()); } }
    private Image img;
    public Transform portParent;

    private int motorId;
    public TextMeshProUGUI motorName;
    public TMP_InputField laneField;
    public TMP_InputField quantityField;
    public GameObject enabledBtn;
    public GameObject disabledBtn;
    public Button SaveButton;

    public bool tempPortIsEnabled;

    public StockDBModelEntity stockDb;
    public TextMeshProUGUI totalText;
    #endregion

    public void ChangeState()
    {
        img.color = colors[System.Convert.ToInt32(is_enabled)];
    }

    private void SelectPort(int id)
    {
        try
        {
            DataRowCollection drc = stockDb.ExecuteCustomSelectQuery("SELECT * FROM " + stockDb.dbSettings.tableName + " WHERE id = " + id);

            int.TryParse(drc[0]["id"].ToString(), out motorId);
            motorName.text = drc[0][1].ToString();
            quantityField.text = drc[0][2].ToString();
            laneField.text = drc[0][3].ToString();
            tempPortIsEnabled = is_enabled_ = !bool.Parse(drc[0][4].ToString());

            if (drc[0][4].ToString() == "false") disabledBtn.SetActive(false);
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

        DataRowCollection drc = stockDb.ExecuteCustomSelectQuery("SELECT * FROM " + stockDb.dbSettings.tableName);
        for (int i = 0; i < drc.Count; i++)
        {
            img_ = imgs[int.Parse(drc[i]["id"].ToString())];
            img.GetComponentInChildren<TextMeshProUGUI>().text = drc[i]["id"].ToString();
        }

        model.SetActive(false);

        drc = stockDb.ExecuteCustomSelectQuery("SELECT SUM(quantity) FROM " + stockDb.dbSettings.tableName + " WHERE is_disabled = 'false'");
        totalText.text = drc[0][0].ToString();

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
                " is_disabled = '" + (!is_enabled).ToString().ToLower() + "'" +
                " WHERE name = '" + motorName.text + "'"
                );
        }catch(System.Exception ex) { Debug.Log(ex.Message); }
    }

    public void ValidateInput()
    {
        if (laneField.text == "" || quantityField.text == "" || int.Parse(quantityField.text) < 1) SaveButton.interactable = false;
        
        else SaveButton.interactable = true;
    }
}