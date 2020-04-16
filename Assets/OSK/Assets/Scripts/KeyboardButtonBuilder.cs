using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;
using System.Threading.Tasks;

public class KeyboardButtonBuilder : MonoBehaviour
{
    public Color32 clickedBackgroundColor;
    public Color32 backgroundColor;
    public Color32 textColor;
    public GameObject[] singleButtons;
    public GameObject[] twinButtons;
    public GameObject[] specialButtons;

    [SerializeField]
    private KeyboardScript ks;

    [SerializeField]
    private float maxHoldTime = 1.2f;

    private bool builded;
    private bool Builded { get { return builded; } set { builded = value; } }

    private void OnEnable()
    {
        if (Builded) return;

        Builded = true;
        BuildButtons();
    }

    [Button(ButtonSizes.Large)]
    public void BuildSingleButton()
    {
        for (int b = 0; b < singleButtons.Length; b++)
        {
            GameObject child = singleButtons[b].transform.GetChild(0).gameObject;

            child.GetComponent<Image>().color = clickedBackgroundColor;

            // Get alphabet from button name
            string bName = singleButtons[b].name.Substring(singleButtons[b].name.IndexOf("_") + 1);
            TextMeshProUGUI text_ = child.GetComponentInChildren<TextMeshProUGUI>();

            if (text_ != null)
                text_.text = bName;

            if (Application.isPlaying)
            {
                // Add callback to OnPointerDown (call alphabet function to keyboard)
                EventTrigger trigger = singleButtons[b].GetComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerUp;
                entry.callback.AddListener((data) => { ks.alphabetFunction(bName); });
                trigger.triggers.Add(entry);
            }

#if UNITY_EDITOR
            if (Application.isEditor && !EditorApplication.isPlaying)
            {
                child.SetActive(true);
                DeactivateGameObjectInSeconds(child, 10);
            }
#endif
        }
    }

    [Button(ButtonSizes.Large)]
    public void BuildTwinButton()
    {
        for (int b = 0; b < twinButtons.Length; b++)
        {
            // Get clickBG children object
            GameObject child = twinButtons[b].transform.GetChild(0).gameObject;

            child.GetComponent<Image>().color = clickedBackgroundColor;

            // Get alphabets from button name
            // Get the string after "_" symbol
            string characters = twinButtons[b].name.Substring(twinButtons[b].name.IndexOf("_") + 1);

            // Get first character after "_" symbol
            // Example : Button_12 . get "1" from Button_12
            string firstCharacter = characters.Substring(0, 1);
            string secondCharacter = characters.Substring(1, 1);

            // Get text component and assign text to it
            // First text -- Main
            TextMeshProUGUI mainText = child.GetComponentInChildren<TextMeshProUGUI>();
            mainText.text = firstCharacter;
            mainText.name = firstCharacter;
            // Second text -- Sub
            TextMeshProUGUI subText = child.GetComponentsInChildren<TextMeshProUGUI>()[1];
            subText.text = secondCharacter;
            subText.name = secondCharacter;

#if UNITY_EDITOR
            // display it in editor for a short time to be check
            if (Application.isEditor && !EditorApplication.isPlaying)
            {
                child.SetActive(true);
                DeactivateGameObjectInSeconds(child, 10);
            }
#endif

            // Hold Background
            GameObject holdBG = twinButtons[b].transform.GetChild(1).gameObject;
            holdBG.GetComponent<Image>().color = clickedBackgroundColor;

            // Set the settings
            KeyboardTwinButton ktb = twinButtons[b].GetComponent<KeyboardTwinButton>();
            ktb.ks = ks;
            ktb.maxHoldTime = maxHoldTime;

            ktb.textColor = textColor;

            // Assign background color to Character option background
            ktb.characterOptions.GetComponent<Image>().color = backgroundColor;

            // Assign clicked color to Selection background
            ktb.characterOptions.transform.GetChild(0).GetComponent<Image>().color = clickedBackgroundColor;

            // Assign character text 1
            TextMeshProUGUI text1 = ktb.characterOptions.GetComponentsInChildren<TextMeshProUGUI>()[0];
            text1.text = firstCharacter;
            text1.name = firstCharacter;
            // Assign click event to main text button

            // Assign character text 2
            TextMeshProUGUI text2 = ktb.characterOptions.GetComponentsInChildren<TextMeshProUGUI>()[1];
            text2.text = secondCharacter;
            text2.name = secondCharacter;
            // Assign click event to sub text button

            if (Application.isPlaying)
            {
                // Add callback to OnPointerDown (call alphabet function to keyboard)
                EventTrigger trigger = text1.GetComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerDown;
                entry.callback.AddListener((data) => { ChangeTextColorToWhite((PointerEventData)data, text1); ChangeTextColorToDefault((PointerEventData)data, text2); });
                trigger.triggers.Add(entry);

                EventTrigger.Entry entryUp1 = new EventTrigger.Entry();
                entryUp1.eventID = EventTriggerType.PointerUp;
                entryUp1.callback.AddListener((data) => { ks.alphabetFunction(firstCharacter); ktb.characterOptions.SetActive(false); });
                trigger.triggers.Add(entryUp1);

                // Add callback to OnPointerDown (call alphabet function to keyboard)
                EventTrigger trigger2 = text2.GetComponent<EventTrigger>();
                EventTrigger.Entry entry2 = new EventTrigger.Entry();
                entry2.eventID = EventTriggerType.PointerDown;
                entry2.callback.AddListener((data) => { ChangeTextColorToWhite((PointerEventData)data, text2); ChangeTextColorToDefault((PointerEventData)data, text1); });
                trigger2.triggers.Add(entry2);

                EventTrigger.Entry entryUp2 = new EventTrigger.Entry();
                entryUp2.eventID = EventTriggerType.PointerUp;
                entryUp2.callback.AddListener((data) => { ks.alphabetFunction(secondCharacter); ktb.characterOptions.SetActive(false); });
                trigger2.triggers.Add(entryUp2);
            }

#if UNITY_EDITOR

            if (Application.isEditor && !EditorApplication.isPlaying)
            {
                holdBG.SetActive(true);
                DeactivateGameObjectInSeconds(holdBG, 10);
            }
#endif
        }
    }

    [Button(ButtonSizes.Large)]
    public void BuildSpecialButton()
    {
        for (int b = 0; b < specialButtons.Length; b++)
        {
            if (specialButtons[b].GetComponentsInChildren<TextMeshProUGUI>().Length < 1) continue;

            GameObject child = specialButtons[b].transform.GetChild(0).gameObject;

            child.GetComponent<Image>().color = clickedBackgroundColor;

            // Get alphabet from button name
            string bName = specialButtons[b].name.Substring(specialButtons[b].name.IndexOf("_") + 1);
            TextMeshProUGUI text_ = child.GetComponentInChildren<TextMeshProUGUI>();

            if (text_ != null)
                text_.text = bName;

#if UNITY_EDITOR
            if (Application.isEditor && !EditorApplication.isPlaying)
            {
                child.SetActive(true);
                DeactivateGameObjectInSeconds(child, 10);
            }
#endif
        }
    }

    [Button(ButtonSizes.Large)]
    private void BuildButtons()
    {
        BuildSingleButton();
        BuildTwinButton();
        BuildSpecialButton();
    }

    public void OnPointerDownDelegate(PointerEventData data, string name_)
    {
        ks.alphabetFunction(name_);
    }

    private void ChangeTextColorToWhite(PointerEventData data, TextMeshProUGUI textComponent)
    {
        textComponent.color = Color.white;
    }

    private void ChangeTextColorToDefault(PointerEventData data, TextMeshProUGUI textComponent)
    {
        textComponent.color = textColor;
    }

    private async void DeactivateGameObjectInSeconds(GameObject obj, int seconds)
    {
        await Task.Delay(seconds * 1000);
        obj.SetActive(false);
    }
}