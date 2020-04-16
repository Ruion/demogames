using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class KeyboardTwinButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float maxHoldTime = 1.2f;

    public GameObject clickedBG, characterOptions;
    public Color32 textColor;

    private bool showCharacterOptions;

    [SerializeField]
    private TextMeshProUGUI firstText, secondText;

    [HideInInspector]
    public KeyboardScript ks;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!characterOptions.activeInHierarchy)
            clickedBG.SetActive(true);

        showCharacterOptions = true;
        Invoke("ShowCharacterOption", maxHoldTime);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (clickedBG.activeInHierarchy)
        {
            clickedBG.SetActive(false);
            string text = name.Substring(name.IndexOf("_") + 1, 1);
            ks.alphabetFunction(text);
        }

        showCharacterOptions = false;
    }

    private void ShowCharacterOption()
    {
        if (showCharacterOptions)
        {
            clickedBG.SetActive(false);
            showCharacterOptions = false;
            characterOptions.SetActive(true);
            ks.CharacterOption = characterOptions;
        }
    }

    public void ResetState()
    {
        firstText.color = Color.white;
        secondText.color = textColor;
    }
}