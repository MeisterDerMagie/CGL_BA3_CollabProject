//(c) copyright by Martin M. Klöckener
using System;
using Unity.Netcode;
using UnityEngine;

public class TEST_ClientConnection : NetworkBehaviour
{
    private void Update()
    {
        Debug.Log(NetworkManager.IsConnectedClient);
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("Spawn");
    }
}