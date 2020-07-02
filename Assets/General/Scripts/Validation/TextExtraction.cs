using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class TextExtraction : MonoBehaviour
{
    public string[] textLines { get; set; }

    public TMP_InputField field;
    public InputField infield;

    [SerializeField]
    private int minimumTextCount = 50;

    private string stringContent;

    [System.Serializable]
    public struct TextFields
    {
        public TMP_InputField field;
        public string propertyName;
    }

    public TextFields[] textFields;

    private void Update()
    {
        /*foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(vKey))
            {
                //your code here
                //Debug.Log(vKey);
                if (vKey != KeyCode.LeftArrow && vKey != KeyCode.RightArrow && vKey != KeyCode.Return
                    && vKey != KeyCode.UpArrow
                    && vKey != KeyCode.DownArrow
                    && vKey != KeyCode.Mouse0
                    && vKey != KeyCode.Mouse1
                    && vKey != KeyCode.Space
                    && vKey != KeyCode.LeftControl
                    && vKey != KeyCode.RightControl
                    && vKey != KeyCode.LeftAlt
                    && vKey != KeyCode.RightAlt
                    && vKey != KeyCode.LeftShift
                    && vKey != KeyCode.RightShift
                    )
                {
                }
            }
        }*/

        if (Input.GetKey(KeyCode.Return))
        {
            //ExtractJsonValueToTMP_InputField();
            ExtractJsonValueToInputField();
        }
    }

    public void ExtractJsonValueToInputField()
    {
        stringContent = infield.text;

        if (string.IsNullOrEmpty(stringContent) || stringContent.Length < minimumTextCount) return;

        stringContent = stringContent.Split('|')[1].Trim().Replace(" ", string.Empty);

        if (stringContent.Length % 4 != 0) return;
        stringContent = stringContent.FromBase64String();

        if (!stringContent.EndsWith("}")) { return; }

        JObject jsonObject = stringContent.JObjectFromString();

        for (int t = 0; t < textFields.Length; t++)
        {
            textFields[t].field.text = jsonObject[$"{textFields[t].propertyName}"]
                .ToString();
        }
    }

    public void ExtractJsonValueToTMP_InputField()
    {
        stringContent = field.text;

        if (string.IsNullOrEmpty(stringContent) || stringContent.Length < minimumTextCount) return;

        stringContent = stringContent.Split('|')[1].Trim().Replace(" ", string.Empty);

        if (stringContent.Length % 4 != 0) return;
        stringContent = stringContent.FromBase64String();

        if (!stringContent.EndsWith("}")) { return; }

        JObject jsonObject = stringContent.JObjectFromString();

        for (int t = 0; t < textFields.Length; t++)
        {
            textFields[t].field.text = jsonObject[$"{textFields[t].propertyName}"]
                .ToString();
        }
    }
}