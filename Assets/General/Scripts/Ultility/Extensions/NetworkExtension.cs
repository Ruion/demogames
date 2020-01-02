﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEditor;

public class NetworkExtension : MonoBehaviour
{
    public static bool IsConnected(string hostedURL = "http://www.google.com")
    {
        try
        {
            string HtmlText = GetHtmlFromUri(hostedURL);
            if (HtmlText == "")
                return false;
            else
                return true;
        }
        catch (IOException ex)
        {
            Debug.LogError(ex.Message);
            return false;
        }
    }

    private static string GetHtmlFromUri(string resource = "http://google.com")
    {
        string html = string.Empty;
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(resource);
       // req.ContentType = "text/xml";
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
            Debug.Log(html);
        }
        catch
        {
            return "";
        }
        return html;
    }

    public static bool CheckForInternetConnection()
    {
        try
        {
            using (var client = new WebClient())
            using (client.OpenRead("http://google.com/generate_204"))
                return true;
        }
        catch
        {
            return false;
        }
    }
}
