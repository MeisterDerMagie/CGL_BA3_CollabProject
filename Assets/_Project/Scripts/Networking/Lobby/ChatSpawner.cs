using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ChatSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject chatPrefab;

    public override void OnNetworkSpawn()
    {
        if (!NetworkManager.IsServer) return;
        
        GameObject chat = Instantiate(chatPrefab, Vector3.zero, Quaternion.identity);
        chat.GetComponent<NetworkObject>().Spawn(destroyWithScene: false);
    }
}
