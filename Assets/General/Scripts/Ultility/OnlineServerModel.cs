using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataBank;
using UnityEngine.Networking;
using System.Net;
using System.IO;
using System.Linq;
using System;

public class OnlineServerModel : GameSettingEntity
{
    public bool isFetchingData = false;

    public string TextPath = "Application.stremingAssetsPath/local/LocalData.txt";

    public List<UserEntity> serverUsers = new List<UserEntity>();

    private void Start()
    {
        SetUpTextPath();
        isFetchingData = false;
    }

    private void SetUpTextPath()
    {
        TextPath = Application.streamingAssetsPath + "/LocalData.txt";
        if (!File.Exists(TextPath))
        {
            File.WriteAllText(TextPath, "");
        }
        LoadGameSettingFromMaster();
    }

    [ContextMenu("GetServerData")]
    public void DoGetDataFromServer()
    {
        if (isFetchingData) return;

        isFetchingData = true;
        StartCoroutine(GetDataFromServer());
    }

    public IEnumerator GetDataFromServer()
    {
        SetUpTextPath();

        serverUsers = new List<UserEntity>();

        string HtmlText = GetHtmlFromUri();
        if (HtmlText == "")
        {
            //No connection
            Debug.LogError("no internet connection");
            yield return serverUsers;
            isFetchingData = false;
            yield break;
        }
        else
        {

            using (UnityWebRequest www = UnityWebRequest.Get(gameSettings.serverGetDataAddress))
            {
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.LogError(www.error);
                }
                else
                {
                    while (!www.downloadHandler.isDone) yield return null;

                    string texts = www.downloadHandler.text;

                    // clear text file
                    File.WriteAllText(TextPath, "");

                    // write email list to file
                    StreamWriter writer = new StreamWriter(TextPath, true); //open txt file (doesnt actually open it inside the game)
                    writer.Write(texts); //write into txt file the string declared above
                    writer.Close();

                    List<string> lines = new List<string>(
                     texts
                     .Split(new string[] { "\r", "\n" },
                     System.StringSplitOptions.RemoveEmptyEntries));

                    lines = lines
                        .Where(line => !(line.StartsWith("//")
                                        || line.StartsWith("#")))
                        .ToList();

                    foreach (string line in lines)
                    {
                        UserEntity user = new UserEntity();
                        user.email = line.ToString();
                       // Debug.Log("Server email " + user.email);
                        serverUsers.Add(user);
                    }
                }
            }
        }

        isFetchingData = false;
    }

    public IEnumerator FeedUsers(List<UserEntity> newUsersList)
    {
        DoGetDataFromServer();

     //   Debug.Log("Fetching user");

        while (isFetchingData)
        {
            yield return null;
        }

        newUsersList.Clear(); // clear list
        newUsersList.AddRange(serverUsers); // add server users into list

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