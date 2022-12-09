//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using Wichtel.Extensions;

[RequireComponent(typeof(TMP_InputField))]
public class InputHelper : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private TMP_InputField inputField;

    //return true if the input field is focused and the user pressed enter, otherwise return false
    private bool UserSubmittedInputField => inputField.isFocused && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter));
    
    public Action<string> OnUserEnteredMessage = delegate(string _message) {  };

    private void Start() => ClearInputFieldAndFocusIt();

    private void OnEnable() => ClearInputFieldAndFocusIt();

    private void Update()
    {
        //don't allow the user to start the input text with white space or submit empty text
        if (inputField.text.IsNullOrWhitespace())
        {
            ClearInputField();
            return;
        }
        
        //if the user pressed Enter...
        if (!UserSubmittedInputField) return;
        //...remove line breaks from input text...
        string input = inputField.text;
        input = Regex.Replace(input, @"\r\n?|\n", string.Empty);
        //...send input...
        OnUserEnteredMessage?.Invoke(input);
        //...reset input field after submission
        ClearInputField();
    }

    private void ClearInputFieldAndFocusIt()
    {
        ClearInputField();
        FocusInputField();
    }

    private void ClearInputField() => inputField.SetTextWithoutNotify(string.Empty);

    private void FocusInputField()
    {
        inputField.Select();
        inputField.ActivateInputField();
    }
    
    #if UNITY_EDITOR
    private void OnValidate() => inputField = GetComponent<TMP_InputField>();
    #endif
}