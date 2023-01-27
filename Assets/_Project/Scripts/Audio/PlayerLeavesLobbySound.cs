using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using Unity.Netcode;

public class PlayerLeavesLobbySound : NetworkBehaviour
{
    [SerializeField] private EventReference aww;

    void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnDestroy()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientID)
    {
        PlayApplausClientRpc();
    }

    [ClientRpc]
    private void PlayApplausClientRpc()
    {
        RuntimeManager.PlayOneShot(aww);
    }
}
