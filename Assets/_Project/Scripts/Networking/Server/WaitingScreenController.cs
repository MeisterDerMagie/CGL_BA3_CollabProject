//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class WaitingScreenController : NetworkBehaviour
{
    [SerializeField] private ScreenWaitingForOtherPlayers waitingScreen;

    public abstract void Show();

    private void Initialize()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        
        var connectedClients = NetworkManager.Singleton.ConnectedClients;
        var players = new Dictionary<ulong, uint>();

        foreach (var client in connectedClients)
        {
            players.Add(client.Key, client.Value.PlayerObject.GetComponent<PlayerData>().CharacterId);
        }
        
        waitingScreen.Initialize(players);
    }

    protected void SetDoneState(ulong clientId, bool isDone)
    {
        waitingScreen.SetDoneState(clientId, isDone);
    }
    
    private void OnClientDisconnected(ulong clientId)
    {
        //treat disconnected clients as "done"
        SetDoneState(clientId, true);
    }

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer) return;
        Initialize();
    }
}