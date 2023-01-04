//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WaitingScreenController_Generic : WaitingScreenController
{
    //call this on the client if they are ready
    public void Ready()
    {
        ReadyServerRpc(NetworkManager.LocalClientId);
    }
    
    //call this along with Ready()
    public override void Show()
    {
        transform.localScale = Vector3.one;
    }

    private void Start()
    {
        transform.localScale = Vector3.zero;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ReadyServerRpc(ulong clientId)
    {
        SetDoneState(clientId, true);
    }
}