using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class VoteCreativity : NetworkBehaviour
{
    [SerializeField] private int pointsFirstVote, pointsSecondVote, pointsThirdVote;
    [SerializeField] private StageController stageController;
    [SerializeField] private LightsController lightsController;
    
    //SERVER
    private Dictionary<ulong, List<ulong>> _awardedVotes = new();

    //CLIENT
    [SerializeField] private UnityEvent onUserTriedToVoteForThemselves; 
    private List<int> _awardedVotesClient = new();

    public override void OnNetworkSpawn()
    {
        _awardedVotesClient.Clear();
        
        if (!NetworkManager.IsServer) return;
        
        //set up dictionary
        _awardedVotes.Clear();
        foreach (ulong clientId in NetworkManager.ConnectedClientsIds)
        {
            _awardedVotes.Add(clientId, new List<ulong>());
        }
    }

    public void AwardVote(int podiumIndex)
    {
        //check if you already voted for three players
        if (_awardedVotesClient.Count >= 3) return;
        
        //check if the player tried to vote for themselves
        if (stageController.GetClientIdAssignedToPodium(podiumIndex) == NetworkManager.LocalClientId)
        {
            onUserTriedToVoteForThemselves.Invoke();
            Debug.Log("Player tried to vote for themselves");
            return;
        }
        
        //check if you already voted for this player
        if (_awardedVotesClient.Contains(podiumIndex)) return;

        //if you haven't voted for this player yet, award vote
        _awardedVotesClient.Add(podiumIndex);
        AwardVoteServerRpc(NetworkManager.LocalClientId, podiumIndex);
        
        Debug.Log("Award vote client");
        
        //show UI feedback
        lightsController.ForceLightPower(podiumIndex, 1f);
        
        //now, if you voted for three players, wait then show waiting screen
        if (_awardedVotesClient.Count >= 3)
        {
            Debug.LogWarning("ToDo: go to next stage");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AwardVoteServerRpc(ulong votingClient, int podiumIndex)
    {
        Debug.Log("award vote server");
        
        //the client who gets the points
        ulong clientWhoWasVotedFor = stageController.GetClientIdAssignedToPodium(podiumIndex);

        //the amount of points
        int points = GetVoteValue(votingClient);
        
        //award points
        foreach (var client in NetworkManager.ConnectedClients)
        {
            if (client.Key != clientWhoWasVotedFor) continue;

            client.Value.PlayerObject.GetComponent<PlayerData>().AddPointsCreativity(points);
            Debug.Log($"Player {client.Key.ToString()} received {points.ToString()} creativity points and now has a total of {client.Value.PlayerObject.GetComponent<PlayerData>().PointsCreativity.ToString()} creativity points.");
        }
        
        //remember who was voted for
        _awardedVotes[votingClient].Add(clientWhoWasVotedFor);
        
        //then if the voting player voted three times, show them the waiting screen
        if (_awardedVotes[votingClient].Count >= 3)
        {
            Debug.Log($"Player {votingClient.ToString()} voted three times. Show them the waiting screen.");
        }
    }

    private int GetVoteValue(ulong votingClient)
    {
        if (_awardedVotes[votingClient].Count == 0) return pointsFirstVote;
        if (_awardedVotes[votingClient].Count == 1) return pointsSecondVote;
        if (_awardedVotes[votingClient].Count == 2) return pointsThirdVote;
        return 0;
    }
}