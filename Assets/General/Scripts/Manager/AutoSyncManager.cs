using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(DataManager))]
public class AutoSyncManager : MonoBehaviour
{
    public DataManager dm;
    public int minutes;
    public int seconds;

    private int totalSeconds;

    public UnityEvent OnSyncStart;

    private void Start()
    {
        if(dm == null)
        {
            dm = FindObjectOfType<DataManager>();
        }

        totalSeconds = (minutes * 60) + seconds;
    }

    public void StartAutoSync()
    {

    }

    IEnumerator AutoSyncRoutine()
    {
        Debug.Log("AutoSync start after " + totalSeconds);

        yield return new WaitForSeconds(totalSeconds);

        Debug.Log("AutoSync started");

        //dm.SendDataToDatabase();
    }
}
