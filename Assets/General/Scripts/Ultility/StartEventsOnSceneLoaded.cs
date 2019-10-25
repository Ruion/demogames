using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class StartEventsOnSceneLoaded : MonoBehaviour
{
    public bool addEventsOnce = true;
    public bool eventsAdded = false;

    [Header("Optional")]
    public bool ExecuteEventsOnSpecifiedScene = false;
    public string sceneNameToRunEvent = "HOME";
    public int sceneIndexToRunEvent = 0;

    public UnityEvent EventOnSceneLoaded;

    public void OnEnable()
    {
        if (eventsAdded) return;

        if (addEventsOnce) eventsAdded = true;
   
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (ExecuteEventsOnSpecifiedScene)
        {
            if (SceneManager.GetActiveScene().name == sceneNameToRunEvent || SceneManager.GetActiveScene().buildIndex == sceneIndexToRunEvent)
                EventOnSceneLoaded.Invoke();
        }
        else { EventOnSceneLoaded.Invoke(); }
       
    }
}
