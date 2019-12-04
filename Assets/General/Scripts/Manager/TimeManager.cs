using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// Execute unity event after specific second.
/// Tips: attach this component to gameObject and assign functions to countdownEndEvents. Call StartGame() to execute events after specific second.
/// TimeManager normally use to execute event once after specific second. 
/// For example, call RestartScene() in SceneSwitch to restart scene, after 120 seconds in Registration Page
/// </summary>
public class TimeManager : MonoBehaviour
{

/// <summary>
/// set this value to change value of "second",
/// </summary>
/// <value>second</value>
    public float countDownSeconds
    {
        get { return second; }
        set { second = value; }
    }
    public float second = 120;

    [ReadOnly] public float initialSecond;

    public bool isRealtime = true;
    public string stringFormat = "";

    [Header("Optional Countdown TextMeshPro")]
    public TextMeshProUGUI[] countDownTexts;
    public UnityEvent countdownEndEvents;

    public bool useShortcut = true;
    private bool counting = false;

    void Awake()
    {
        initialSecond = second;
        UpdateText();
    }

    public void StartGame()
    {
        if (initialSecond == 0) initialSecond = second;

        // ResetCountDown();
        StartCoroutine(StartCountdown());
    }

    private void Update()
    {
        if(!useShortcut) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            second = 1;
        }
    }

    public IEnumerator StartCountdown()
    {
        counting = true;

        while(counting){
        
        if (isRealtime) yield return new WaitForSecondsRealtime(1);

        else yield return new WaitForSeconds(1);

        second--;

        // update textmesh text if textmesh components exists
        if (countDownTexts.Length > 0) { UpdateText(countDownTexts, Mathf.RoundToInt(second)); }

        if (second <= 0)
        {
            // Execute event on countdown ended
            countdownEndEvents.Invoke();
            counting = false;
            StopAllCoroutines();
            ResetCountDown();
            yield return null;
        }
        }
        // repeat countdown until time become 0
        //StartCoroutine(StartCountdown());

    }

    void UpdateText(TextMeshProUGUI[] texts_, int number)
    {
        for (int t = 0; t < texts_.Length; t++)
        {
            texts_[t].text = number.ToString(stringFormat);
        }
    }

    public void UpdateText()
    {
        for (int t = 0; t < countDownTexts.Length; t++)
        {
            countDownTexts[t].text = second.ToString(stringFormat);
        }
    }

    public void ResetCountDown()
    {
        second = initialSecond;
    }
}
