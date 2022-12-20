//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WaitingScreenController_Prompt : WaitingScreenController
{
    //this gets only called on the client
    public override void Show()
    {
        transform.localScale = Vector3.one;
    }

    private void Start()
    {
        transform.localScale = Vector3.zero;

        if (!NetworkManager.Singleton.IsServer) return;
        
        PlayerData.OnPromptResponseServer += ProcessPromptResponseServer;
    }
    
    private void ProcessPromptResponseServer(ulong clientId, PlayerData.PromptResponse promptResponse)
    {
        if (promptResponse != PlayerData.PromptResponse.Accepted) return;
        
        SetDoneState(clientId, true);
    }
}