using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class DisplayLocalPlayerAssignedPrompt : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textField;
    
    private void Update()
    {
        if (NetworkManager.Singleton.IsServer || PlayerData.LocalPlayerData == null) return;
        
        textField.SetText(PlayerData.LocalPlayerData.AssignedPrompt);       
    }
}
