//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Prompt : MonoBehaviour
{
    [SerializeField] private TMP_InputField promptInput;
    [SerializeField] private InputHelper promptInputHelper;

    [SerializeField] private UnityEvent onPromptAccepted, onPromptDeclinedProfanity;
    
    private void Start()
    {
        if (NetworkManager.Singleton.IsServer) return;
        
        promptInputHelper.OnUserEnteredMessage += SubmitPrompt;
        PlayerData.LocalPlayerData.OnPromptResponse += ProcessPromptResponse;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton.IsServer) return;
        
        promptInputHelper.OnUserEnteredMessage -= SubmitPrompt;
        if(PlayerData.LocalPlayerData != null) PlayerData.LocalPlayerData.OnPromptResponse -= ProcessPromptResponse;
    }

    public void SubmitPrompt()
    {
        PlayerData.LocalPlayerData.SetPrompt(promptInput.text);
    }

    public void SubmitPrompt(string newPrompt)
    {
        PlayerData.LocalPlayerData.SetPrompt(newPrompt);
    }

    private void ProcessPromptResponse(PlayerData.PromptResponse response)
    {
        if (response == PlayerData.PromptResponse.Declined_Profanity)
        {
            onPromptDeclinedProfanity?.Invoke();
        }
        else if (response == PlayerData.PromptResponse.Accepted)
        {
            onPromptAccepted?.Invoke();
        }
    }
}