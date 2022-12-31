using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Podium : NetworkBehaviour
{
    [SerializeField] private Transform playerPosition;
    [SerializeField] private TextMeshProUGUI playerNameTextField, podiumTextField;
        
    public Vector2 PlayerPosition => playerPosition.position;
    
    private NetworkVariable<bool> _isActive = new NetworkVariable<bool>();

    private void Awake()
    {
        playerNameTextField.gameObject.SetActive(false);
        podiumTextField.gameObject.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        _isActive.OnValueChanged += OnActiveStateChanged;
        gameObject.SetActive(_isActive.Value);
    }

    public void SetActive(bool isActive)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogError("This method should only be called on the server.");
            return;
        }

        _isActive.Value = isActive;
        
        gameObject.SetActive(isActive);
    }

    [ClientRpc]
    public void AssignPlayerNameClientRpc(string playerName)
    {
        playerNameTextField.gameObject.SetActive(true);
        playerNameTextField.SetText(playerName);
    }

    [ClientRpc]
    public void SetTextColorPlayerNameClientRpc(Color color, ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId) return;
        playerNameTextField.color = color;
    }

    [ClientRpc]
    public void SetPodiumTextClientRpc(string text)
    {
        SetPodiumText(text);
    }

    public void SetPodiumText(string text)
    {
        podiumTextField.gameObject.SetActive(true);
        podiumTextField.SetText(text);
    }
    
    private void OnActiveStateChanged(bool previousvalue, bool newvalue)
    {
        gameObject.SetActive(newvalue);
    }
}
