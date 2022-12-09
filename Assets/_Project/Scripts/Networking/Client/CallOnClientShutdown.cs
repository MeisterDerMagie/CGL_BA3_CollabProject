//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CallOnClientShutdown : NetworkBehaviour
{
    //NetworkManager.Singleton.OnClientConnectedCallback doesn't work smh...
    public override void OnDestroy()
    {
        if(IsLocalPlayer) NetworkEvents.OnClientShutdown?.Invoke();
    }

}