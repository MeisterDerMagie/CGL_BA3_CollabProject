using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LobbySeat : MonoBehaviour
{
    [ReadOnly]
    public int seatIndex;
    
    [SerializeField] private Transform ready, notReady;
    [SerializeField] private TextMeshProUGUI playerName;
    
    [SerializeField] private List<Transform> activeObjectsOnLocalPlayer;
    [SerializeField] private List<Transform> activeObjectsOnLocalAndRemotePlayers;
    [SerializeField] private List<Transform> activeObjectsOnRemotePlayers;

    private readonly List<PlayerData> _playerDatas = new List<PlayerData>();

    public PlayerData AssociatedPlayer => _associatedPlayer;
    private PlayerData _associatedPlayer;
    
    private void Start()
    {
        //initially deactivate all objects
        DeactivateAllUIElements();
    }

    //why update the UI every update? Because the client callbacks (OnClientConnectedCallback / Disconnected) are only triggered on the server and for the local client 
    private void Update()
    {
        GetAssociatedPlayer();
        UpdateSlotUI();
        UpdateCharacterId();
        UpdatePlayerName();
    }

    //gets the player that's associated to this slot
    private void GetAssociatedPlayer()
    {
        //update player list (yes, FindObjectsOfType is very inefficient and inelegant. But other "proper" solutions would be much more complex right now, so we'll live with this hack)
        _playerDatas.Clear();
        PlayerData[] players = FindObjectsOfType<PlayerData>();
        //sort players by ID
        Array.Sort(players, (player1, player2) => player1.OwnerClientId.CompareTo(player2.OwnerClientId));
        _playerDatas.AddRange(players);

        //reset in case a player left the room
        _associatedPlayer = null;
        
        //assign player to slot
        if (_playerDatas.Count >= seatIndex + 1)
            _associatedPlayer = _playerDatas[seatIndex];
    }
    
    //show / hide UI elements
    private void UpdateSlotUI()
    {
        //hide all UI elements, if this slot has no player assigned
        if (_associatedPlayer == null)
        {
            DeactivateAllUIElements();
            return;
        }
        
        //activate ui elements for local player
        if(_associatedPlayer.IsLocalPlayer) 
        {
            foreach (Transform t in activeObjectsOnLocalPlayer)
            {
                if(!t.gameObject.activeInHierarchy) t.gameObject.SetActive(true);
            }
            
            foreach (Transform t in activeObjectsOnRemotePlayers)
            {
                t.gameObject.SetActive(false);
            }
        }
        //activate ui elements only for remote players
        else
        {
            foreach (Transform t in activeObjectsOnLocalPlayer)
            {
                t.gameObject.SetActive(false);
            }

            foreach (Transform t in activeObjectsOnRemotePlayers)
            {
                t.gameObject.SetActive(true);
            }
        }
        
        
        //activate ui elements for local and remote players
        foreach (Transform t in activeObjectsOnLocalAndRemotePlayers)
        {
            if(!t.gameObject.activeInHierarchy) t.gameObject.SetActive(true);
        }
        
        //update ready status
        if (_associatedPlayer.IsReadyInLobby)
        {
            ready.gameObject.SetActive(true);
            notReady.gameObject.SetActive(false);
        }
        else
        {
            ready.gameObject.SetActive(false);
            notReady.gameObject.SetActive(true);
        }
    }

    private void DeactivateAllUIElements()
    {
        foreach (Transform t in activeObjectsOnLocalPlayer) { t.gameObject.SetActive(false); }
        foreach (Transform t in activeObjectsOnLocalAndRemotePlayers) { t.gameObject.SetActive(false); }
        foreach (Transform t in activeObjectsOnRemotePlayers) { t.gameObject.SetActive(false); }
        
        ready.gameObject.SetActive(false);
        notReady.gameObject.SetActive(false);
    }

    private void UpdateCharacterId()
    {
        if (_associatedPlayer == null) return;

        Debug.LogWarning("Update the displayed character in lobby here. (just the view)");
    }

    private void UpdatePlayerName()
    {
        if (_associatedPlayer == null) return;
        
        playerName.SetText(_associatedPlayer.PlayerName);
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        seatIndex = transform.GetSiblingIndex();
    }
    #endif
}