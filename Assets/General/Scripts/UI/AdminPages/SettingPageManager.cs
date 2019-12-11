using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using TMPro;

public class SettingPageManager : MonoBehaviour
{
    public JSONSetter jsonSetter;
    public GameObject settingFieldPrefab;
    public Transform settingFieldContainer;

    public GameObject popUpModal;
    public Button doneButton;

    private TextMeshProUGUI selectedField;

    public TextMeshProUGUI selectedField_
    {
        get {return selectedField;}
        set
        {
            selectedField = value;
            propertyTitle.text = selectedField.text;
            valueField.text = 
            jsonSetter.LoadSetting()[selectedField.text].ToString();
            popUpModal.SetActive(true);
        }
    }

    public TextMeshProUGUI propertyTitle;
    public TMP_InputField valueField;

    [ContextMenu("LoadGlobaSettings")]
    void OnEnable()
    {
        if(settingFieldContainer.childCount < 1) {

        if(jsonSetter == null) jsonSetter = FindObjectOfType<JSONSetter>();
        JObject globalSettings = jsonSetter.LoadSetting();

        foreach (JProperty p in globalSettings.Properties())
        {
            GameObject newField = Instantiate(settingFieldPrefab, settingFieldContainer);
            
            // assign property name
            TextMeshProUGUI property = newField.GetComponent<TextMeshProUGUI>();
            property.text = p.Name;

            // assign On Click () event ot btn
            Button btn = newField.GetComponentInChildren<Button>();
            btn.onClick.AddListener(delegate
            {
                selectedField_ = btn.GetComponentInParent<TextMeshProUGUI>();
            });

            // assign value to text
            TextMeshProUGUI valueText = btn.GetComponentInChildren<TextMeshProUGUI>();
            valueText.text = p.Value.ToString();
        }
        }
    }

    public void UpdateProperty()
    {
        jsonSetter.UpdateSetting(selectedField.text, valueField.text);
        selectedField.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text
         = jsonSetter.LoadSetting()[selectedField.text].ToString();
    }

    private void RefreshField()
    {

    }

    public void ValidateField()
    {
        if(string.IsNullOrEmpty(valueField.text)) doneButton.interactable = false;
        else doneButton.interactable = true;
    }
}
