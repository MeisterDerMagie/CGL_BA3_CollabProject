using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(NetworkManager))]
public class NetworkManagerHUD : MonoBehaviour
{
    private NetworkManager _manager;

    [SerializeField] private int offsetX, offsetY;

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10 + offsetX, 10 + offsetY, 300, 300));
        
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
            StopButtons();
        }
        
        //Connection status
        var connectionStatus = ConnectionStatus.Unknown;
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsConnectedClient)
            connectionStatus = ConnectionStatus.Disconnected;
        if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsConnectedClient)
            connectionStatus = ConnectionStatus.Client_AttemptConnection;
        else if (NetworkManager.Singleton.IsConnectedClient)
            connectionStatus = ConnectionStatus.Client_Connected;
        else if (NetworkManager.Singleton.IsServer)
            connectionStatus = ConnectionStatus.Server_Running;

        switch (connectionStatus)
        {
            case ConnectionStatus.Disconnected:
                GUILayout.Label("<color=orange>Disconnected</color>");
                break;
            case ConnectionStatus.Client_AttemptConnection:
                GUILayout.Label("<color=yellow>Connecting client</color>");
                break;
            case ConnectionStatus.Client_Connected:
                GUILayout.Label("<color=green>Client connected</color>");
                break;
            case ConnectionStatus.Server_Running:
                GUILayout.Label("<color=green>Server running</color>");
                break;
            case ConnectionStatus.Unknown:
                GUILayout.Label("Unknown connection status");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        GUILayout.EndArea();
    }

    private static void StartButtons()
    {
        //if (GUILayout.Button("Start local Host")) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Start local Client")) NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("Start local Server")) NetworkManager.Singleton.StartServer();
    }

    private static void StopButtons()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            if(GUILayout.Button("Stop Host")) NetworkManager.Singleton.Shutdown();
            return;
        }

        if(NetworkManager.Singleton.IsServer)
            if(GUILayout.Button("Stop Server")) NetworkManager.Singleton.Shutdown();
        
        if(NetworkManager.Singleton.IsClient)
            if(GUILayout.Button("Stop Client")) NetworkManager.Singleton.Shutdown();
    }

    private static void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
                        NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }

    private enum ConnectionStatus
    {
        Unknown,
        Disconnected,
        Client_AttemptConnection,
        Client_Connected,
        Server_Running
    }
}