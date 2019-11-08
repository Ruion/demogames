using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Reflection;

public class TimeManager : MonoBehaviour
{

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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            second = 1;
        }
    }

    public IEnumerator StartCountdown()
    {
        if (isRealtime) yield return new WaitForSecondsRealtime(1);

        else yield return new WaitForSeconds(1);

        second--;

        // update textmesh text if textmesh components exists
        if (countDownTexts.Length > 0) { UpdateText(countDownTexts, Mathf.RoundToInt(second)); }


        if (second <= 0)
        {
            // Execute event on countdown ended
            countdownEndEvents.Invoke();
            StopAllCoroutines();
            yield return null;
        }

        // repeat countdown until time become 0
        StartCoroutine(StartCountdown());

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
