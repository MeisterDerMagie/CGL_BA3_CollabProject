using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class DetailedScores : NetworkBehaviour
{
    [SerializeField] private Transform detailedScoresUI;
    
    [SerializeField]
    private TextMeshProUGUI creativityPointsTextField, playabilityPointsTextField, performancePointsTextField;

    private void Start() => HideDetailedScores();

    [ClientRpc]
    public void SetDetailedScoresClientRpc(int creativityPoints, int playabilityPoints, int performancePoints)
    {
        creativityPointsTextField.SetText(creativityPoints.ToString());
        playabilityPointsTextField.SetText(playabilityPoints.ToString());
        performancePointsTextField.SetText(performancePoints.ToString());
    }

    public void ShowDetailedScores()
    {
        detailedScoresUI.gameObject.SetActive(true);
    }

    public void HideDetailedScores()
    {
        detailedScoresUI.gameObject.SetActive(false);
    }
}
