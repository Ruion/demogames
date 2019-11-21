using System;
using System.Net;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    void OnEnable(){
        eventSettings = (EventSettings)Data.LoadData(eventSettings.dataSettings.fileFullName);
        FetchServerOptions();
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
    private void FetchServerOptions(){
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
                EventOption options = JsonUtility.FromJson<EventOption>(www.downloadHandler.text);
                eventSettings.eventOptions.eventOptions = options.eventOptions;
                eventSettings.eventOptions.locationOptions = options.locationOptions;
                eventSettings.eventOptions.sourceIdetifierOptions = options.sourceIdetifierOptions;
                SaveSettings();
            }
        }
    }

    ///////////// AFTER License Key Verify /////////////
    private void AddOptionsToAllDropdown()
    {
        AddOptionToDropdown(eventSettings.eventOptions.eventOptions, eventDropdown);
        AddOptionToDropdown(eventSettings.eventOptions.locationOptions, locationDropdown);
        AddOptionToDropdown(eventSettings.eventOptions.sourceIdetifierOptions, sourceIdentifierDropdown);
    }

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

}

[Serializable]
public struct EventSettings
{
    [HideInPlayMode]
    public DataSettings dataSettings;
    [DisableInEditorMode] public string serverURL;
    [DisableInEditorMode] public string licenseKey;
    public EventOption eventOptions;
    
}

[Serializable]
public struct EventOption{
    public string[] eventOptions;
    public string[] locationOptions;
    public string[] sourceIdetifierOptions;
}