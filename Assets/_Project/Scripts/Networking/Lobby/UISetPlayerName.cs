//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using TMPro;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(LobbySeat))]
public class UISetPlayerName : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    private LobbySeat _lobbySeat;
    
    private void Start()
    {
        _lobbySeat = GetComponent<LobbySeat>();

        //load initial name
        if(!NetworkManager.Singleton.IsServer) Timing.RunCoroutine(_SetInitialNameAsSoonAsPlayerDataIsAvailable());
    }

    //as soon as the player data is loaded, we initially set the player name
    private IEnumerator<float> _SetInitialNameAsSoonAsPlayerDataIsAvailable()
    {
        yield return Timing.WaitForOneFrame;
        
        bool wait = true;
        while (wait)
        {
            if (PlayerData.LocalPlayerData == null) yield return Timing.WaitForOneFrame;

            inputField.text = PlayerData.LocalPlayerData.PlayerName;
            wait = false;
            yield return Timing.WaitForOneFrame;
        }
    }

    private void Update()
    {
        //Update the player name
        if(!NetworkManager.Singleton.IsServer && !inputField.isFocused && PlayerData.LocalPlayerData != null) inputField.SetTextWithoutNotify(PlayerData.LocalPlayerData.PlayerName);
    }

    public void ConfirmPlayerName()
    {
        if (!_lobbySeat.AssociatedPlayer.IsLocalPlayer) return;
        
        _lobbySeat.AssociatedPlayer.SetPlayerName(inputField.text);
    }
}