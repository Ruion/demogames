using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Data;
using TMPro;
using System.Linq;

/// <summary>
/// Manage the voucher stock, set and save the daily UI distribution
/// </summary>
public class VoucherStockManager : MonoBehaviour
{
    /// <summary>
    /// Parent that will hold the voucher setting InputFields
    /// </summary>
    public Transform voucherSettingParent;

    /// <summary>
    /// Prefab of voucher setting inputfield
    /// </summary>
    public GameObject voucherSettingField;
    public KeyboardScript ks;
    public VoucherDBModelEntity vdb;
    [ReadOnly] [SerializeField] private List<TMP_InputField> fields;

    public Button setButton;
    public Button saveButton;

    private void OnEnable()
    {
       if(vdb == null) vdb = FindObjectOfType<VoucherDBModelEntity>();
        ReloadUI();
    }

    [ContextMenu("Reload UI")]
    public void ReloadUI()
    {
        vdb.LoadSetting();

        #region Handler table is empty || not exist
        vdb.CreateTable();
        int rowCount = vdb.ExecuteCustomSelectQuery("SELECT * FROM " + vdb.dbSettings.tableName).Count;
        if (rowCount < 1) vdb.Populate();
        #endregion

        #region Clear fields
        if (voucherSettingParent.childCount > 0)
        {
            GameObject[] allChildren = new GameObject[voucherSettingParent.childCount];

            for (int i = 0; i < voucherSettingParent.childCount; i++)
            {
                allChildren[i] = voucherSettingParent.GetChild(i).gameObject;
            }

            foreach (GameObject child in allChildren)
            {
                DestroyImmediate(child);
            }

            if (voucherSettingParent.childCount > 0)
            {
                foreach (Transform c in voucherSettingParent)
                {
                    DestroyImmediate(c.gameObject);
                }
            }
        }
        #endregion

        #region Generate Fields
        // get voucher list
        DataRowCollection drc = vdb.ExecuteCustomSelectQuery("SELECT * FROM " + vdb.dbSettings.tableName);

        fields = new List<TMP_InputField>();

        // spawn field based on items in voucher list 
        for (int r = 0; r < drc.Count; r++)
        {
           GameObject newField = Instantiate(voucherSettingField, voucherSettingParent);
            newField.GetComponent<TextMeshProUGUI>().text = (r+1).ToString();

            Transform inputField = newField.GetComponentInChildren<TMP_InputField>().transform;
            TMP_InputField field = inputField.GetComponent<TMP_InputField>();
            field.text = drc[r]["initial_quantity"].ToString();
            field.onSelect.AddListener(delegate { ToggleKeyboard(field); });
            field.onValueChanged.AddListener(delegate { ValidateFields(); });
            newField.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = field.GetComponent<PlayerPrefsSaver>().name_ = drc[r]["name"].ToString();
            fields.Add(field);

        }

        ValidateFields();
        #endregion
    }

    public void SetVoucherStockQuantity()
    {
        vdb.LoadSetting();
        
        for (int i = 0; i < fields.Count; i++)
        {
            string voucherName = fields[i].GetComponent<PlayerPrefsSaver>().name_;
            PlayerPrefs.SetInt(voucherName, int.Parse(fields[i].text));
            // UPDATE voucher quantity
            vdb.ExecuteCustomNonQuery("UPDATE " + vdb.dbSettings.tableName + " SET initial_quantity = " + PlayerPrefs.GetInt(voucherName) + " WHERE name = '" + voucherName + "'");

        }
    }

    public void ResetVoucherStockQuantity()
    {
        vdb.LoadSetting();

        vdb.vouchersQuantity = new int[fields.Count];
        for (int q = 0; q < fields.Count; q++)
        {
            vdb.vouchersQuantity[q] = int.Parse(fields[q].text);
        }

        vdb.DeleteAllData();
        vdb.Populate();
    }

    void ToggleKeyboard(TMP_InputField _field)
    {
        ks.inputFieldTMPro_ = _field;
        ks.gameObject.SetActive(true);
        ks.ShowLayout(ks.SymbLayout);
    }

/// <summary>
/// Validate the input field's text is empty or not. 
/// Enable button click when no empty text in input field,
/// disable button click when there is empty text in input field
/// </summary>
    public void ValidateFields()
    {
        TMP_InputField field_ = fields.Where(i => string.IsNullOrEmpty(i.text)).FirstOrDefault();

        if (field_ != null) { saveButton.interactable = setButton.interactable = false; return; }

        saveButton.interactable = setButton.interactable = true;

    }

}
