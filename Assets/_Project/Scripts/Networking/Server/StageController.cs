using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MEC;
using Unity.Netcode;
using UnityEngine;

public class StageController : NetworkBehaviour
{
    [SerializeField] public List<Podium> podiums = new();

    public event Action OnInitialized = delegate {  };
    
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
        
        //init assignedClientsList with maxValue (maxValue == no player assigned)
        foreach (Podium podium in podiums)
        {
            _clientIdsAssignToPodiums.Add(ulong.MaxValue);
        }
        
        //sort players by total points (most points is first one int the list)
        List<NetworkClient> connectedClientsOrderedByPoints = NetworkManager.ConnectedClientsList.OrderByDescending(networkClient=>networkClient.PlayerObject.GetComponent<PlayerData>().TotalPoints).ToList();

        //set up the stage (the player with the most points is in the center)
        for (int i = 0; i < connectedClientsOrderedByPoints.Count; i++)
        {
            ulong clientId = connectedClientsOrderedByPoints[i].ClientId;
            NetworkObject player = connectedClientsOrderedByPoints[i].PlayerObject;
            PlayerData playerData = player.GetComponent<PlayerData>();
            Podium podium = podiums[i];
            
            //activate podium
            podium.SetActive(true);
            
            //assign playerName to podium
            podium.AssignPlayerNameClientRpc(playerData.PlayerName);
            
            //set player name color (if it's the local player)
            podium.SetTextColorPlayerNameClientRpc(Constants.ownPlayerNameColor, clientId);

            //set assigned player
            _clientIdsAssignToPodiums[i] = clientId;
            
            //if we're in the voting stage, set the prompt and the composition
            var podiumPlayComposition = podium.GetComponent<Podium_PlayComposition>();
            if (podiumPlayComposition != null)
            {
                podiumPlayComposition.SetComposition(playerData.Recording);
            }

            var podiumPromptDisplay = podium.GetComponent<Podium_PromptDisplay>();
            if (podiumPromptDisplay != null)
            {
                podiumPromptDisplay.AssignedPrompt.Value = playerData.AssignedPrompt;
            }

            var playerVisuals = player.GetComponent<PlayerVisuals>();
            
            //position player on podium
            playerVisuals.SetPosition(podium.PlayerPosition);
            
            //set player sort order
            playerVisuals.SetSortOrderClientRpc((uint)i);
                
            //set player visible
            playerVisuals.isVisible.Value = true;
        }
        
        //call initialized event
        OnInitialized?.Invoke();
    }

    public override void OnDestroy()
    {
        _clientIdsAssignToPodiums.Dispose();
    }
}