using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayLobbyCode : MonoBehaviour
{
    [SerializeField] private Transform lobbyCodeUI;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;
    
    private void Start()
    {
        //hide lobby code if none exists (this will happen if you play locally)
        lobbyCodeUI.gameObject.SetActive(LobbyCode.Instance.code != string.Empty);

        lobbyCodeText.SetText(LobbyCode.Instance.code);
    }
}
