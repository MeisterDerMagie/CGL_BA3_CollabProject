//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using MEC;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReadyCountdown : NetworkBehaviour
{
    [SerializeField] private float countdownDuration = 4.99f;
    [SerializeField] private Transform countdownUI;
    [SerializeField] private TextMeshProUGUI countdownNumberText;
    [SerializeField] private SceneReference sceneToLoadAfterCountdownFinished;

    [HideInInspector] public bool readyCountdownHasStarted = false;
    private NetworkVariable<float> countdown;
    private CoroutineHandle coroutine;

    [ClientRpc]
    private void StartCountdownOnClientClientRpc() => ShowCountdownUI();

    [ClientRpc]
    private void StopCountdownOnClientClientRpc() => HideCountdownUI();

    private void Awake()
    {
        countdown = new NetworkVariable<float>();
    }

    private void Start()
    {
        HideCountdownUI();
    }
    
    private void Update()
    {
        //if server
        if (NetworkManager.Singleton.IsServer)
        {
            //check if all players are ready
            if (NetworkManager.ConnectedClientsList.Count > 0)
            {
                bool allPlayersAreReady = true;
                foreach (var client in NetworkManager.ConnectedClientsList)
                {
                    if (!client.PlayerObject.GetComponent<PlayerLobbyData>().IsReadyInLobby)
                    {
                        allPlayersAreReady = false;
                        break;
                    }
                }
            
                if(allPlayersAreReady && !readyCountdownHasStarted) StartCountdown();
                else if(!allPlayersAreReady && readyCountdownHasStarted) StopCountdown();
            }
        }

        //if client
        else
        {
            //update ui
            countdownNumberText.SetText(CountdownToString());
        }
    }

    public void StartCountdown()
    {
        readyCountdownHasStarted = true;
        
        //reset countdown
        countdown.Value = countdownDuration;
        
        //show UI
        ShowCountdownUI();

        //run coroutine
        coroutine = Timing.RunCoroutine(_TickCountdown());
        
        //show countdown on clients
        StartCountdownOnClientClientRpc();
    }

    public void StopCountdown()
    {
        readyCountdownHasStarted = false;
        
        Timing.KillCoroutines(coroutine);

        HideCountdownUI();
        
        //stop countdown on clients
        StopCountdownOnClientClientRpc();
    }

    private void StartGame()
    {
        NetworkManager.SceneManager.LoadScene(sceneToLoadAfterCountdownFinished, LoadSceneMode.Single);
    }

    private IEnumerator<float> _TickCountdown()
    {
        //keep going as long as the counter is bigger than 1
        while (countdown.Value > 1f)
        {
            //update countdown
            countdown.Value -= 1f;
        
            //update UI
            countdownNumberText.SetText(CountdownToString());
            
            yield return Timing.WaitForSeconds(1);
        }
        
        //if countdown reached 0
        //hide countdown
        HideCountdownUI();
        
        //inform server provider about the started game
        ServerProviderCommunication.Instance.ServerInGame();
        
        //load the next scene to start the game
        StartGame();
    }

    private void HideCountdownUI()
    {
        if (countdownUI == null) return;
        countdownUI.gameObject.SetActive(false);
    }

    private void ShowCountdownUI() => countdownUI.gameObject.SetActive(true);
    private string CountdownToString() => Mathf.Floor(countdown.Value).ToString(CultureInfo.InvariantCulture);
}