using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MEC;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class VoteCreativity : NetworkBehaviour
{
    [SerializeField] private StageController stageController;
    [SerializeField] private LightsController lightsController;
    [SerializeField] private WaitingScreenController_VoteForCreativity waitingScreen;
    [SerializeField] private NetworkSceneLoader nextScene;
    
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
        
        //start coroutine to check if all votes were awarded
        Timing.RunCoroutine(_CheckIfAllVotesAreAwarded());
    }

    //check if all players awarded 3 votes or if not enough players are in the lobby to vote three times
    private IEnumerator<float> _CheckIfAllVotesAreAwarded()
    {
        bool noMoreVotesCanBeAwarded;
        bool allPlayersAwardedThreeVotes;
        
        do
        {
            noMoreVotesCanBeAwarded = true;
            allPlayersAwardedThreeVotes = true;
            
            int connectedClientsAmount = NetworkManager.ConnectedClients.Count;
            foreach (var client in _awardedVotes)
            {
                //keep voting going, as long as not all players awarded three votes...
                if (client.Value.Count < 3)
                {
                    allPlayersAwardedThreeVotes = false;
                }
                
                //... or as long as there are enough players to award votes to
                if (client.Value.Count < connectedClientsAmount-1)
                {
                    noMoreVotesCanBeAwarded = false;
                }
            }

            yield return Timing.WaitForOneFrame;

        }
        while (!noMoreVotesCanBeAwarded && !allPlayersAwardedThreeVotes);
        
        
        //if either all players awarded three votes or there are no more votes that can be awarded, load the next scene
        NetworkSceneLoading.LoadNetworkScene(nextScene, LoadSceneMode.Single);
    }

    //CLIENT
    public void AwardVote(int podiumIndex)
    {
        //check if you already voted for three players
        if (_awardedVotesClient.Count >= 3) return;
        
        //check if the player tried to vote for themselves
        if (stageController.GetClientIdAssignedToPodium(podiumIndex) == NetworkManager.LocalClientId)
        {
            onUserTriedToVoteForThemselves.Invoke();
            return;
        }
        
        //check if you already voted for this player
        if (_awardedVotesClient.Contains(podiumIndex)) return;

        //if you haven't voted for this player yet, award vote
        _awardedVotesClient.Add(podiumIndex);
        AwardVoteServerRpc(NetworkManager.LocalClientId, podiumIndex);
        
        //show UI feedback
        lightsController.ForceLightPower(podiumIndex, 1f);
        
        //now, if you voted for three players, wait then show waiting screen
        if (_awardedVotesClient.Count >= 3)
        {
            waitingScreen.Show();
        }
    }

    //SERVER
    [ServerRpc(RequireOwnership = false)]
    private void AwardVoteServerRpc(ulong votingClient, int podiumIndex)
    {
        //the client who gets the points
        ulong clientWhoWasVotedFor = stageController.GetClientIdAssignedToPodium(podiumIndex);

        //the amount of points
        int points = GetVoteValue(votingClient);
        
        //award points
        foreach (var client in NetworkManager.ConnectedClients)
        {
            if (client.Key != clientWhoWasVotedFor) continue;

            client.Value.PlayerObject.GetComponent<PlayerData>().AddPointsCreativity(points);
        }

        //remember who was voted for
        _awardedVotes[votingClient].Add(clientWhoWasVotedFor);
        
        //show the feedback on the client
        AwardedVoteSuccessfullyClientRpc(votingClient, podiumIndex, points);
        
        //then if the voting player voted three times, show them the waiting screen
        if (_awardedVotes[votingClient].Count >= 3)
        {
            waitingScreen.ClientAwardedAllVotes(votingClient);
        }
    }
    
    [ClientRpc]
    private void AwardedVoteSuccessfullyClientRpc(ulong votingClient, int podiumIndex, int awardedPoints)
    {
        if (votingClient != NetworkManager.Singleton.LocalClientId) return;
        stageController.podiums[podiumIndex].SetPodiumText($"+{awardedPoints.ToString()}");
    }

    private int GetVoteValue(ulong votingClient)
    {
        if (_awardedVotes[votingClient].Count == 0) return Constants.POINTS_CREATIVITY_FIRST_VOTE;
        if (_awardedVotes[votingClient].Count == 1) return Constants.POINTS_CREATIVITY_SECOND_VOTE;
        if (_awardedVotes[votingClient].Count == 2) return Constants.POINTS_CREATIVITY_THIRD_VOTE;
        return 0;
    }
}