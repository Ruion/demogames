﻿using UnityEngine;
using UnityEngine.Events;
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
    public Image img_ { set { model.SetActive(true); img = value; SelectPort(value.transform.GetSiblingIndex()+1); } }
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
    public int item_limit;

    public VendingMachineDBModelEntity vendingMachineDb;
    public TextMeshProUGUI totalText;

    public UnityEvent onSetPorts;
    #endregion

    void OnEnable()
    {
        if (vendingMachineDb == null) vendingMachineDb = FindObjectOfType<VendingMachineDBModelEntity>();
    }

    public void ChangeState()
    {
        img.color = colors[System.Convert.ToInt32(is_enabled)];
    }

    private void SelectPort(int id)
    {
        DataRowCollection drc = vendingMachineDb.ExecuteCustomSelectQuery("SELECT * FROM " + vendingMachineDb.dbSettings.tableName + " WHERE id = " + id);

        int.TryParse(drc[0]["id"].ToString(), out motorId);
        motorName.text = drc[0][1].ToString();
        quantityField.text = drc[0][2].ToString();
        laneField.text = drc[0][3].ToString();
        tempPortIsEnabled = is_enabled_ = !bool.Parse(drc[0][4].ToString());
        item_limit = System.Int32.Parse(drc[0]["item_limit"].ToString());

        if (drc[0][4].ToString() == "false") disabledBtn.SetActive(false);
        else disabledBtn.SetActive(true);

    }

    [Button(ButtonSizes.Medium)]
    public void SetPorts()
    {
        Image[] imgs = portParent.GetComponentsInChildren<Image>();

        DataRowCollection drc = vendingMachineDb.ExecuteCustomSelectQuery("SELECT * FROM " + vendingMachineDb.dbSettings.tableName);
        for (int i = 0; i < drc.Count; i++)
        {
            img_ = imgs[System.Int32.Parse(drc[i]["id"].ToString()) - 1];
            
            // name the box
            img.GetComponentInChildren<TextMeshProUGUI>().text = drc[i][1].ToString();

            // display the current amount of motor
            if(img.enabled == true)
            img.GetComponentsInChildren<TextMeshProUGUI>()[1].text = string.Format("{0}/{1}", new System.Object[] { drc[i]["quantity"].ToString(), drc[i]["item_limit"].ToString() });
            
            // display avaibility of motor
            imgs[System.Int32.Parse(drc[i]["id"].ToString()) - 1].color = colors[System.Convert.ToInt32(!bool.Parse(drc[i][4].ToString()))];
        }

        drc = vendingMachineDb.ExecuteCustomSelectQuery("SELECT SUM(quantity) FROM " + vendingMachineDb.dbSettings.tableName + " WHERE is_disabled = 'false'");
        totalText.text = drc[0][0].ToString();

        model.SetActive(false);

        if (onSetPorts.GetPersistentEventCount() > 0) onSetPorts.Invoke();

    }

    public void RevertPortInfo()
    {
        is_enabled_ = tempPortIsEnabled;
    }

    public void SavePort()
    {
        vendingMachineDb.ExecuteCustomNonQuery(
            "UPDATE " + vendingMachineDb.dbSettings.tableName +
            " SET quantity = '" + quantityField.text + "' ," +
            " lane = '" + laneField.text + "' ," +
            " is_disabled = '" + (!is_enabled).ToString().ToLower() + "'" +
            " WHERE id = " + motorId
            );
    }

    public void Refill()
    {
        vendingMachineDb.ExecuteCustomNonQuery(
            "UPDATE " + vendingMachineDb.dbSettings.tableName +
            " SET quantity = '" + item_limit.ToString() + "' ," +
            " lane = '" + laneField.text + "' ," +
            " is_disabled = 'false'" +
            " WHERE id = " + motorId
            );
    }

    public void RefillAll()
    {
        vendingMachineDb.ExecuteCustomNonQuery(
            "UPDATE " + vendingMachineDb.dbSettings.tableName +
            " SET quantity = '5' ,is_disabled = 'false'"
            );

        SetPorts();
    }

    public void ValidateInput()
    {
        if (laneField.text == "" || quantityField.text == "" || int.Parse(quantityField.text) < 1) SaveButton.interactable = false;

        else SaveButton.interactable = true;
    }
}
