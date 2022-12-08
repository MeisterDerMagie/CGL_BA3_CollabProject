using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerDebugInfos : NetworkBehaviour
{
    #if UNITY_SERVER
    private void Awake()
    {
        NetworkManager.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.OnClientDisconnectCallback += OnClientDisconnected;
    }

    public override void OnDestroy()
    {
        NetworkManager.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    private void OnClientConnected(ulong id) => Debug.Log($"Player connected. Id: {id.ToString()}");
    private void OnClientDisconnected(ulong id) => Debug.Log($"Player disconnected. Id: {id.ToString()}");

    #endif
}
