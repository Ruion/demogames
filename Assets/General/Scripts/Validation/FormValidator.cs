using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections;
using TMPro;
using Sirenix.OdinInspector;
using System.IO;

/// <summary>
/// Validate the user input in Registration Page.
/// Validate Type support : text, email.
/// Notes: By default it get "email" and "contact" list from DBModelEntity to check email, contact duplication
/// </summary>
public class FormValidator : MonoBehaviour
{
    #region variables

    public Button Submit;

    //[InfoBox("Same button as submit put on top. Warn player when click on it, when form is not completed")]
    //public Button WarningSubmit;

    public Toggle consent;

    //public GameObject formWarning;
    //public TextMeshProUGUI warningText;

    [TableList]
    public List<FormField> formFields = new List<FormField>();

    private FormField invalidField;

    #endregion variables

    private void OnEnable()
    {
        InvokeRepeating("ValidateFields", 2f, .5f);
    }

    private void Start()
    {
        for (int i = 0; i < formFields.Count; i++)
        {
            Debug.Log(i);
            formFields[i].Initialize();
        }
    }

    private void ValidateFields()
    {
        invalidField = formFields.FirstOrDefault(f => f.isValid == false);

        if (invalidField == null && consent.isOn == true)
        {
            Submit.interactable = true;
            //WarningSubmit.gameObject.SetActive(false);
        }
        else
        {
            //WarningSubmit.gameObject.SetActive(true);
            Submit.interactable = false;
            //warningText.text = invalidField.warningMessage;
        }
    }

    //public void WarningValidate()
    //{
    //    if (invalidField != null)
    //    {
    //        formWarning.SetActive(true);
    //    }
    //    else
    //    {
    //        if (consent.isOn == false)
    //        {
    //            warningText.text = "Please accept policy agreement before submit";
    //            formWarning.SetActive(true);
    //        }
    //    }
    //}

    private void OnDisable()
    {
        CancelInvoke();
    }
}

[System.Serializable]
public class FormField
{
    //public FieldType fieldType = FieldType.text;
    [HorizontalGroup("Fields")]
    [VerticalGroup("Fields/Left")]
    [PropertyTooltip("Label the item with a name. This is important to make the set up clear and easier to manage. Database Model will take coulmn from \"Field Name\" to compare when Is Unique is tick.")]
    public string fieldName;

    [VerticalGroup("Fields/Left")]
    public TMP_InputField textField;

    [VerticalGroup("Fields/Left")]
    [LabelText("DropDown (optional)")]
    [PropertyTooltip("Provide optional dropdown input. The final value will become DropDown + Text Field. Example : dropdown value + abc")]
    public TMP_Dropdown dropDown;

    [VerticalGroup("Fields/Left")]
    [LabelText("RegexPattern (optional)")]
    [PropertyTooltip(@"Regex pattern that validate value of Text Field. When dropdown is provided, regext will validate value of Drop Down + Text Field")]
    public string regexPattern;

    [VerticalGroup("Fields/Right")]
    [PropertyTooltip("Tick to check for duplication. You must provide either a Database Model component or Data txt file")]
    public bool isUnique = false;

    [VerticalGroup("Fields/Right")]
    [PropertyTooltip("Allow the input to use once. When column \"status\" is \"ready\", it will be valid. When it is \"redeemed\", will validate as duplicate")]
    public bool useOnceOnly = false;

    [ShowIf("isUnique")]
    [VerticalGroup("Fields/Right")]
    [LabelText("Database Model (optional)")]
    [PropertyTooltip("By providing Database Model Component, duplicate checking will check column in database name with Field Name. For example, Field Name is contact, validation will check \"contact\" column in database")]
    public DBModelMaster dbmodel;

    [ShowIf("isUnique")]
    [VerticalGroup("Fields/Right")]
    [FilePath(AbsolutePath = true)]
    [LabelText("Data txt file (optional)")]
    [PropertyTooltip(@"By providing full txt file path, checking will add the lines of string inside txt file. Example: C:\UID_Toolkit\output\player_email_list.txt")]
    public string serverDataFilePath;

    [ShowIf("isUnique")]
    [VerticalGroup("Fields/Right")]
    [PropertyTooltip("This object will be display when duplicate detected")]
    public GameObject duplicateWarning;

    [VerticalGroup("Fields/Left")]
    [PropertyTooltip("This object will be display when field is valid")]
    public GameObject validMarker;

    [VerticalGroup("Fields/Left")]
    [PropertyTooltip("This object will be display when field is invalid")]
    public GameObject invalidMarker;

    //[VerticalGroup("Fields/Left")]
    //[FoldoutGroup("Fields/Left/WarningMessage")]
    //[Multiline]
    //public string invalidFormatMessage;

    //[VerticalGroup("Fields/Left")]
    //[FoldoutGroup("Fields/Left/WarningMessage")]
    //[Multiline]
    //public string emptyMessage;

    //[VerticalGroup("Fields/Left")]
    //[FoldoutGroup("Fields/Left/WarningMessage")]
    //[ShowIf("isUnique")]
    //[Multiline]
    //public string duplicateMessage;

    //[HideInInspector]
    //public string warningMessage;

    [HideInInspector] public bool isValid;
    [HideInInspector] public bool isDuplicated;
    private List<string> dataList;
    private bool initialize;

    public string value
    {
        get
        {
            if (dropDown != null)
                return dropDown.options[dropDown.value].text + textField.text;
            else return textField.text;
        }
    }

    public void Initialize()
    {
        if (initialize) return;
        initialize = true;

        if (isUnique)
        {
            if (serverDataFilePath != "")
                dataList = GetDataFromTextFile(serverDataFilePath);

            if (dbmodel != null && useOnceOnly == false)
                // add distinct item that not exist in dataList
                dataList.AddRange(dbmodel.GetDataByStringToList(fieldName).Except(dataList));
        }

        if (dropDown != null)
            dropDown.onValueChanged.AddListener(delegate { OnValueChange(); });
        textField.onValueChanged.AddListener(delegate { OnValueChange(); });
    }

    public void UpdateDatabase()
    {
    }

    private List<string> GetDataFromTextFile(string filePath)
    {
        List<string> textList = new List<string>();

        //Debug.Log(name + " GetEmailDataFromTextFile() started");

        string[] lines = File.ReadAllLines(filePath);

        // add emails to list
        foreach (string line in lines)
        {
            textList.Add(line.ToString());
        }

        return textList;
    }

    public void OnValueChange()
    {
        // check empty string
        isValid = !string.IsNullOrEmpty(textField.text);

        // string is empty
        if (isValid)
        {
            // check regex match
            if (!string.IsNullOrEmpty(regexPattern))
                isValid = Regex.IsMatch(value, regexPattern);

            // format not match
            //if (!isValid) warningMessage = invalidFormatMessage;

            // Check duplication
            if (isUnique)
            {
                // check duplicate
                string same = dataList.FirstOrDefault(t => t == value);

                if (same != null)
                {
                    isDuplicated = true;
                }
                else
                {
                    isDuplicated = false;
                }
            }

            if (useOnceOnly)
            {
                string query = $"SELECT status FROM {dbmodel.dbSettings.tableName} WHERE {fieldName} IN ('{value}')";
                object status = dbmodel.ExecuteCustomSelectObject(query);
                if (status != null)
                {
                    Debug.Log(status.ToString());

                    if (status.ToString() == "redeemed")
                    {
                        isDuplicated = true;
                    }
                }
            }

            DuplicateAction();
        }
        else
        {
            //warningMessage = emptyMessage;
        }

        if (!isValid)
        {
            invalidMarker.SetActive(true);
            validMarker.SetActive(false);
        }
        else
        {
            invalidMarker.SetActive(false);
            validMarker.SetActive(true);
        }
    }

    private void DuplicateAction()
    {
        if (isDuplicated)
        {
            isValid = false;
            //if (duplicateWarning != null)
            duplicateWarning.SetActive(true);
            //warningMessage = duplicateMessage;
        }
        else
        {
            isDuplicated = false;

            //if (duplicateWarning != null)
            duplicateWarning.SetActive(false);
        }
    }
}

public enum FieldType
{
    text = 1,
    consent = 2,
    option = 3
}