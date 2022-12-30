using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using Unity.Netcode;
using UnityEngine;

public class StageController : NetworkBehaviour
{
    [SerializeField] private List<Podium> podiums = new();

    //Quasi Dictionary weil es NetworkDictionaries nicht gibt: die PodiumIndexes matchen die indexes von _clientIdsAssignToPodiums
    private NetworkList<ulong> _clientIdsAssignToPodiums;
    //---

    private void Awake()
    {
        _clientIdsAssignToPodiums = new NetworkList<ulong>();
    }

    public override void OnNetworkSpawn()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        Timing.RunCoroutine(_InitializeDelayed());
    }

    public ulong GetClientIdAssignedToPodium(int podiumIndex)
    {
        return _clientIdsAssignToPodiums[podiumIndex];
    }
    
    private IEnumerator<float> _InitializeDelayed()
    {
        yield return Timing.WaitForSeconds(0.5f);
        
        Initialize();
    }

    private void Initialize()
    {
        //only run on the server
        if (!NetworkManager.Singleton.IsServer) return;

        //initially hide all podiums
        foreach (Podium podium in podiums)
        {
            podium.SetActive(false);
        }
        
        //set up the stage
        for (int i = 0; i < NetworkManager.ConnectedClientsList.Count; i++)
        {
            ulong clientId = NetworkManager.ConnectedClientsList[i].ClientId;
            NetworkObject player = NetworkManager.ConnectedClientsList[i].PlayerObject;
            PlayerData playerData = player.GetComponent<PlayerData>();
            Podium podium = podiums[i];
            
            //activate podium
            podium.SetActive(true);
            
            //assign playerName to podium
            podium.AssignPlayerNameClientRpc(playerData.PlayerName);
            
            //set player name color (if it's the local player)
            podium.SetTextColorPlayerNameClientRpc(Constants.ownPlayerNameColor, clientId);

            //set assigned player
            _clientIdsAssignToPodiums.Add(clientId);
            
            //position player on podium
            player.GetComponent<PlayerVisuals>().SetPosition(podium.PlayerPosition);
            
            //set player visible
            player.GetComponent<PlayerVisuals>().isVisible.Value = true;
        }
    }

    public override void OnDestroy()
    {
        _clientIdsAssignToPodiums.Dispose();
    }
}