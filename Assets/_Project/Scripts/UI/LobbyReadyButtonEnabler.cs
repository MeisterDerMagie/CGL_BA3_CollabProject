//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LobbyReadyButtonEnabler : MonoBehaviour
{
    private Button _button;
    
    private void Awake() => _button = GetComponent<Button>();

    private void Update()
    {
        if(!NetworkManager.Singleton.IsServer) _button.interactable = PlayerLobbyData.LocalPlayerLobbyData.pickedUniqueCharacter;
    }
}