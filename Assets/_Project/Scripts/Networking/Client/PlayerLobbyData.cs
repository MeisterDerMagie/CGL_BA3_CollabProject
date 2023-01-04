//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Wichtel.Extensions;

public class PlayerLobbyData : NetworkBehaviour
{
    public static PlayerLobbyData LocalPlayerLobbyData => (localPlayerLobbyDataCached == null || localPlayerLobbyDataCached.IsDestroyed()) ? CacheLocalPlayerLobbyData() : localPlayerLobbyDataCached;
    private static PlayerLobbyData localPlayerLobbyDataCached;

    private static PlayerLobbyData CacheLocalPlayerLobbyData()
    {
        localPlayerLobbyDataCached = (NetworkManager.Singleton == null || NetworkManager.Singleton.LocalClient == null || NetworkManager.Singleton.LocalClient.PlayerObject == null) ? null : NetworkManager.Singleton.LocalClient?.PlayerObject.GetComponent<PlayerLobbyData>();
        return localPlayerLobbyDataCached;
    }

    [HideInInspector] public bool pickedUniqueCharacter = false; //this is only checked locally. In theory, it's possible to bypass this and two players end up with the same character. In practice this is inprobable and even if, it's not tragic.
    
    public bool IsReadyInLobby
    {
        get
        {
            if(!IsLocalPlayer) return _isReadyInLobby.Value;
            return _isReadyInLobbyIsSynced ? _isReadyInLobby.Value : _isReadyInLobbyLocal;
        }
    }

    private bool _isReadyInLobbyLocal = false;
    private bool _isReadyInLobbyIsSynced = false;
    
    //Network variables
    private NetworkVariable<bool> _isReadyInLobby;

    private void Awake()
    {
        //instantiate NetworkVariables
        _isReadyInLobby = new NetworkVariable<bool>(false);
        
        //subscribe to change events+
        _isReadyInLobby.OnValueChanged += OnReadyStateChanged;
    }

    public override void OnDestroy()
    {
        //reset local data
        if (IsLocalPlayer) localPlayerLobbyDataCached = null;
        
        //unsubscribe from events
        _isReadyInLobby.OnValueChanged -= OnReadyStateChanged;

        //dispose network variables
        _isReadyInLobby.Dispose();
        
    }
    
    //Callbacks
    private void OnReadyStateChanged(bool previousvalue, bool newvalue)
    {
        if(newvalue == _isReadyInLobbyLocal) _isReadyInLobbyIsSynced = true;
    }
    
    //Setters
    public void SetReadyState(bool newState)
    {
        _isReadyInLobbyLocal = newState;
        _isReadyInLobbyIsSynced = false;
        SetReadyStateServerRpc(newState);
    }
    
    [ServerRpc]
    private void SetReadyStateServerRpc(bool newState)
    {
        /*
        //do this weird stuff to always call the onChangedEvent
        _isReadyInLobby.Value = true;
        _isReadyInLobby.Value = false;
        //--
        
        //check if the player picked a unique character
        if (newState == true)
        {
            uint ownCharacterId = GetComponent<PlayerData>().CharacterId;
            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (client.PlayerObject.GetComponent<PlayerData>().CharacterId == ownCharacterId)
                {
                    _isReadyInLobby.Value = false;
                    ForceUnreadyClientRpc();
                    return;
                }
            }
        }*/

        //now we actually set the value we want
        _isReadyInLobby.Value = newState;
    }

    [ClientRpc]
    private void ForceUnreadyClientRpc()
    {
        //force the client to be unready, in case they managed to ready up with a character that was picked by someone else (can happen with slow connections)
        SetReadyState(false);
    }
}