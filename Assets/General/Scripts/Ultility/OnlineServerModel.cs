using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataBank;
using UnityEngine.Networking;
using System.Net;
using System.IO;
using System.Linq;

public class OnlineServerModel : GameSettingEntity
{
    [ContextMenu("GetServerData")]
    public void DoGetDataFromServer()
    {
        StartCoroutine(GetDataFromServer());
    }

    IEnumerator GetDataFromServer()
    {
        List<UserEntity> serverUsers = new List<UserEntity>();

        string HtmlText = GetHtmlFromUri();
        if (HtmlText == "")
        {
            //No connection
            yield return serverUsers;
            yield break;
        }
        else
        {
            // fetch server data to serverUser
            WWWForm form = new WWWForm();
            //form.AddField("licensekey", value);

            using (UnityWebRequest www = UnityWebRequest.Post(gameSettings.serverGetDataAddress, form))
            {

                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.LogError(www.error);
                    yield return null;
                }
                else
                {
                    string texts = www.downloadHandler.text;
                    //Debug.Log(texts);

                    List<string> lines = new List<string>(
                     texts
                     .Split(new string[] { "\r", "\n" },
                     System.StringSplitOptions.RemoveEmptyEntries));

                    lines = lines
                        .Where(line => !(line.StartsWith("//")
                                        || line.StartsWith("#")))
                        .ToList();

                    foreach(string line in lines)
                    {
                        Debug.Log(line);
                    }
                }
            }

        }
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
}