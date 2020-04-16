using UnityEngine;
using System;

public class KeyboardScript : MonoBehaviour
{
    #region Fields

    public AudioSource clickSound;
    private int selectionStartPost;
    private int selectionEndPost;
    private int selectionAmount;

    public Transform destinationPos_ { get { return destinationPos; } set { destinationPos = value; Reposition(); } }
    private Transform destinationPos;

    public GameObject characterOption = null;

    public GameObject CharacterOption
    {
        set
        {
            if (characterOption == null || characterOption == value) characterOption = value;
            else
            {
                characterOption.SetActive(false);
                characterOption = value;
            }
        }
    }

    private string copyText;

    private void OnEnable()
    {
        if (Application.platform == RuntimePlatform.Android)
            gameObject.SetActive(false);
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            gameObject.SetActive(false);
    }

    public TMPro.TMP_InputField inputFieldTMPro_
    {
        get { return inputFieldTMPro; }
        set
        {
            inputFieldTMPro = value;
            inputFieldTMPro.onFocusSelectAll = false;
            gameObject.SetActive(true);

            currentField = value;
        }
    }

    private TMPro.TMP_InputField currentField;

    public TMPro.TMP_InputField inputFieldTMPro;
    public GameObject lowercaseLayout, uppercaseLayout;

    #endregion Fields

    // insert character(s) to right or replace selected characters
    public void alphabetFunction(string alphabet)
    {
        // Play typing sound
        clickSound.Play();

        bool canType = true;

        if (inputFieldTMPro.contentType == TMPro.TMP_InputField.ContentType.IntegerNumber)
        {
            // No typing if type alphabet to digit field
            int out_;
            if (!int.TryParse(alphabet, out out_)) canType = false;
        }

        if (canType)
        {
            if (!SelectionFocus())
            {
                // insert text to right if no text selected
                inputFieldTMPro.text = inputFieldTMPro.text.Insert(inputFieldTMPro.stringPosition, alphabet);
                inputFieldTMPro.stringPosition += alphabet.Length;
            }
            else
            {
                // remove selected text
                RemoveSelectionTexts();

                // insert text at removed text postion
                inputFieldTMPro.text = inputFieldTMPro.text.Insert(inputFieldTMPro.stringPosition, alphabet);
                inputFieldTMPro.stringPosition += alphabet.Length;
            }
        }

        // focus back to the input field
        inputFieldTMPro.Select();
    }

    /// <summary>
    /// Remove selected characters or remove 1 character to left
    /// </summary>
    public void BackSpace()
    {
        if (inputFieldTMPro.text.Length < 0) return;

        clickSound.Play();

        //  if (inputFieldTMPro.text.Length>0) inputFieldTMPro.text= inputFieldTMPro.text.Remove(inputFieldTMPro.text.Length-1); // SAFE

        int cutPos = inputFieldTMPro.stringPosition - 1;

        if (!SelectionFocus())
        {
            // no backspace if no text
            if (inputFieldTMPro.stringPosition - 1 < 0) return;

            // remove 1 character from the current text in input
            string newText = inputFieldTMPro.text.Remove(cutPos, 1);

            // replace the text into input field
            inputFieldTMPro.text = newText;

            // reassign new typing position to new text right most position
            inputFieldTMPro.stringPosition = cutPos;
        }
        else
        {
            RemoveSelectionTexts();
        }

        inputFieldTMPro.Select();
    }

    /// <summary>
    /// Check user is selecting characters or not
    /// </summary>
    /// <returns></returns>
    private bool SelectionFocus()
    {
        // Check user is select characters from left ?
        if (inputFieldTMPro.selectionStringAnchorPosition < inputFieldTMPro.selectionStringFocusPosition)
        {
            selectionStartPost = inputFieldTMPro.selectionStringAnchorPosition;
            selectionEndPost = inputFieldTMPro.selectionStringFocusPosition;
        }
        else // Check user is select characters from right ?
        {
            selectionStartPost = inputFieldTMPro.selectionStringFocusPosition;
            selectionEndPost = inputFieldTMPro.selectionStringAnchorPosition;
        }

        // get amount of selected character
        selectionAmount = selectionEndPost - selectionStartPost;

        // if selected character have 1 or more return true
        if ((selectionAmount) >= 1) return true;

        return false;
    }

    /// <summary>
    /// Remove selected text(s)
    /// </summary>
    private void RemoveSelectionTexts()
    {
        inputFieldTMPro.text = inputFieldTMPro.text.Remove(selectionStartPost, selectionAmount);
        inputFieldTMPro.stringPosition = selectionStartPost;
        inputFieldTMPro.onValueChanged.Invoke(inputFieldTMPro.text);
    }

    /// <summary>
    /// Copy selected text
    /// </summary>
    public void CopyText()
    {
        clickSound.Play();

        if (!SelectionFocus()) return;

        // get text between selection
        copyText = inputFieldTMPro.text.Substring(selectionStartPost, selectionAmount);
        Debug.Log(name + "- Copied text : " + copyText);
    }

    /// <summary>
    /// Paste selected text to right or replace selected text
    /// </summary>
    public void PasteText()
    {
        clickSound.Play();

        if (string.IsNullOrEmpty(copyText)) return;

        bool canType = true;

        if (inputFieldTMPro.contentType == TMPro.TMP_InputField.ContentType.IntegerNumber)
        {
            // No typing if type alphabet to digit field
            int out_;
            if (!int.TryParse(copyText, out out_)) canType = false;
        }

        if (canType)
        {
            if (!SelectionFocus())
            {
                // insert text to right if no text selected
                inputFieldTMPro.text = inputFieldTMPro.text.Insert(inputFieldTMPro.stringPosition, copyText);
                inputFieldTMPro.stringPosition += copyText.Length;
            }
            else
            {
                // remove selected text
                RemoveSelectionTexts();

                // insert text at removed text postion
                inputFieldTMPro.text = inputFieldTMPro.text.Insert(inputFieldTMPro.stringPosition, copyText);
                inputFieldTMPro.stringPosition += copyText.Length;
            }
        }

        // focus back to the input field
        inputFieldTMPro.Select();
    }

    /// <summary>
    // Deactivate all currently available layout gameobjects
    /// </summary>
    public void CloseAllLayouts()
    {
        lowercaseLayout.SetActive(false);
        uppercaseLayout.SetActive(false);
    }

    /// <summary>
    /// Activate selected layout gameobject
    /// </summary>
    /// <param name="SetLayout"></param>
    public void ShowLayout(GameObject SetLayout)
    {
        clickSound.Play();

        if (currentField != inputFieldTMPro_)
        {
            CloseAllLayouts();
            SetLayout.SetActive(true);
            currentField = inputFieldTMPro_;
        }
    }

    public void SwitchLayout()
    {
        clickSound.Play();
        if (lowercaseLayout.activeInHierarchy)
        {
            lowercaseLayout.SetActive(false);
            uppercaseLayout.SetActive(true);
        }
        else
        {
            lowercaseLayout.SetActive(true);
            uppercaseLayout.SetActive(false);
        }
    }

    /// <summary>
    /// Position keyboard to the selected transform (change variable destinationPos_ to invoke this function)
    /// </summary>
    private void Reposition()
    {
        transform.position = destinationPos.position;
    }
}