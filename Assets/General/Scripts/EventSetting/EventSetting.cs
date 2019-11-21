using System;
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

public class EventSetting : MonoBehaviour
{
    #region Fields
    public EventSettings eventSettings;
    
    public GameObject internetConnectionHandler;
    public GameObject errorHandler;
    public TMP_Dropdown eventDropdown;
    public TMP_Dropdown locationDropdown;
    public TMP_Dropdown sourceIdentifierDropdown;

    #endregion

    #region InputFields & Action
    public TMP_InputField serverField;
    public TMP_InputField licenseKeyField;

    public List<EventFields> eventFields = new List<EventFields>();
    public List<EventCode> eventCodes = new List<EventCode>();

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
                yield break;
            }
            else
            {
                while(!www.downloadHandler.isDone) yield return null;
                EventCode[] options = JsonUtility.FromJson<EventCode>(www.downloadHandler.text);
                eventCodes = options.ToList();
                eventSettings.isVerify = true;
                // Save options to file for local loading
                SaveSettings();

                // activate license key field
                licenseKeyField.interactable = true;
            }
        }
    }

    ///////////// AFTER License Key Verify /////////////
#region Dropdown Manipulation
    public void EnableDropdown(Dropdown dropdown_)
    {
        dropdown_.interactable = true;
    }

    private void AddOptionsToAllDropdown()
    {
        List<string> eventNames = new List<string>();
        foreach (EventCode e in eventSettings.eventCodes)
        {
            eventNames.Add(e.eventName);
        }
        AddOptionToDropdown(eventNames.ToArray(), eventDropdown);
        AddOptionToDropdown(eventSettings.eventCodes[0].locationOptions, locationDropdown);
        AddOptionToDropdown(eventSettings.eventCodes[0].sourceIdentifierOptions, sourceIdentifierDropdown);
    }

    public void ChangeDropdownOption(TMP_Dropdown dropdown)
    {
        EventCode selectedEvent = eventCodes.FirstOrDefault(
            e => e.eventName == dropdown.options[dropdown.value].ToString());
        AddOptionToDropdown(selectedEvent.locationOptions, locationDropdown);
        AddOptionToDropdown(selectedEvent.sourceIdentifierOptions, sourceIdentifierDropdown);
    }

#endregion
    #region SubMethod
    private void AddOptionToDropdown(string[] options, TMP_Dropdown dropdown)
    {
        dropdown.options.Clear();
        dropdown.AddOptions(options.ToList());
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
            }
            else
            {
                if(eventFields[fieldIndex].dropdownfield.value == 0) { isCorrect = false; return;}
            }
            
            if(isCorrect && eventFields[fieldIndex].successAction.GetPersistentEventCount() > 0 ) eventFields[fieldIndex].successAction.Invoke();
            else
            {
                if(!isCorrect && eventFields[fieldIndex].failAction.GetPersistentEventCount() > 0 ) eventFields[fieldIndex].failAction.Invoke();

            }
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
    [HideInInspector] public bool isVerify;

    public EventCode[] eventCodes;
    
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
public struct EventCode
{
    public string eventName;
    public string[] locationOptions;
    public string[] sourceIdentifierOptions;
}
#endregion