using TMPro;
using UnityEngine;

public class PlayerPrefsSaver_DropDownInput : PlayerPrefsSaver
{
    public TMP_Dropdown dropDown;
    public TMP_InputField inputField;

    public string prefix = "+6";

    public void SaveCombineField()
    {
        PlayerPrefs.SetString(name_, prefix + dropDown.value + inputField.text);
    }
}
