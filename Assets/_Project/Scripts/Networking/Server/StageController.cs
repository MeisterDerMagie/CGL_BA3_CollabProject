using System.Collections;
using System.Collections.Generic;
using MEC;
using Unity.Netcode;
using UnityEngine;

public class StageController : NetworkBehaviour
{
    [SerializeField] private List<Podium> podiums = new();

    public override void OnNetworkSpawn()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        Timing.RunCoroutine(_InitializeDelayed());
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
            NetworkObject player = NetworkManager.ConnectedClientsList[i].PlayerObject;
            Podium podium = podiums[i];
            
            //activate podium
            podium.SetActive(true);
            
            //position player on podium
            player.GetComponent<PlayerVisuals>().SetPosition(podium.PlayerPosition);
            
            //set player visible
            player.GetComponent<PlayerVisuals>().isVisible.Value = true;
        }
    }
}