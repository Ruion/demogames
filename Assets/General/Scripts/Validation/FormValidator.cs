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
    private bool Text2OK = false;
    private bool Text3OK = false;

    public Button Submit;
    public Button virtualSubmit;

    public TMP_InputField NameText;
    public TMP_InputField PhoneText;
    public TMP_InputField EmailText;
    public TMP_Dropdown contactDropdown;
    public Toggle consent;

    private string MailPattern = @"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$";

    private string PhonePattern = @"^6?01\d{8,9}$";

    public GameObject emailWarning;
    public GameObject phoneWarning;

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
    }

    public void StartValidateOnFrequency()
    {
        Debug.Log("Start validate");
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

        if (Text1OK && Text2OK && Text3OK && consent.isOn)
        {
            Submit.interactable = true;
            virtualSubmit.interactable = true;
        }
        else
        {
            Submit.interactable = false;
            virtualSubmit.interactable = false;
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
        Text2OK = Regex.IsMatch(contact, PhonePattern);

        if (!Text2OK || ToogleWarning(PhoneText.text, contactList, phoneWarning)) { ChangeHint(1, false); }
        else { ChangeHint(1, true); }
    }

    public void T3Change()
    {
        Text3OK = Regex.IsMatch(EmailText.text, MailPattern);

        if (!Text3OK || ToogleWarning(EmailText.text, emailList, emailWarning)) { ChangeHint(2, false); }
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

    private bool ToogleWarning(string text, List<string> list, GameObject warningObject)
    {
        if (text == "") return false;

        if (ValidateDuplicate(list, text))
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

    private bool ValidateDuplicate(List<string> source, string text_)
    {
        bool hasSame = false;

        string same = source.FirstOrDefault(t => t == text_);
        if (same != null) hasSame = true;

        return hasSame;
    }

    public void DoCombineServerUsers()
    {
        StartCoroutine(CombineServerUsers());
    }

    private IEnumerator CombineServerUsers()
    {
        yield return StartCoroutine(playerDataDbModelEntity.GetDataFromServer());

        for (int i = 0; i < playerDataDbModelEntity.serverEmailList.Count; i++)
        {
            AddUniqueUser(playerDataDbModelEntity.serverEmailList[i], emailList);
        }

        playerDataDbModelEntity.serverEmailList = new List<string>();
    }
}