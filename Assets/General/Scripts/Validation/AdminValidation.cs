using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class AdminValidation : MonoBehaviour
{
    public string password = "hondaBoss";
    public TMP_InputField passwordInput;

    public UnityEvent OnPasswordCorrect;


    public virtual void Validate()
    {
        if(passwordInput.text == password)
        {
            OnPasswordCorrect.Invoke();
            passwordInput.text = "";
           
        }
    }
}
