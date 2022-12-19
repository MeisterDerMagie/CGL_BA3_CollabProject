//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Wichtel.Extensions;

public class PromptController : NetworkBehaviour
{
    [SerializeField] private TMP_InputField promptInput;
    [SerializeField] private InputHelper promptInputHelper;
    [SerializeField] private TextMeshProUGUI errorMessage;
    [SerializeField] private Button okButton;
    [SerializeField] private Transform acceptedPromptScreen;
    [SerializeField] private Timer timer;
    [SerializeField] private NetworkSceneLoader sceneToLoadAfterAllPlayersEnteredPrompt;

    [SerializeField] private UnityEvent onPromptAccepted, onPromptDeclined;

    private bool _nextSceneIsLoading = false;

    private void Start()
    {
        timer.OnTimerEndedServer += OnTimerEndedServer;
        
        if (NetworkManager.Singleton.IsServer) return;
        
        promptInputHelper.OnUserEnteredMessage += SubmitPrompt;
        PlayerData.LocalPlayerData.OnPromptResponse += ProcessPromptResponse;
        timer.OnTimerEndedClient += OnTimerEndedClient;
        
        errorMessage.gameObject.SetActive(false);
        acceptedPromptScreen.gameObject.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        
        if (NetworkManager.Singleton == null || NetworkManager.Singleton.IsServer) return;
        
        promptInputHelper.OnUserEnteredMessage -= SubmitPrompt;
        if(PlayerData.LocalPlayerData != null) PlayerData.LocalPlayerData.OnPromptResponse -= ProcessPromptResponse;
    }

    private void Update()
    {
        //check if all players submitted a prompt. If so, we can skip the rest of the timer and immediately switch to the next scene
        if (!IsServer || _nextSceneIsLoading) return;

        bool allPlayersSubmittedPrompt = true;
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (!client.PlayerObject.GetComponent<PlayerData>().SubmittedPrompt) allPlayersSubmittedPrompt = false;
        }
        
        //load next scene
        if (allPlayersSubmittedPrompt)
        {
            Timing.RunCoroutine(_WaitThenLoadNextScene());
        }
    }

    public void SubmitPrompt()
    {
        SetPrompt(promptInput.text);
    }

    public void SubmitPrompt(string input)
    {
        SetPrompt(input);
    }

    private void SetPrompt(string input)
    {
        //set prompt
        input = input.RemoveLineBreaks();
        PlayerData.LocalPlayerData.SetPrompt(input);
        
        //disable the input field and ok-button
        promptInput.interactable = false;
        okButton.interactable = false;
    }

    private void ProcessPromptResponse(PlayerData.PromptResponse response)
    {
        if (response == PlayerData.PromptResponse.Declined_EmptyString)
        {
            onPromptDeclined?.Invoke();
            
            //show error message
            errorMessage.gameObject.SetActive(true);
            
            //enable inputField and ok-button
            promptInput.interactable = true;
            okButton.interactable = true;
        }
        else if (response == PlayerData.PromptResponse.Declined_Profanity)
        {
            onPromptDeclined?.Invoke();
            
            //show error message
            errorMessage.gameObject.SetActive(true);
            
            //enable inputField and ok-button
            promptInput.interactable = true;
            okButton.interactable = true;
        }
        else if (response == PlayerData.PromptResponse.Accepted)
        {
            onPromptAccepted?.Invoke();
            acceptedPromptScreen.gameObject.SetActive(true);
        }
    }

    private void OnTimerEndedServer()
    {
        //load next scene
        Timing.RunCoroutine(_WaitThenLoadNextScene());
    }
    
    private void OnTimerEndedClient()
    {
        //disable the input field and ok-button
        promptInput.interactable = false;
        okButton.interactable = false;
        
        //if the player didn't input a prompt, use a default one
        if (string.IsNullOrWhiteSpace(PlayerData.LocalPlayerData.Prompt))
        {
            PlayerData.LocalPlayerData.SetPrompt("I didn't feel creative, so it's up to you.");
        }
        
        //show the endscreen of this section
        acceptedPromptScreen.gameObject.SetActive(true);
    }

    private IEnumerator<float> _WaitThenLoadNextScene()
    {
        //if the next scene is already loading (triggered before / from elsewhere), do nothing
        if(_nextSceneIsLoading) yield break;
        
        _nextSceneIsLoading = true;

        //wait a short time, ...
        yield return Timing.WaitForSeconds(2f);
        
        //then load the next scene
        NetworkSceneLoading.LoadNetworkScene(sceneToLoadAfterAllPlayersEnteredPrompt, LoadSceneMode.Single);
    }
}