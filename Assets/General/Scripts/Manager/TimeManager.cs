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
    #region Fields

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
    public UnityEvent onCountDown;

    public bool useUpdate = false;
    public bool useShortcut = true;
    private bool counting = false;

    public bool useGameSettingTime = false;

    #endregion Fields

    private void Awake()
    {
        if (useGameSettingTime)
        {
            GameSettingEntity gse = GameObject.FindGameObjectWithTag("GameSettingMaster").GetComponent<GameSettingEntity>();
            second = gse.gameSettings.gameTime;
        }

        initialSecond = second;

        if (countDownTexts.Length < 1) return;

        UpdateText();
    }

    public void StartGame()
    {
        if (initialSecond == 0) initialSecond = second;

        // ResetCountDown();
        counting = true;

        if (!useUpdate) StartCoroutine(StartCountdown());
    }

    private void Update()
    {
        if (useUpdate && counting)
        {
            second -= Time.deltaTime;

            System.TimeSpan interval = System.TimeSpan.FromSeconds(second);
            if (countDownTexts.Length > 0)
            {
                UpdateText();
            }

            if (onCountDown.GetPersistentEventCount() > 0) onCountDown.Invoke();

            if (second <= 0)
            {
                countdownEndEvents.Invoke();
                counting = false;
                ResetCountDown();
                UpdateText();
            }
        }

        if (!useShortcut) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            second = 1;
        }
    }

    public IEnumerator StartCountdown()
    {
        while (counting)
        {
            if (isRealtime) yield return new WaitForSecondsRealtime(1);
            else yield return new WaitForSeconds(1);

            second--;

            if (onCountDown.GetPersistentEventCount() > 0) onCountDown.Invoke();

            // update textmesh text if textmesh components exists
            if (countDownTexts.Length > 0) { UpdateText(); }

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

    public void UpdateText()
    {
        if (countDownTexts.Length < 1) return;

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