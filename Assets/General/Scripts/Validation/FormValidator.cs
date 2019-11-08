using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections;
using TMPro;
using DataBank;

public class FormValidator : ServerModelMaster
{

    #region variables
    bool Text1OK = false;
    bool Text2OK = false;
    bool Text3OK = false;
    bool userIsUnique = true;

    public Button Submit;
    public Button virtualSubmit;


    public TMP_InputField NameText;
    public TMP_InputField PhoneText;
    public TMP_InputField EmailText;
    public TMP_Dropdown contactDropdown;

    string MailPattern = @"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$";

    string PhonePattern = @"^6?01\d{8,9}$";

    public GameObject emailWarning;
    public GameObject phoneWarning;

    public GameObject[] Ok_Markers;
    public GameObject[] NotOk_Markers;

    private List<string> emailList;
    private List<string> contactList;
    public UniversalUserDB udb;

    private int oskID;
    public float validateFrequency;
    #endregion

    private void Start()
    {
        udb.ConnectDbCustom();

        emailList = udb.GetDataByStringToList("email");
        contactList = udb.GetDataByStringToList("contact");

        udb.Close();
    }

    // Update is called once per frame
    void Update()
    {
        if (Text1OK && Text2OK && Text3OK && userIsUnique)
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
        CheckUserExists();
    }

    public void T1Change()
    {
        Text1OK = InputNotEmpty(NameText);

        if (!Text1OK) { ChangeHint(0, false); }
        else { ChangeHint(0, true); }
    }

    public void T2Change()
    {
        string contact = contactDropdown.options[contactDropdown.value].text + PhoneText.text;
        Text2OK = Regex.IsMatch(contact, PhonePattern);
        if (!Text2OK) { ChangeHint(1, false); }
        else { ChangeHint(1, true); }
    }

    public void T3Change()
    {
        Text3OK = Regex.IsMatch(EmailText.text, MailPattern);
        if (!Text3OK) { ChangeHint(2, false); }
        else { ChangeHint(2, true); }
    }

    private bool InputNotEmpty(TMP_InputField text)
    {
        bool notEmpty = true;

        if (text.text == "" || text.text == null) notEmpty = false;

        return notEmpty;
    }

    public void CheckUserExists()
    {
        userIsUnique = ToggleWarnings();
     //  if(!userIsUnique) Debug.Log("user not unique");
    }

    private void ChangeHint(int InputIndex, bool isPass = false)
    {
        Ok_Markers[InputIndex].SetActive(isPass);
        NotOk_Markers[InputIndex].SetActive(!isPass);
    }

    private bool ToggleWarnings()
    {
        bool isUnique = true;

        bool emailIsUnique = ToggleEmailWarning();
        bool phoneIsUnique = TogglePhoneWarning();

        if (emailIsUnique || phoneIsUnique)
        {
            isUnique = false;
        }

        return isUnique;
    }

    private bool ToggleEmailWarning()
    {
        bool hasSame = false;

        if (EmailText.text != "")
        {
            if (ValidateDuplicate(emailList, EmailText.text))
            {
                emailWarning.SetActive(true);
                hasSame = true;
                //   Debug.Log("email not unique");
            }
            else
            {
                emailWarning.SetActive(false);
                hasSame = false;

            }
        }

        return hasSame;
    }

    private bool TogglePhoneWarning()
    {
        bool hasSame = false;

        if (PhoneText.text != "")
        {
            if (ValidateDuplicate(contactList, contactDropdown.value + PhoneText.text))
            {
                phoneWarning.SetActive(true);
                hasSame = true;
                //  Debug.Log("phone not unique");
            }
            else
            {
                phoneWarning.SetActive(false);
                hasSame = false;

            }
        }

        return hasSame;
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
        OnlineServerModel osm = FindObjectOfType<OnlineServerModel>();

        yield return StartCoroutine(osm.GetDataFromServer());

        for (int i = 0; i < osm.emailList.Count; i++)
        {
            AddUniqueUser(osm.emailList[i], emailList);
        }
    }
}
