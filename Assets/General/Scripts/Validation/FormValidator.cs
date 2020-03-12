using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections;
using TMPro;

/// <summary>
/// Validate the user input in Registration Page.
/// Validate Type support : text, email.
/// Notes: By default it get "email" and "contact" list from DBModelEntity to check email, contact duplication
/// </summary>
public class FormValidator : ServerModelMaster
{
    #region variables

    private bool Text1OK = false;
    private bool contactValid = false;
    private bool emailValid = false;

    private bool contactDuplicate = false;
    private bool emailDuplicate = false;

    public Button Submit;
    public Button warningButton;

    public TMP_InputField NameText;
    public TMP_InputField PhoneText;
    public TMP_InputField EmailText;
    public TMP_Dropdown contactDropdown;
    public Toggle consent;

    private string MailPattern = @"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$";

    //private string PhonePattern = @"^6?01\d{8,9}$";
    private string PhonePattern = @"^6?01\d{8,9}$";

    public GameObject emailWarning;
    public GameObject phoneWarning;

    public GameObject msgWarning;
    public TextMeshProUGUI warningText;

    public GameObject[] Ok_Markers;
    public GameObject[] NotOk_Markers;

    private List<string> emailList;
    private List<string> contactList;
    public DBModelEntity playerDataDbModelEntity;

    private int oskID;
    public float validateFrequency;
    public string contactPrefix = "+6";

    #endregion variables

    private void OnEnable()
    {
        emailList = playerDataDbModelEntity.GetDataByStringToList("email");
        contactList = playerDataDbModelEntity.GetDataByStringToList("contact");
        playerDataDbModelEntity.GetEmailDataFromTextFile();
        playerDataDbModelEntity.GetContactDataFromTextFile();

        DoCombineServerUsers();
    }

    public void StartValidateOnFrequency()
    {
        //Debug.Log("Start validate");
        InvokeRepeating("Validate", 2f, validateFrequency);
    }

    public void StopValidateOnFrequency()
    {
        CancelInvoke("Validate");
    }

    public void Validate()
    {
        T1Change();
        T2Change();
        T3Change();

        if (Text1OK && contactValid && emailValid && consent.isOn)
        {
            Submit.interactable = true;
            warningButton.gameObject.SetActive(false);
        }
        else
        {
            Submit.interactable = false;
            warningButton.gameObject.SetActive(true);
        }
    }

    public void T1Change()
    {
        Text1OK = InputNotEmpty(NameText);

        if (!Text1OK) { ChangeHint(0, false); }
        else { ChangeHint(0, true); }
    }

    public void T2Change()
    {
        //string contact = contactDropdown.options[contactDropdown.value].text + PhoneText.text;
        string contact = PhoneText.text;
        contactValid = Regex.IsMatch(contact, PhonePattern);

        if (!contactValid || ToogleWarning(PhoneText.text, contactList, phoneWarning, contactDuplicate)) { ChangeHint(1, false); }
        else { ChangeHint(1, true); }
    }

    public void T3Change()
    {
        emailValid = Regex.IsMatch(EmailText.text, MailPattern);

        if (!emailValid || ToogleWarning(EmailText.text, emailList, emailWarning, emailDuplicate)) { ChangeHint(2, false); }
        else { ChangeHint(2, true); }
    }

    private bool InputNotEmpty(TMP_InputField text)
    {
        bool notEmpty = true;

        if (text.text == "" || text.text == null) notEmpty = false;

        return notEmpty;
    }

    private void ChangeHint(int InputIndex, bool isPass = false)
    {
        Ok_Markers[InputIndex].SetActive(isPass);
        NotOk_Markers[InputIndex].SetActive(!isPass);
    }

    private bool ToogleWarning(string text, List<string> list, GameObject warningObject, bool duplicateBool)
    {
        if (text == "") return false;

        if (ValidateDuplicate(list, text, duplicateBool))
        {
            warningObject.SetActive(true);

            return true;
        }
        else
        {
            warningObject.SetActive(false);
            return false;
        }
    }

    private bool ValidateDuplicate(List<string> source, string text_, bool duplicateBool)
    {
        bool hasSame = false;

        string same = source.FirstOrDefault(t => t == text_);
        if (same != null) hasSame = true;

        duplicateBool = hasSame;
        return hasSame;
    }

    public void ShowWarning()
    {
        warningText.text = "";

        if (!consent.isOn)
            warningText.text = "Please accept policy agreement";

        // if email valid
        if (!emailValid)
            warningText.text = "The email address you have entered is invalid\n example : example@gmail.com";
        if (string.IsNullOrEmpty(EmailText.text))
            warningText.text = "Please fill in your email address\n example : example@gmail.com";

        // if contact valid
        if (!contactValid)
            warningText.text = "Mobile number format entered is invalid\n Example : 0146734292";
        if (string.IsNullOrEmpty(PhoneText.text))
            warningText.text = "Please fill in your mobile number\n example : 0146734292";

        // NAME is empty
        if (!Text1OK)
        {
            warningText.text = "Please fill in your name";
        }

        // if email duplicate
        if (emailDuplicate)
            warningText.text = "The email address you have entered is already registered,\nplease enter a different email address.";

        // if contact duplicate
        if (contactDuplicate)
            warningText.text = "The mobile number you have entered is already registered,\nplease enter a different mobile number.";

        if (warningText.text != "")
            msgWarning.SetActive(true);
    }

    public void DoCombineServerUsers()
    {
        for (int i = 0; i < playerDataDbModelEntity.serverEmailList.Count; i++)
        {
            AddUniqueUser(playerDataDbModelEntity.serverEmailList[i], emailList);
        }

        for (int i = 0; i < playerDataDbModelEntity.serverContactList.Count; i++)
        {
            AddUniqueUser(playerDataDbModelEntity.serverContactList[i], contactList);
        }
    }
}