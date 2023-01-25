//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayabilityCalculation : NetworkBehaviour
{
    [SerializeField] private int maxPoints, pointsDecreasePerPlace;
    
    private Dictionary<string /*playerGuid*/, List<float> /*ratings*/> _ratings = new();

    public override void OnNetworkSpawn()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        _ratings.Clear();
        foreach (KeyValuePair<ulong,NetworkClient> client in NetworkManager.Singleton.ConnectedClients)
        {
            _ratings.Add(client.Value.PlayerObject.GetComponent<PlayerData>().ClientGuid.ToString(), new List<float>());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SubmitPlayabilityServerRpc(FixedString64Bytes performingPlayerGuid, FixedString64Bytes ratedPlayerGuid, float percent)
    {
        _ratings[ratedPlayerGuid.Value].Add(percent);
    }

    public override void OnDestroy()
    {
        //if this object gets destroyed we are changing to a new scene, which meanst that  all players have submitted their playability scores
        //so we set those to the PlayerDatas

        foreach (var client in NetworkManager.ConnectedClients)
        {
            var playerData = client.Value.PlayerObject.GetComponent<PlayerData>();
            string playerGuid = playerData.ClientGuid.ToString();
            if (!_ratings.ContainsKey(playerGuid))
            {
                Debug.LogError($"The client with the guid {playerGuid} apparently had not been rated any playability points. What happened?", this);
                return;
            }
            
            //calculate the average playability score
            float totalScore = 0f;
            int ratingCount = 0;
            foreach (float playabilityRating in _ratings[playerGuid])
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