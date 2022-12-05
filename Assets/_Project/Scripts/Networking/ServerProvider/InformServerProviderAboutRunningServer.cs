using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InformServerProviderAboutRunningServer : NetworkBehaviour
{
    #if UNITY_SERVER
    public override void OnNetworkSpawn()
    {
        ServerProviderCommunication.Instance.ServerStarted();
    }
    #endif
}
