//(c) copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Wichtel.Extensions;

public class ScoreViewController : NetworkBehaviour
{
    [SerializeField] private StageController stageController;
    [SerializeField] private List<DetailedScores> detailedScores;

    public void Start()
    {
        //only run on server
        if (!NetworkManager.Singleton.IsServer) return;

        stageController.OnInitialized += Initialize;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if(stageController != null && !stageController.IsDestroyed()) stageController.OnInitialized -= Initialize;
    }

    private void Initialize()
    {
        //set points on podiums
        for (int i = 0; i < stageController.podiums.Count; i++)
        {
            //Total Points
            Podium podium = stageController.podiums[i];

            ulong clientId = stageController.GetClientIdAssignedToPodium(i);

            if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId)) continue;
            
            PlayerData playerData = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerData>();
            int totalPoints = playerData.TotalPoints;
            
            podium.SetPodiumTextClientRpc(totalPoints.ToString());
            
            //Detailed Points
            Debug.Log($"Set detailed points: podium {i.ToString()}, creat: {playerData.PointsCreativity.ToString()}, playab: {playerData.PointsPlayability.ToString()}, perf: {playerData.PointsPerformance.ToString()}");
            detailedScores[i].SetDetailedScoresClientRpc(playerData.PointsCreativity, playerData.PointsPlayability, playerData.PointsPerformance);
        }
    }
}