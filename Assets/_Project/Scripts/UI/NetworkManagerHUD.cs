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
}