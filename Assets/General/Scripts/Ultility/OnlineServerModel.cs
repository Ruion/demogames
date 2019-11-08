using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataBank;
using UnityEngine.Networking;
using System.IO;
using System.Linq;

public class OnlineServerModel : ServerModelMaster
{
    public bool isFetchingData = false;

    public string TextPath = "Application.stremingAssetsPath/local/LocalData.txt";

    public List<UserEntity> serverUsers = new List<UserEntity>();

    public List<string> emailList = new List<string>();

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

        emailList = new List<string>();

        string HtmlText = GetHtmlFromUri();
        if (HtmlText == "")
        {
            //No connection
            Debug.LogError("no internet connection");
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
                    isFetchingData = false;
                    yield break;
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

                        emailList.Add(line.ToString());
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

        newUsersList = new List<UserEntity>();
        newUsersList.AddRange(serverUsers); // add server users into list

    }

    public IEnumerator FeedEmail(List<string> emailList_)
    {
        DoGetDataFromServer();

      //  Debug.Log(methodBase.Name + " : Fetching email");
        
        while (isFetchingData)
        {
            yield return null;
        }
    }

}