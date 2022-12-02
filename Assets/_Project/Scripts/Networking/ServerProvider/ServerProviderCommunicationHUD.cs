//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerProviderCommunicationHUD : MonoBehaviour
{
    [SerializeField] private int offsetX, offsetY;
    private string _lobbyCode;
    
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10 + offsetX, 10 + offsetY, 300, 300));

        GUILayout.Label(ServerProviderClient.Connected ? "<color=green>Servers online</color>" : "<color=red>Servers offline</color>");

        //don't show buttons if servers are offline or if the client is connected
        if (!ServerProviderClient.Connected || NetworkManager.Singleton.IsClient)
        {
            GUILayout.EndArea();
            return;
        }
        
        //show buttons
        if(GUILayout.Button("Host Lobby"))
            ServerProviderCommunication.Instance.HostRequest();
        
        GUILayout.BeginHorizontal();
        _lobbyCode = GUILayout.TextField(_lobbyCode);
        if (GUILayout.Button("Join Lobby")) ServerProviderCommunication.Instance.JoinRequest(_lobbyCode);
        GUILayout.EndHorizontal();
        
        GUILayout.EndArea();
    }
}