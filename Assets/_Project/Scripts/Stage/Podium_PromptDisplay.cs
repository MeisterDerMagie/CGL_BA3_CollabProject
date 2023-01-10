//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Podium_PromptDisplay : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI promptTextField;
    [SerializeField] private Transform promptView;

    public NetworkVariable<FixedString512Bytes> AssignedPrompt;

    private void Awake()
    {
        AssignedPrompt = new NetworkVariable<FixedString512Bytes>();
    }

    private void Start()
    {
        AssignedPrompt.OnValueChanged += OnPromptSet;
        Debug.Log($"Prompt: {AssignedPrompt.Value.Value}");
        promptTextField.SetText(AssignedPrompt.Value.Value);
        Hide();
    }

    private void OnPromptSet(FixedString512Bytes previousvalue, FixedString512Bytes newvalue)
    {
        Debug.Log($"OnPromptSet: {AssignedPrompt.Value.Value}");

        promptTextField.SetText(newvalue.Value);
    }

    public void Show()
    {
        promptView.gameObject.SetActive(true);
    }

    public void Hide()
    {
        promptView.gameObject.SetActive(false);
    }
}