//(c) copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UISetReadyState : MonoBehaviour
{
    public void Ready()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("The server can't ready up.");
            return; 
        }
        
        PlayerLobbyData.LocalPlayerLobbyData.SetReadyState(true);
    }

    public void Unready()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("The server can't unready.");
            return;
        }
        
        PlayerLobbyData.LocalPlayerLobbyData.SetReadyState(false);
    }
}