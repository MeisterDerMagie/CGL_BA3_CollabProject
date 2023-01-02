//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LobbyIsFullCheck : MonoBehaviour
{
    private bool _wasFullBefore = false;
    
    private void Update()
    {
        if (NetworkManager.Singleton == null) return;
        
        //only run on server
        if (!NetworkManager.Singleton.IsServer) return;

        //if server became full --> inform server provider about it
        if (ConnectionApproval.Instance.ServerIsFull && !_wasFullBefore)
        {
            ServerProviderCommunication.Instance.ServerIsFull();
            _wasFullBefore = true;
        }
        
        //if server became not full --> inform server provider about it
        else if (!ConnectionApproval.Instance.ServerIsFull && _wasFullBefore)
        {
            ServerProviderCommunication.Instance.ServerIsNotFull();
            _wasFullBefore = false;
        }
    }
}