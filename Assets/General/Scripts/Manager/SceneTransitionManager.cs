using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static Animator animator;
 public bool addEventsOnce = true;
    private bool eventsAdded = false;

    [Header("Optional")]
    public bool ExecuteEventsOnSpecifiedScene = false;

/// <summary>
/// Everytime the scene with this name loaded, EventOnSceneLoaded will be Invoke()
/// </summary>
    public string sceneNameToRunEvent = "HOME";

/// <summary>
/// Everytime the scene with this name loaded, EventOnSceneLoaded will be Invoke()
/// </summary>
    public int sceneIndexToRunEvent = 0;

    public void OnEnable()
    {
        if (eventsAdded) return;
   
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FadeOut();       
    }

    public static void FadeIn()
    {
        animator.SetInteger("Fade", 1);
    }

    public static void FadeOut()
    {
        animator.SetInteger("Fade", 0);
    }
}
