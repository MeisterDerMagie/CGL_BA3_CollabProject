//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class DisplayLocalPlayerPrompt : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textField;
    
    private void Update()
    {
        if (NetworkManager.Singleton == null || NetworkManager.Singleton.IsServer || PlayerData.LocalPlayerData == null) return;
        
        textField.SetText(PlayerData.LocalPlayerData.Prompt);
    }
}