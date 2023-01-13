using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class JuryController : MonoBehaviour
{
    [SerializeField] private TextMeshPro textFieldShield1, textFieldShield2, textFieldShield3;
    
    private void Start()
    {
        if (NetworkManager.Singleton.IsServer) return;
        
        textFieldShield1.SetText(PlayerData.LocalPlayerData.PointsCreativity.ToString());
        textFieldShield2.SetText(PlayerData.LocalPlayerData.PointsPlayability.ToString());
        textFieldShield3.SetText(PlayerData.LocalPlayerData.PointsPerformance.ToString());
    }
}
