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
    [SerializeField] private Transform notEnoughPlayersUI;
    [SerializeField] private NetworkSceneLoader sceneToLoadAfterCountdownFinished;

    [HideInInspector] public bool readyCountdownHasStarted = false;
    private NetworkVariable<float> countdown;
    private NetworkVariable<bool> enoughPlayersToStartTheGame;
    private CoroutineHandle coroutine;

    [ClientRpc]
    private void StartCountdownOnClientClientRpc() => ShowCountdownUI();

    [ClientRpc]
    private void StopCountdownOnClientClientRpc() => HideCountdownUI();

    private void Awake()
    {
        countdown = new NetworkVariable<float>();
        enoughPlayersToStartTheGame = new NetworkVariable<bool>();
    }

    public override void OnNetworkSpawn()
    {
        enoughPlayersToStartTheGame.OnValueChanged += OnEnoughPlayersChanged;
    }

    private void OnEnoughPlayersChanged(bool previousvalue, bool newvalue) => UpdateNotEnoughPlayersUI();

    private void Start()
    {
        HideCountdownUI();
        UpdateNotEnoughPlayersUI();
    }
    
    private void Update()
    {
        //if server
        if (NetworkManager.Singleton.IsServer)
        {
            //check if enough players are connected and if all players are ready
            if (NetworkManager.ConnectedClientsList.Count > 0)
            {
                bool enoughPlayersConnected = NetworkManager.ConnectedClientsList.Count >= Constants.MIN_PLAYER_AMOUNT;
                enoughPlayersToStartTheGame.Value = enoughPlayersConnected;
                
                bool allPlayersAreReady = true;
                foreach (var client in NetworkManager.ConnectedClientsList)
                {
                    if (!client.PlayerObject.GetComponent<PlayerLobbyData>().IsReadyInLobby)
                    {
                        allPlayersAreReady = false;
                        break;
                    }
                }
            
                if(enoughPlayersConnected && allPlayersAreReady && !readyCountdownHasStarted) StartCountdown();
                else if((!enoughPlayersConnected || !allPlayersAreReady) && readyCountdownHasStarted) StopCountdown();
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
        NetworkSceneLoading.LoadNetworkScene(sceneToLoadAfterCountdownFinished, LoadSceneMode.Single);
        //NetworkManager.SceneManager.LoadScene(sceneToLoadAfterCountdownFinished, LoadSceneMode.Single);
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
        
        //disallow new players from connecting, after the game started
        ConnectionApproval.Instance.gameIsInLobby = false;
        
        //inform server provider about the started game
        ServerProviderCommunication.Instance.ServerInGame();
        
        //load the next scene to start the game
        StartGame();
    }

    private void UpdateNotEnoughPlayersUI()
    {
        notEnoughPlayersUI.gameObject.SetActive(!enoughPlayersToStartTheGame.Value);
    }

    private void HideCountdownUI()
    {
        if (countdownUI == null) return;
        countdownUI.gameObject.SetActive(false);
    }

    private void ShowCountdownUI() => countdownUI.gameObject.SetActive(true);
    private string CountdownToString() => Mathf.Floor(countdown.Value).ToString(CultureInfo.InvariantCulture);
}