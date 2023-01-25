//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayabilityCalculation : NetworkBehaviour
{    
    private Dictionary<ulong /*playerId*/, List<float> /*ratings*/> _ratings = new();
    private List<ulong> _donePlayers = new();
    [SerializeField] private NetworkSceneLoader nextScene;

    public override void OnNetworkSpawn()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        _ratings.Clear();
        foreach (KeyValuePair<ulong,NetworkClient> client in NetworkManager.Singleton.ConnectedClients)
        {
            ulong playerGuid = client.Value.PlayerObject.GetComponent<PlayerData>().ClientIdentifier;
            _ratings.Add(playerGuid, new List<float>());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SubmitPlayabilityServerRpc(ulong performingPlayer, ulong ratedPlayer, float percent)
    {
        _ratings[ratedPlayer].Add(percent);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DoneServerRpc(ulong clientId)
    {
        _donePlayers.Add(clientId);
        
        //then if all players are done, load the next scene
        if(AllPlayersAreReady()) NetworkSceneLoading.LoadNetworkScene(nextScene, LoadSceneMode.Single);
    }

    private bool AllPlayersAreReady()
    {
        foreach (ulong client in NetworkManager.ConnectedClientsIds)
        {
            if (!_donePlayers.Contains(client)) return false;
        }

        return true;
    }

    public override void OnDestroy()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        
        //if this object gets destroyed we are changing to a new scene, which meanst that  all players have submitted their playability scores
        //so we set those to the PlayerDatas

        foreach (var client in NetworkManager.ConnectedClients)
        {
            var playerData = client.Value.PlayerObject.GetComponent<PlayerData>();
            ulong clientId = playerData.ClientIdentifier;
            if (!_ratings.ContainsKey(clientId))
            {
                Debug.LogError($"The client with the guid {clientId} apparently had not been rated any playability points. What happened?", this);
                return;
            }
            
            //calculate the average playability score
            float totalScore = 0f;
            int ratingCount = 0;
            foreach (float playabilityRating in _ratings[clientId])
            {
                ratingCount++;
                totalScore += playabilityRating;
            }

            float averageRating = totalScore / (float)ratingCount;

            //set it to the PlayerData
            playerData.AddPointsPlayabilityPercent(averageRating);
        }
        
        
        base.OnDestroy();
    }
}