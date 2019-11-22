﻿using System;
using System.Net;
using System.IO;
using System.Linq;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class EventSetting : SerializedMonoBehaviour
{
    #region Fields & Action
    public EventSettings eventSettings;
    
    public GameObject internetConnectionHandler;
    public GameObject errorHandler;
    public TMP_Dropdown eventDropdown;
    public TMP_Dropdown locationDropdown;
    public TMP_Dropdown sourceIdentifierDropdown;

    public TMP_InputField serverField;
    public TMP_InputField licenseKeyField;

    public List<EventFields> eventFields = new List<EventFields>();

    [TableList]
    public Dictionary<string, Button> buttons = new Dictionary<string, Button>();
    public Dictionary<string, SubmitActions> submitActions = new Dictionary<string, SubmitActions>();
    #endregion

    void OnEnable(){
        if(eventSettings.isVerify) eventSettings = (EventSettings)Data.LoadData(eventSettings.dataSettings.fileFullName);
    }

#region SaveLoad
    [Button(ButtonSizes.Medium)]
    private void SaveSettings(){
        eventSettings.dataSettings.fileFullName = eventSettings.dataSettings.fileName + "." + eventSettings.dataSettings.extension;
        Data.SaveData(eventSettings, eventSettings.dataSettings.fileFullName);
    }

    [Button(ButtonSizes.Medium)]
    private void LoadSettings(){
        eventSettings = (EventSettings)Data.LoadData(eventSettings.dataSettings.fileFullName);
    }

    public void SaveConfiguration()
    {
        eventSettings.selectedEvent = eventDropdown.options[eventDropdown.value].text;
        eventSettings.selectedLocation = locationDropdown.options[locationDropdown.value].text;
        eventSettings.selectedSourceIdentifier = sourceIdentifierDropdown.options[sourceIdentifierDropdown.value].text;
   
        SaveSettings();
    }
#endregion

    // verify from server
    public void FetchServerOptions(){
        string HtmlText = GetHtmlFromUri();
        if (HtmlText == "")
        {
            //No connection
            Debug.LogError("no internet connection");
            internetConnectionHandler.SetActive(true);
            return;
        }
        
        StartCoroutine(FetchServerOptionsRoutine());
    }

    private IEnumerator FetchServerOptionsRoutine(){
        using (UnityWebRequest www = UnityWebRequest.Get(eventSettings.serverURL)){
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
                errorHandler.SetActive(true);
                errorHandler.GetComponentInChildren<TextMeshProUGUI>().text = www.error;
                yield break;
            }
            else
            {
                while(!www.downloadHandler.isDone) yield return null;
                EventCode[] options = JsonUtility.FromJson<EventCode[]>(www.downloadHandler.text);
                eventSettings.eventCodes = options;
                eventSettings.isVerify = true;
                // Save options to file for local loading
                SaveSettings();

                // activate license key field
                licenseKeyField.interactable = true;
            }
        }
    }

    public void ResetFields()
    {
        foreach (EventFields f in eventFields)
        {
            if(f.field != null) f.field.text = "";
            if(f.dropdownfield != null) f.dropdownfield.ClearOptions();
        }
        buttons["url"].interactable = false;
        buttons["url"].gameObject.SetActive(true);

        buttons["licensekey"].interactable = false;
        buttons["licensekey"].gameObject.SetActive(true);

        buttons["saveoptions"].gameObject.SetActive(false);
        buttons["saveoptions"].interactable = false;
        
        buttons["done"].interactable = false;
        buttons["done"].gameObject.SetActive(false);
    }
    ///////////// AFTER License Key Verify /////////////
#region Dropdown Manipulation

    public void SubmitDomainReqest(TMP_InputField field)
    {
        StartCoroutine(SubmitDomainReqestRoutine(field.text));
    }

    private IEnumerator SubmitDomainReqestRoutine(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url)){
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
                errorHandler.SetActive(true);
                errorHandler.GetComponentInChildren<TextMeshProUGUI>().text = www.error;
                yield break;
            }
            else
            {
                while(!www.downloadHandler.isDone) yield return null;
                JSONResponse response = JsonUtility.FromJson<JSONResponse>(www.downloadHandler.text);

                if(response.result == "Success"){ submitActions["domain"].successAction.Invoke(); }
                else{  submitActions["domain"].failAction.Invoke(); }
            }
        }

    }
    private IEnumerator SubmitRequest(string url, TMP_Dropdown dropdown, string[] options_, string firstOption = "Select")
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url)){
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
                errorHandler.SetActive(true);
                errorHandler.GetComponentInChildren<TextMeshProUGUI>().text = www.error;
                yield break;
            }
            else
            {
                while(!www.downloadHandler.isDone) yield return null;
                string[] options = JsonUtility.FromJson<string[]>(www.downloadHandler.text);
                options_ = options;
                AddOptionToDropdown(options_, dropdown, firstOption);
            }
        }
    }
    private void AddOptionsToAllDropdown()
    {
        List<string> eventNames = new List<string>();
        foreach (EventCode e in eventSettings.eventCodes)
        {
            eventNames.Add(e.eventName);
        }
        AddOptionToDropdown(eventNames.ToArray(), eventDropdown, "Event");
    }

    public void ChangeDropdownOption(TMP_Dropdown dropdown)
    {
        EventCode selectedEvent = new EventCode();
        selectedEvent = eventSettings.eventCodes.FirstOrDefault(e => e.eventName == dropdown.options[dropdown.value].text);
        if(selectedEvent == null) return;
       
        AddOptionToDropdown(selectedEvent.locationOptions, locationDropdown, "Location");
        AddOptionToDropdown(selectedEvent.sourceIdentifierOptions, sourceIdentifierDropdown, "Source Identifier");
    }

#endregion
    #region SubMethod
    private void AddOptionToDropdown(string[] options, TMP_Dropdown dropdown, string firstOption = "Select")
    {
        dropdown.options.Clear();
        List<string> newOptions = options.ToList(); newOptions.Insert(0, firstOption);
        dropdown.AddOptions(newOptions);
    }

    public string GetHtmlFromUri(string resource = "http://google.com")
    {
        string html = string.Empty;
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(resource);
        try
        {
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
                if (isSuccess)
                {
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        //We are limiting the array to 80 so we don't have
                        //to parse the entire html document feel free to 
                        //adjust (probably stay under 300)
                        char[] cs = new char[80];
                        reader.Read(cs, 0, cs.Length);
                        foreach (char ch in cs)
                        {
                            html += ch;
                        }
                    }
                }
            }
        }
        catch
        {
            return "";
        }
        return html;
    }
    #endregion

    #region Field validation
        public void ValidateInputField(int fieldIndex)
        {
            bool isCorrect = true;
            if(eventFields[fieldIndex].isText)
            {
                if(!string.IsNullOrEmpty(eventFields[fieldIndex].field.text))
                {
                    isCorrect = Regex.IsMatch(eventFields[fieldIndex].field.text, eventFields[fieldIndex].regexPattern);
                }
                else isCorrect = false;
            }
            else
            {
                if(eventFields[fieldIndex].dropdownfield.value < 1) isCorrect = false;
            }
            
            if(isCorrect && eventFields[fieldIndex].successAction.GetPersistentEventCount() > 0 ) eventFields[fieldIndex].successAction.Invoke();

            if(!isCorrect && eventFields[fieldIndex].failAction.GetPersistentEventCount() > 0 ) eventFields[fieldIndex].failAction.Invoke();
        }
    #endregion
}

#region Objects
[Serializable]
public struct EventSettings
{
    [HideInPlayMode]
    public DataSettings dataSettings;
    [DisableInEditorMode] public string serverURL;
    [DisableInEditorMode] public string licenseKey;
    public bool isVerify;

    public EventCode[] eventCodes;
    
    public string selectedEvent;
    public string selectedLocation;
    public string selectedSourceIdentifier;
}

[Serializable]
public class EventFields
{
    public bool isText;
[ShowIf("isText", true)]  public TMP_InputField field;
[ShowIf("isText", true)]  public string regexPattern;
[HideIf("isText", true)]  public TMP_Dropdown dropdownfield;
[HorizontalGroup] public UnityEvent successAction;
[HorizontalGroup] public UnityEvent failAction;
}

[Serializable]
public class EventCode
{
    public string eventName;
    public string[] locationOptions;
    public string[] sourceIdentifierOptions;
}

[Serializable]
public class SubmitActions
{
    public UnityEvent successAction = new UnityEvent();
    public UnityEvent failAction = new UnityEvent();
}

#endregion