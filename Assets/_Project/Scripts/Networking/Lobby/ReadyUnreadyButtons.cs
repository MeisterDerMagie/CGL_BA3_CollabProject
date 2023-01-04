using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ReadyUnreadyButtons : MonoBehaviour
{
    [SerializeField] private Button readyButton, unreadyButton;
    
    void Update()
    {
        if (NetworkManager.Singleton.IsServer) return;
        if (PlayerLobbyData.LocalPlayerLobbyData == null) return;
        
        if (!PlayerLobbyData.LocalPlayerLobbyData.IsReadyInLobby && !readyButton.gameObject.activeSelf)
        {
            readyButton.gameObject.SetActive(true);
            unreadyButton.gameObject.SetActive(false);
        }
        else if (PlayerLobbyData.LocalPlayerLobbyData.IsReadyInLobby && !unreadyButton.gameObject.activeSelf)
        {
            readyButton.gameObject.SetActive(false);
            unreadyButton.gameObject.SetActive(true);
        }
    }
}
